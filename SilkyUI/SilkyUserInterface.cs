namespace SilkyUI;

#region enum and struct

public enum MouseButtonType
{
    Left,
    Middle,
    Right
}

public enum MouseEventType
{
    Down,
    Up
}

/// <summary>
/// 鼠标按键状态
/// </summary>
public class MouseStatusMinor
{
    private bool
        _leftButton,
        _middleButton,
        _rightButton;

    public void SetState(MouseStatusMinor status)
    {
        _leftButton = status._leftButton;
        _middleButton = status._middleButton;
        _rightButton = status._rightButton;
    }

    public void SetState(bool leftButton, bool middleButton, bool rightButton)
    {
        _leftButton = leftButton;
        _middleButton = middleButton;
        _rightButton = rightButton;
    }

    public bool this[MouseButtonType button]
    {
        get
        {
            return button switch
            {
                MouseButtonType.Left => _leftButton,
                MouseButtonType.Middle => _middleButton,
                MouseButtonType.Right => _rightButton,
                _ => _leftButton,
            };
        }
        set
        {
            switch (button)
            {
                default:
                case MouseButtonType.Left:
                    _leftButton = value;
                    break;
                case MouseButtonType.Right:
                    _rightButton = value;
                    break;
                case MouseButtonType.Middle:
                    _middleButton = value;
                    break;
            }
        }
    }
}

public class MouseTarget
{
    public UIElement LeftButton, MiddleButton, RightButton;

    public UIElement this[MouseButtonType button]
    {
        get
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
                default:
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
public class SilkyUserInterface
{
    public static SilkyUserInterfaceManager Manager => SilkyUserInterfaceManager.Instance;
    public Matrix TransformMatrix;
    public BasicBody BasicBody { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public UIElement CurrentHoverTarget { get; private set; }
    public UIElement LastHoverTarget { get; private set; }
    public MouseTarget LastMouseTargets { get; } = new();

    private readonly MouseStatusMinor _mouseStatus = new();
    private readonly MouseStatusMinor _lastMouseStatus = new();

    public void SetBasicBody(BasicBody basicBody = null)
    {
        if (BasicBody == basicBody) return;
        BasicBody = basicBody;

        if (BasicBody == null) return;
        BasicBody.Activate();
        BasicBody.Recalculate();
    }

    /// <summary>
    /// 更新鼠标位置
    /// </summary>
    private void UpdateMousePosition() => MousePosition = SilkyUserInterfaceManager.MouseScreen;

    /// <summary>
    /// 更新鼠标悬停目标
    /// </summary>
    private void UpdateHoverTarget()
    {
        LastHoverTarget = CurrentHoverTarget;
        CurrentHoverTarget =
            Manager.HasMouseFocusUIElement ||
            BasicBody.IsNotSelectable
                ? null
                : BasicBody.GetElementAt(Vector2.Transform(MousePosition, TransformMatrix));
    }

    public void Update(GameTime gameTime)
    {
        if (BasicBody is null or { IsEnabled: false }) return;

        UpdateMousePosition();

        _lastMouseStatus.SetState(_mouseStatus);
        _mouseStatus.SetState(Main.mouseLeft, Main.mouseMiddle, Main.mouseRight);

        try
        {
            UpdateHoverTarget();

            if (BasicBody.CanSetFocusTarget(CurrentHoverTarget))
            {
                Manager.MouseFocusUIElement = CurrentHoverTarget;
            }

            if (CurrentHoverTarget != LastHoverTarget)
            {
                LastHoverTarget?.MouseOut(new UIMouseEvent(CurrentHoverTarget, MousePosition));
                CurrentHoverTarget?.MouseOver(new UIMouseEvent(CurrentHoverTarget, MousePosition));
            }

            // 遍历三种鼠标按键：左键、右键和中键
            foreach (MouseButtonType mouseButton in Enum.GetValues(typeof(MouseButtonType)))
            {
                switch (_mouseStatus[mouseButton])
                {
                    // 判断当前按键是否被按下
                    case true when !_lastMouseStatus[mouseButton]:
                    {
                        // 如果目标元素存在且可以被优先处理，则将视图置于顶层
                        if (CurrentHoverTarget is not null && Manager.MouseFocusUIElement == CurrentHoverTarget)
                        {
                            Manager.MoveCurrentUserInterfaceToTop();
                        }

                        HandleMouseEvent(MouseEventType.Down, mouseButton);
                        break;
                    }
                    case false when _lastMouseStatus[mouseButton]:
                        HandleMouseEvent(MouseEventType.Up, mouseButton);
                        break;
                }
            }

            if (PlayerInput.ScrollWheelDeltaForUI != 0)
            {
                CurrentHoverTarget?.ScrollWheel(new UIScrollWheelEvent(CurrentHoverTarget, MousePosition,
                    PlayerInput.ScrollWheelDeltaForUI));
            }

            BasicBody.Update(gameTime);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private static Action<UIMouseEvent> GetMouseDownEvent(MouseButtonType mouseButtonType, UIElement element)
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

    private static Action<UIMouseEvent> GetMouseUpEvent(MouseButtonType mouseButtonType, UIElement element)
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

    private static Action<UIMouseEvent> GetClickEvent(MouseButtonType mouseButtonType, UIElement element)
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
            evt = new UIMouseEvent(CurrentHoverTarget, MousePosition);
            GetMouseDownEvent(mouseButton, CurrentHoverTarget)?.Invoke(evt);
        }
        else
        {
            evt = new UIMouseEvent(CurrentHoverTarget, MousePosition);
            GetMouseUpEvent(mouseButton, LastMouseTargets[mouseButton])?.Invoke(evt);

            if (LastMouseTargets[mouseButton] == CurrentHoverTarget)
            {
                evt = new UIMouseEvent(CurrentHoverTarget, MousePosition);
                GetClickEvent(mouseButton, LastMouseTargets[mouseButton])?.Invoke(evt);
            }
        }

        LastMouseTargets[mouseButton] = CurrentHoverTarget;
    }

    /// <summary>
    /// 绘制 UI
    /// </summary>
    public bool Draw()
    {
        if (BasicBody?.IsEnabled ?? false) BasicBody.Draw(Main.spriteBatch);
        return true;
    }
}