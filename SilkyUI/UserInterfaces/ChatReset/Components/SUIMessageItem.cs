namespace SilkyUI.UserInterfaces.ChatReset.Components;

public class SUIMessageItem : View
{
    public SUIText Name { get; private set; }
    public SUIText Message { get; private set; }

    public SUIMessageItem(bool isMe)
    {
        SpecifyWidth = true;
        Width.Percent = 1f;
        Display = Display.Flexbox;
        FlexDirection = FlexDirection.Row;
        MainAxisAlignment = isMe ? MainAxisAlignment.End : MainAxisAlignment.Start;

        var view = new View
        {
            Display = Display.Flexbox,
            FlexDirection = FlexDirection.Column,
            Gap = new Vector2(4f),
            Border = 2f,
            BorderColor = Color.Black * 0.5f,
            BgColor = Color.Black * 0.25f,
            CornerRadius = new Vector4(8f),
            Width = { Percent = 1f },
        }.Join(this);
        view.SetWidth(0f, 0.75f);
        view.SetPadding(8f);

        Name = new SUIText
        {
            TextColor = Color.Yellow,
            WordWrap = true,
            TextAlign = isMe ? Vector2.One : Vector2.Zero,
            TextScale = 0.8f,
        }.Join(view);
        Name.Text = isMe ? "热爱玩****庄" : "系统消息";
        Name.SetWidth(0f, 1f);

        Message = new SUIText
        {
            WordWrap = true,
            TextScale = 0.9f,
        }.Join(view);
        Message.Text =
            isMe ? "三百颗够吗？哈哎~ 应该够吧，来吧！试一下你，这个 BOSS 用这个枪来打从来没试过" : "骷髅王已苏醒";
        Message.SetWidth(0f, 1f);
    }
}