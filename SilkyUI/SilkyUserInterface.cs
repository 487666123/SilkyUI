using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;

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

public class MouseStatus
{
    private bool
        _leftButton,
        _middleButton,
        _rightButton;

    public void SetState(MouseStatus status)
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
    private UIElement _leftButton;
    private UIElement _middleButton;
    private UIElement _rightButton;

    public UIElement this[MouseButtonType button]
    {
        get => button switch
        {
            MouseButtonType.Left => _leftButton,
            MouseButtonType.Middle => _middleButton,
            MouseButtonType.Right => _rightButton,
            _ => _leftButton
        };
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
    public UIElement FocusTarget { get; private set; }

    private readonly MouseStatus _mouseStatus = new();
    private readonly MouseStatus _lastMouseStatus = new();

    public void SetBasicBody(BasicBody basicBody = null)
    {
        if (BasicBody == basicBody) return;
        BasicBody = basicBody;

        if (BasicBody == null) return;
        BasicBody.Activate();
        BasicBody.Recalculate();
    }

    public void Update(GameTime gameTime)
    {
        if (BasicBody is null or { IsEnabled: false }) return;

        MousePosition = SilkyUserInterfaceManager.MouseScreen;

        _lastMouseStatus.SetState(_mouseStatus);
        _mouseStatus.SetState(Main.mouseLeft, Main.mouseMiddle, Main.mouseRight);

        try
        {
            // Update HoverTarget
            LastHoverTarget = CurrentHoverTarget;
            CurrentHoverTarget =
                Manager.HasMouseHoverElement ||
                BasicBody.IsNotSelectable
                    ? null
                    : BasicBody.GetElementAt(Vector2.Transform(MousePosition, TransformMatrix));

            // 悬浮元素能否交互, 可交互将 Manager 悬浮目标设为它, 设置后会阻止下层元素交互
            if (BasicBody.AreHoverTargetInteractive(CurrentHoverTarget))
                Manager.MouseHoverTarget = CurrentHoverTarget;

            // 当切换悬浮目标
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
                        if (CurrentHoverTarget is not null && Manager.MouseHoverTarget == CurrentHoverTarget)
                        {
                            Manager.CurrentUserInterfaceMoveToTop();
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
        if (element is null) return null;
        return mouseButtonType switch
        {
            MouseButtonType.Left => element.LeftMouseDown,
            MouseButtonType.Right => element.RightMouseDown,
            MouseButtonType.Middle => element.MiddleMouseDown,
            _ => element.LeftMouseDown,
        };
    }

    private static Action<UIMouseEvent> GetMouseUpEvent(MouseButtonType mouseButtonType, UIElement element)
    {
        if (element is null) return null;
        return mouseButtonType switch
        {
            MouseButtonType.Left => element.LeftMouseUp,
            MouseButtonType.Right => element.RightMouseUp,
            MouseButtonType.Middle => element.MiddleMouseUp,
            _ => element.LeftMouseUp,
        };
    }

    private static Action<UIMouseEvent> GetClickEvent(MouseButtonType mouseButtonType, UIElement element)
    {
        if (element is null) return null;
        return mouseButtonType switch
        {
            MouseButtonType.Left => element.LeftClick,
            MouseButtonType.Right => element.RightClick,
            MouseButtonType.Middle => element.MiddleClick,
            _ => element.LeftClick,
        };
    }

    private void HandleMouseEvent(MouseEventType eventType, MouseButtonType mouseButton)
    {
        var evt = new UIMouseEvent(CurrentHoverTarget, MousePosition);

        switch (eventType)
        {
            default:
            case MouseEventType.Down:
                FocusTarget = CurrentHoverTarget;
                GetMouseDownEvent(mouseButton, CurrentHoverTarget)?.Invoke(evt);
                break;
            case MouseEventType.Up:
                GetMouseUpEvent(mouseButton, LastMouseTargets[mouseButton])?.Invoke(evt);

                if (LastMouseTargets[mouseButton] == CurrentHoverTarget)
                {
                    evt = new UIMouseEvent(CurrentHoverTarget, MousePosition);
                    GetClickEvent(mouseButton, LastMouseTargets[mouseButton])?.Invoke(evt);
                }

                break;
        }

        LastMouseTargets[mouseButton] = CurrentHoverTarget;
    }

    public bool DrawUI()
    {
        if (BasicBody is not { IsEnabled: true })
            return true;

        HandlePlayerInput();
        BasicBody.Draw(Main.spriteBatch);

        return true;
    }

    private void HandlePlayerInput()
    {
        if (FocusTarget is not IInputElement { OccupyInput: true } inputElement) return;

        PlayerInput.WritingText = true;
        Main.instance.HandleIME();
        var inputText = GetInputText();

        inputElement.OnInput(inputText);
    }

    /// <summary>
    /// keyboard input
    /// </summary>
    /// <returns></returns>
    public static string KeyboardInput()
    {
        var text = "";

        for (var i = 0; i < Main.keyCount; i++)
        {
            var key = (Keys)Main.keyInt[i]; // 键值
            var keyString = Main.keyString[i]; // 键对应的字符
            switch (key)
            {
                case Keys.Enter:
                    Main.inputTextEnter = true; // Enter 键
                    break;
                case Keys.Escape:
                    Main.inputTextEscape = true; // Escape 键
                    break;
                case >= Keys.Space when (sbyte)key != sbyte.MaxValue:
                    text += keyString; // 处理其他字符输入
                    break;
            }
        }

        return text;
    }

    /// <summary>
    /// Main.GetInputText
    /// </summary>
    public static string GetInputText(string oldString = null, bool allowMultiLine = false)
    {
        if (Main.dedServ) return ""; // 如果是服务器模式，直接返回空字符串
        if (!Main.hasFocus) return oldString; // 如果窗口没有焦点，返回原始字符串

        Main.inputTextEnter = false; // 重置 Enter 键状态
        Main.inputTextEscape = false; // 重置 Escape 键状态

        var newString = oldString ?? ""; // 如果 oldString 为 null，则初始化为 ""
        var newKeys = ""; // 用于存储输入的字符
        var flag1 = false; // 标记是否进行特殊删除操作

        // 检查是否按下了 Ctrl 键，并执行对应的快捷操作
        if (Main.inputText.IsControlKeyDown() && !Main.inputText.IsAltKeyDown())
        {
            if (Keys.Z.JustPressed())
            {
                newString = ""; // Ctrl + Z 清空输入
            }
            else if (Keys.Z.JustPressed())
            {
                Platform.Get<IClipboard>().Value = oldString; // Ctrl + X 剪切到剪贴板
                newString = "";
            }
            else if (Keys.C.JustPressed() || Keys.Insert.JustPressed())
            {
                Platform.Get<IClipboard>().Value = oldString; // Ctrl + C 复制到剪贴板
            }
            else if (Keys.V.JustPressed())
            {
                newKeys = Main.PasteTextIn(allowMultiLine, newKeys); // Ctrl + V 粘贴
            }
        }
        else
        {
            // 检查是否按下了 Shift 键，处理删除操作
            if (Main.inputText.IsShiftKeyDown())
            {
                if (Keys.Delete.JustPressed())
                {
                    Platform.Get<IClipboard>().Value = oldString; // Shift + Delete 剪切到剪贴板
                    newString = "";
                }

                if (Keys.Insert.JustPressed())
                {
                    newKeys = Main.PasteTextIn(allowMultiLine, newKeys); // Shift + Insert 粘贴
                }
            }

            newKeys += KeyboardInput(); // 获取键盘输入
        }

        Main.keyCount = 0; // 重置按键计数
        var text = newString + newKeys; // 合并输入的字符串
        Main.oldInputText = Main.inputText; // 更新旧的输入状态
        Main.inputText = Keyboard.GetState(); // 更新当前输入状态

        var pressedKeys = Main.inputText.GetPressedKeys(); // 当前按下的键
        var oldPressedKeys = Main.oldInputText.GetPressedKeys(); // 上一帧按下的键

        // 处理退格键 (Backspace) 操作
        if (Main.inputText.IsKeyDown(Keys.Back) && Main.oldInputText.IsKeyDown(Keys.Back))
        {
            Main.backSpaceRate -= 0.05f; // 退格速度调整
            if ((double)Main.backSpaceRate < 0.0)
                Main.backSpaceRate = 0.0f;
            if (Main.backSpaceCount <= 0)
            {
                Main.backSpaceCount = (int)Math.Round((double)Main.backSpaceRate);
                flag1 = true;
            }

            --Main.backSpaceCount; // 更新退格计数
        }
        else
        {
            Main.backSpaceRate = 7f; // 恢复默认退格速度
            Main.backSpaceCount = 15;
        }

        // 处理按键变化
        for (var index1 = 0; index1 < pressedKeys.Length; ++index1)
        {
            var flag2 = true;
            for (var index2 = 0; index2 < oldPressedKeys.Length; ++index2)
            {
                if (pressedKeys[index1] == oldPressedKeys[index2])
                    flag2 = false;
            }

            // 如果按下退格键且没有变化，处理文本删除
            if (string.Concat((object)pressedKeys[index1]) == "Back" && flag2 || flag1 && text.Length > 0)
            {
                var array = ChatManager.ParseMessage(text, Color.White).ToArray();
                text = !array[array.Length - 1].DeleteWhole
                    ? text.Substring(0, text.Length - 1)
                    : text.Substring(0, text.Length - array[array.Length - 1].TextOriginal.Length);
            }
        }

        return text; // 返回最终的输入文本
    }
}