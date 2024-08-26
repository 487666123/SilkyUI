using SilkyUI.UserInterfaces;

namespace SilkyUI;

#region enum and struct
public enum MouseButtonType { Left, Middle, Right }
public enum MouseEventType { Down, Up }

/// <summary>
/// 鼠标按键状态
/// </summary>
public struct MouseStatusMinor(bool leftButton, bool middleButton, bool rightButton)
{
    public bool
        LeftButton = leftButton,
        MiddleButton = middleButton,
        RightButton = rightButton;

    public void SetState(bool leftButton, bool middleButton, bool rightButton)
    {
        LeftButton = leftButton; MiddleButton = middleButton; RightButton = rightButton;
    }

    public readonly bool this[MouseButtonType button] => button switch
    {
        MouseButtonType.Left => LeftButton,
        MouseButtonType.Middle => MiddleButton,
        MouseButtonType.Right => RightButton,
        _ => throw new NotImplementedException()
    };
}

public struct MouseTarget()
{
    public UIElement LeftButton = null, MiddleButton = null, RightButton = null;

    public UIElement this[MouseButtonType button]
    {
        readonly get
        {
            return button switch
            {
                MouseButtonType.Left => LeftButton,
                MouseButtonType.Middle => MiddleButton,
                MouseButtonType.Right => RightButton,
                _ => throw new NotImplementedException()
            };
        }
        set
        {
            switch (button)
            {
                case MouseButtonType.Left:
                    LeftButton = value;
                    break;
                case MouseButtonType.Right:
                    RightButton = value;
                    break;
                case MouseButtonType.Middle:
                    MiddleButton = value;
                    break;
            }
        }
    }
}
#endregion

/// <summary>
/// 处理交互逻辑
/// </summary>
/// <param name="insertionPoint"></param>
/// <param name="name"></param>
public class SilkyUserInterface()
{
    public static SilkyUserInterfaceManager Manager => SilkyUserInterfaceManager.Instance;

    /// <summary>
    /// 变换矩阵
    /// </summary>
    public Matrix TransformMatrix;

    /// <summary>
    /// 操控主体
    /// </summary>
    public BasicBody BasicBody { get; private set; }

    public Vector2 MouseFocus { get; private set; }
    public UIElement CurrentHoverTarget { get; private set; }
    public UIElement PreviousHoverTarget { get; private set; }

    public MouseTarget PreviousMouseTargets => _previousMouseTargets;
    private MouseTarget _previousMouseTargets = new();

    private MouseStatusMinor _mouseStatus;
    private MouseStatusMinor _previousMouseStatus;

    public void SetBasicBody(BasicBody basicBody)
    {
        if (BasicBody != basicBody)
        {
            BasicBody = basicBody;

            if (BasicBody != null)
            {
                BasicBody.Activate();
                BasicBody.Recalculate();
            }
        }
    }

    /// <summary>
    /// 更新鼠标位置
    /// </summary>
    public void UpdateMouseFocus()
    {
        MouseFocus = SilkyUserInterfaceManager.MouseScreen;
    }

    /// <summary>
    /// 更新鼠标悬停目标
    /// </summary>
    public void UpdateHoverTarget()
    {
        PreviousHoverTarget = CurrentHoverTarget;
        CurrentHoverTarget =
            Manager.MouseFocusHasUIElement ||
            BasicBody.IsNotSelectable ? null : BasicBody.GetElementAt(Vector2.Transform(MouseFocus, TransformMatrix));
    }

    public void Update(GameTime gameTime)
    {
        if (BasicBody is null or { Enabled: false })
        {
            return;
        }

        UpdateMouseFocus();

        _previousMouseStatus = _mouseStatus;
        _mouseStatus.SetState(Main.mouseLeft, Main.mouseMiddle, Main.mouseRight);

        try
        {
            UpdateHoverTarget();

            if (BasicBody.CanSetFocusTarget(CurrentHoverTarget))
            {
                Manager.MouseFocusUIElement = CurrentHoverTarget;
            }

            if (CurrentHoverTarget != PreviousHoverTarget)
            {
                PreviousHoverTarget?.MouseOut(new(CurrentHoverTarget, MouseFocus));
                CurrentHoverTarget?.MouseOver(new(CurrentHoverTarget, MouseFocus));
            }

            // 遍历三种鼠标按键：左键、右键和中键
            foreach (MouseButtonType mouseButton in Enum.GetValues(typeof(MouseButtonType)))
            {
                // 判断当前按键是否被按下
                if (_mouseStatus[mouseButton] && !_previousMouseStatus[mouseButton])
                {
                    // 如果目标元素存在且可以被优先处理，则将视图置于顶层
                    if (CurrentHoverTarget is not null && Manager.MouseFocusUIElement == CurrentHoverTarget)
                    {
                        Manager.MoveCurrentUserIntrerfaceToTop();
                    }

                    HandleMouseEvent(MouseEventType.Down, mouseButton);
                }
                else if (!_mouseStatus[mouseButton] && _previousMouseStatus[mouseButton])
                {
                    HandleMouseEvent(MouseEventType.Up, mouseButton);
                }
            }

            if (PlayerInput.ScrollWheelDeltaForUI != 0)
            {
                CurrentHoverTarget?.ScrollWheel(new UIScrollWheelEvent(CurrentHoverTarget, MouseFocus, PlayerInput.ScrollWheelDeltaForUI));
            }

            BasicBody.Update(gameTime);
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
    }

    public static Action<UIMouseEvent> GetMouseDownEvent(MouseButtonType mouseButtonType, UIElement element)
    {
        if (element is null)
            return null;

        return mouseButtonType switch
        {
            MouseButtonType.Left => element.LeftMouseDown,
            MouseButtonType.Right => element.RightMouseDown,
            MouseButtonType.Middle => element.MiddleMouseDown,
            _ => null,
        };
    }

    public static Action<UIMouseEvent> GetMouseUpEvent(MouseButtonType mouseButtonType, UIElement element)
    {
        if (element is null)
            return null;

        return mouseButtonType switch
        {
            MouseButtonType.Left => element.LeftMouseUp,
            MouseButtonType.Right => element.RightMouseUp,
            MouseButtonType.Middle => element.MiddleMouseUp,
            _ => null,
        };
    }

    public static Action<UIMouseEvent> GetClickEvent(MouseButtonType mouseButtonType, UIElement element)
    {
        if (element is null)
            return null;

        return mouseButtonType switch
        {
            MouseButtonType.Left => element.LeftClick,
            MouseButtonType.Right => element.RightClick,
            MouseButtonType.Middle => element.MiddleClick,
            _ => null,
        };
    }

    private void HandleMouseEvent(MouseEventType eventType, MouseButtonType mouseButton)
    {
        UIMouseEvent evt;

        if (eventType is MouseEventType.Down)
        {
            evt = new UIMouseEvent(CurrentHoverTarget, MouseFocus);
            GetMouseDownEvent(mouseButton, CurrentHoverTarget)?.Invoke(evt);
        }
        else
        {
            evt = new UIMouseEvent(CurrentHoverTarget, MouseFocus);
            GetMouseUpEvent(mouseButton, _previousMouseTargets[mouseButton])?.Invoke(evt);

            if (_previousMouseTargets[mouseButton] == CurrentHoverTarget)
            {
                evt = new UIMouseEvent(CurrentHoverTarget, MouseFocus);
                GetClickEvent(mouseButton, _previousMouseTargets[mouseButton])?.Invoke(evt);
            }
        }

        _previousMouseTargets[mouseButton] = CurrentHoverTarget;
    }

    /// <summary>
    /// 绘制 UI
    /// </summary>
    public bool Draw()
    {
        if (BasicBody?.Enabled ?? false)
        {
            BasicBody.Draw(Main.spriteBatch);
        }

        return true;
    }
}