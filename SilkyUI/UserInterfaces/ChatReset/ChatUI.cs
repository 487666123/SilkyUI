using SilkyUI.BasicComponents;
using SilkyUI.UserInterfaces.ChatReset.Components;

namespace SilkyUI.UserInterfaces.ChatReset;

[Attributes.Autoload("Vanilla: Radial Hotbars", "SilkyUI: ExampleUI")]
public class ChatUI : BasicBody
{
    public SUIDraggableView MainPanel { get; private set; }
    public SUIScrollView ChatContainer { get; private set; }
    public View ActiveBarContainer { get; private set; }

    public override void OnInitialize()
    {
        UseRenderTarget = true;

        var backgroundColor = new Color(63, 65, 151);
        var borderColor = new Color(18, 18, 38);

        MainPanel = new SUIDraggableView
        {
            Display = Display.Flow,
            Shaded = true,
            ShadowThickness = 25f,
            ShadowColor = borderColor * 0.1f,
            Border = 2f,
            Gap = new Vector2(-0.5f),
            FinallyDrawBorder = true,
            CornerRadius = new Vector4(12f),
            Draggable = false,
            DragIncrement = new Vector2(5f)
        }.Join(this);
        MainPanel.HAlign = 0.5f;
        MainPanel.VAlign = 0.5f;
        MainPanel.SetWidth(550f);
        MainPanel.SetPadding(-1f);

        ChatContainer = new SUIScrollView(Direction.Vertical)
        {
            CornerRadius = new Vector4(10f, 10f, 0f, 0f),
        }.Join(MainPanel);
        ChatContainer.SetPadding(8f);
        ChatContainer.SetSize(0, 400f, 1f);

        // 分界线
        var br = new View
        {
            BgColor = borderColor * 0.75f,
            ZIndex = 1f,
        }.Join(MainPanel);
        br.SetSize(0, 2f, 1f);

        InputContainerInitialize();
        ChatContainer.ScrollBar.ScrollByEnd();
    }

    private void InputContainerInitialize()
    {
        var backgroundColor = new Color(63, 65, 151);
        var borderColor = new Color(18, 18, 38);

        ActiveBarContainer = new View
        {
            Display = Display.Flexbox,
            FlexDirection = FlexDirection.Row,
            MainAxisAlignment = MainAxisAlignment.SpaceBetween,
            CornerRadius = new Vector4(0f, 0f, 10f, 10f),
            BgColor = Color.Black * 0.1f,
        }.Join(MainPanel);
        ActiveBarContainer.SetPadding(8f);
        ActiveBarContainer.SetSize(0, 80f, 1f);

        var inputContainer = new SUIScrollView(Direction.Vertical)
        {
            CornerRadius = new Vector4(8f),
            Border = 2,
            BorderColor = borderColor * 0.75f,
            BgColor = backgroundColor * 0.75f,
        }.Join(ActiveBarContainer);
        inputContainer.SetPadding(8f);
        inputContainer.SetSize(-140f - 16f, 0f, 1f, 1f);

        var input = new SUIEditText
        {
            TextAlign = new Vector2(0, 0f),
            OverflowHidden = true,
            WordWrap = true,
            MinHeight = { Percent = 1f },
            PaddingLeft = 2f,
            PaddingRight = 2f,
        }.Join(inputContainer.Container);
        input.OnTextChanged += () =>
        {
            inputContainer.Recalculate();
            inputContainer.ScrollBar.ScrollByEnd();
        };
        input.OnEnterKeyDown += () => { SendMessage(input); };
        input.SetWidth(0f, 1f);

        var clearButton = new SUIText
        {
            CornerRadius = new Vector4(8f),
            Border = 2,
            BorderColor = borderColor * 0.75f,
            BgColor = backgroundColor * 0.75f,
            Text = "清空",
            TextAlign = new Vector2(0.5f),
            DragIgnore = false,
            TextColor = Color.Red
        }.Join(ActiveBarContainer);
        clearButton.SetPadding(-1f);
        clearButton.SetSize(70f, 0f, 0f, 1f);
        clearButton.OnLeftMouseDown += (_, _) =>
        {
            ChatContainer?.Container?.RemoveAllChildren();
            ChatContainer?.Recalculate();
        };

        var send = new SUIText
        {
            CornerRadius = new Vector4(8f),
            Border = 2,
            BorderColor = borderColor * 0.75f,
            BgColor = backgroundColor * 0.75f,
            Text = "发送",
            TextAlign = new Vector2(0.5f),
            DragIgnore = false,
        }.Join(ActiveBarContainer);
        send.SetPadding(-1f);
        send.SetSize(70f, 0f, 0f, 1f);
        send.OnLeftMouseDown += (_, _) => { SendMessage(input); };
    }

    public void SendMessage(SUIEditText input)
    {
        if (ChatContainer.Container != null)
        {
            var count = ChatContainer.Container.Children.Count() - 50;
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    ChatContainer.Container.Children.First().Remove();
                }

                ChatContainer.Recalculate();
                ChatContainer.ScrollBar.CurrentScrollPosition = ChatContainer.ScrollBar.CurrentScrollPosition;
            }
        }

        var sender = Main.LocalPlayer.name;
        var message = input.Text;
        input.Text = string.Empty;
        new MessageComp(sender, message, true)
            .Join(ChatContainer.Container);

        ChatContainer.Recalculate();
        ChatContainer.ScrollBar.ScrollByEnd();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        UseRenderTarget = false;

        MainPanel.HoverTimer.Speed = 15f;
        Opacity = MainPanel.HoverTimer.Lerp(0f, 1f);

        base.Draw(spriteBatch);
    }
}