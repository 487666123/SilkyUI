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

    public override string GetOrigianlText()
    {
        return base.GetOrigianlText() + "[c/ffffff:|]";
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