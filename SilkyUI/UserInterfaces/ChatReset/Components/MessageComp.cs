namespace SilkyUI.UserInterfaces.ChatReset.Components;

public class MessageComp : View
{
    public SUIText SenderText { get; private set; }
    public SUIText MessageText { get; private set; }

    public MessageComp(string sender, string message, bool isMyMessage = false)
    {
        SpecifyWidth = true;
        Width.Percent = 1f;
        Display = Display.Flexbox;
        FlexDirection = FlexDirection.Row;
        MainAxisAlignment = isMyMessage ? MainAxisAlignment.End : MainAxisAlignment.Start;

        var card = new View
        {
            Display = Display.Flexbox,
            FlexDirection = FlexDirection.Column,
            Gap = new Vector2(4f),
            Border = 2f,
            BorderColor = Color.Black * 0.5f,
            BgColor = Color.Black * 0.2f,
            CornerRadius = new Vector4(8f),
        }.Join(this);
        card.SetWidth(0f, 0.75f);
        card.SetPadding(8f);

        var header = new View
        {
            Display = Display.Flexbox,
            FlexDirection = FlexDirection.Row,
            MainAxisAlignment = MainAxisAlignment.SpaceBetween,
        }.Join(card);
        header.SetWidth(0f, 1f);

        if (isMyMessage)
            new SUIText
            {
                TextScale = 0.7f,
                TextColor = Color.Pink,
                Text = $"{DateTime.Now}",
                TextAlign = Vector2.Zero,
            }.Join(header);

        SenderText = new SUIText
        {
            TextScale = 0.7f,
            TextColor = Color.Yellow,
            Text = $"{sender}",
            TextAlign = isMyMessage ? Vector2.One : Vector2.Zero,
        }.Join(header);

        if (!isMyMessage)
            new SUIText
            {
                TextScale = 0.7f,
                TextColor = Color.Pink,
                Text = $"{DateTime.Now}",
                TextAlign = Vector2.Zero,
            }.Join(header);

        MessageText = new SUIText
        {
            WordWrap = true,
            TextScale = 0.8f,
            Text = message,
        }.Join(card);
        MessageText.SetWidth(0f, 1f);
    }
}