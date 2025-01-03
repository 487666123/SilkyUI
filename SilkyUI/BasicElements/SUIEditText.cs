using Microsoft.Xna.Framework.Input;
using Terraria.UI.Chat;

namespace SilkyUI.BasicElements;

public class SUIEditText : SUIText
{
    protected float TextCursorTimer;

    public SUIEditText()
    {
        DragIgnore = false;
    }

    protected override void RecalculateText()
    {
        LastText = Text;
        LastMaxWidth = SpecifyWidth ? _innerDimensions.Width : 114514f;

        var origianlText = LastText;

        // 是否换行
        if (WordWrap)
        {
            var maxWidth = LastMaxWidth / TextScale;
            var original =
                TextSnippetHelper.ConvertNormalSnippets(TextSnippetHelper.ParseMessage(origianlText, TextColor));
            original.Add(new PlainSnippet("|"));
            TextSnippetHelper.WordwrapString(original, FinalSnippets, TextColor, Font, maxWidth, MaxWordLength);
        }
        else
        {
            TextSnippetHelper.ConvertNormalSnippets
                (TextSnippetHelper.ParseMessage(origianlText, TextColor), FinalSnippets);
        }

        TextSize = ChatManager.GetStringSize(Font, FinalSnippets.ToArray(), new Vector2(1f));
    }

    protected override void DrawText(SpriteBatch spriteBatch, List<TextSnippet> textSnippets)
    {
        if (textSnippets.Count > 0)
        {
            if (TextCursorTimer >= 45)
                textSnippets.RemoveAt(textSnippets.Count - 1);
            TextCursorTimer++;
            TextCursorTimer %= 90;
        }

        base.DrawText(spriteBatch, textSnippets);
    }

    public override bool OccupyInput => true;

    public override void OnInput(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        if (Keys.D1.JustPressed())
        {
            OnEnterKeyDown?.Invoke();
        }
        else
        {
            Text += input;
        }
    }

    public event Action OnEnterKeyDown;
}