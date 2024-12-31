﻿using SilkyUI.BasicComponents;
using SilkyUI.UserInterfaces.ChatReset.Components;

namespace SilkyUI.UserInterfaces.ChatReset;

[Attributes.Autoload("Vanilla: Radial Hotbars", "SilkyUI: ExampleUI")]
public class ChatUI : BasicBody
{
    public SUIDraggableView MainPanel { get; private set; }
    public View ChatContainer { get; private set; }
    public View InputContainer { get; private set; }

    public override void OnInitialize()
    {
        UseRenderTarget = true;

        var backgroundColor = new Color(63, 65, 151);
        var borderColor = new Color(18, 18, 38);

        MainPanel = new SUIDraggableView(backgroundColor * 0.75f, borderColor * 0.75f, draggable: true)
        {
            Display = Display.Flexbox,
            FlexDirection = FlexDirection.Column,
            Shaded = true,
            ShadowThickness = 25f,
            ShadowColor = borderColor * 0.1f,
            Border = 2f,
            Gap = new Vector2(-1f),
            FinallyDrawBorder = true,
            CornerRadius = new Vector4(12f),
            Draggable = true,
        }.Join(this);
        MainPanel.SetWidth(500f);
        MainPanel.Left.Pixels = 80f;
        MainPanel.Top.Pixels = -10f;
        MainPanel.VAlign = 1f;
        MainPanel.SetPadding(-1f);

        ChatContainer = new View
        {
            Display = Display.Flexbox,
            FlexDirection = FlexDirection.Column,
            Gap = new Vector2(12f),
            CornerRadius = new Vector4(10f, 10f, 0f, 0f),
            BgColor = Color.White * 0.25f,
            OverflowHidden = true,
        }.Join(MainPanel);
        ChatContainer.SetPadding(12f);
        ChatContainer.SetSize(0, 260f, 1f);

        for (var i = 0; i < 2; i++)
        {
            new SUIMessageItem(i == 1).Join(ChatContainer);
        }

        // 分界线
        var br = new View
        {
            BgColor = borderColor * 0.75f,
            ZIndex = 1f,
        }.Join(MainPanel);
        br.SetSize(0, 2.5f, 1f);

        InputContainerInitialize();
    }

    private void InputContainerInitialize()
    {
        var backgroundColor = new Color(63, 65, 151);
        var borderColor = new Color(18, 18, 38);

        InputContainer = new View
        {
            Display = Display.Flexbox,
            FlexDirection = FlexDirection.Row,
            MainAxisAlignment = MainAxisAlignment.SpaceBetween,
            CornerRadius = new Vector4(0f, 0f, 10f, 10f),
            BgColor = Color.White * 0.5f,
        }.Join(MainPanel);
        InputContainer.SetPadding(8f);
        InputContainer.SetSize(0, 50f, 1f);

        var input = new View
        {
            CornerRadius = new Vector4(8f),
            Border = 2,
            BorderColor = borderColor * 0.75f,
            BgColor = backgroundColor * 0.75f,
        }.Join(InputContainer);
        input.SetPadding(-1f);
        input.SetSize(-80f - 8f, 0f, 1f, 1f);

        var send = new SUIText
        {
            CornerRadius = new Vector4(8f),
            Border = 2,
            BorderColor = borderColor * 0.75f,
            BgColor = backgroundColor * 0.75f,
            Text = "发送",
            TextAlign = new Vector2(0.5f),
        }.Join(InputContainer);
        send.SetPadding(-1f);
        send.SetSize(80f, 0f, 0f, 1f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        MainPanel.HoverTimer.Speed = 15f;
        Opacity = MainPanel.HoverTimer.Lerp(0f, 1f);
        
        var offset = MainPanel.HoverTimer.Lerp(20f, 0f);
        MainPanel.TransformMatrix = Matrix.CreateTranslation(0, offset, 0f);
        
        base.Draw(spriteBatch);
    }
}