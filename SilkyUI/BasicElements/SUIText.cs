using System.Text.RegularExpressions;
using Terraria.UI.Chat;

namespace SilkyUI.BasicElements;

public class SUIText : View
{
    public void UseDeathText() => Font = FontAssets.DeathText.Value;
    public void UseMouseText() => Font = FontAssets.MouseText.Value;
    public bool IsLarge => Font == FontAssets.DeathText.Value;

    #region 控制属性

    public virtual DynamicSpriteFont Font
    {
        get => _font ?? FontAssets.MouseText.Value;
        set
        {
            if (_font != value) return;
            _font = value;
            RecalculateText();
        }
    }

    private DynamicSpriteFont _font = FontAssets.MouseText.Value;

    private string _text = string.Empty;
    private bool _wordWrap;
    private int _maxWordLength = 19;
    private int _maxLines = -1;

    public string Text
    {
        get => _text;
        set
        {
            if (_text == value) return;
            _text = value;
            RecalculateText();
        }
    }

    public bool WordWrap
    {
        get => _wordWrap;
        set
        {
            if (_wordWrap == value) return;
            _wordWrap = value;
            RecalculateText();
        }
    }

    public int MaxWordLength
    {
        get => _maxWordLength;
        set
        {
            if (_maxWordLength == value) return;
            _maxWordLength = value;
            RecalculateText();
        }
    }

    public int MaxLines
    {
        get => _maxLines;
        set
        {
            if (_maxLines == value) return;
            _maxLines = value;
            RecalculateText();
        }
    }

    public float TextScale { get; set; } = 1f;

    public Color TextColor { get; set; } = Color.White;

    public float TextBorder { get; set; } = 2f;

    public Color TextBorderColor { get; set; } = Color.Black;

    public Vector2 TextOffset { get; set; } = Vector2.Zero;
    public Vector2 TextPercentOffset { get; set; } = Vector2.Zero;
    public Vector2 TextPercentOrigin { get; set; } = Vector2.Zero;
    public Vector2 TextAlign { get; set; } = Vector2.Zero;

    #endregion

    protected readonly List<TextSnippet> FinalSnippets = [];
    protected string LastText;
    protected float LastMaxWidth;

    public Vector2 TextSize { get; protected set; } = Vector2.Zero;

    protected override Vector2 CalculateOuterSize(float width, float height)
    {
        RecalculateText(width <= 0 ? float.MaxValue : width);

        if (!SpecifyWidth) width = TextSize.X;
        if (!SpecifyHeight) height = TextSize.Y * TextScale;

        return base.CalculateOuterSize(width, height);
    }

    protected virtual void RecalculateText(float? width = null)
    {
        LastText = Text;
        LastMaxWidth = width ?? _innerDimensions.Width;

        // 是否换行
        if (_wordWrap)
        {
            var maxWidth = _innerDimensions.Width / TextScale;
            TextSnippetHelper.WordwrapString
                (FinalSnippets, LastText, TextColor, Font, maxWidth, MaxWordLength, MaxLines);
        }
        else
        {
            TextSnippetHelper.ConvertNormalSnippets
                (TextSnippetHelper.ParseMessage(LastText, TextColor), FinalSnippets);
        }

        TextSize = ChatManager.GetStringSize(Font, FinalSnippets.ToArray(), new Vector2(1f));
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (LastText != Text || (_wordWrap && LastMaxWidth != _innerDimensions.Width)) RecalculateText();

        var innerSize = _innerDimensions.Size();
        var innerPos = _innerDimensions.Position();

        var textSize = TextSize;
        // 无字符时会出问题，加上下面这行就好了
        textSize.Y = Math.Max(Font.LineSpacing, textSize.Y);

        SilkyUISystem.Instance.Configuration.FontYAxisOffset.TryGetValue(Font, out var yAxisOffset);

        var textPos =
            innerPos
            + TextOffset
            + TextPercentOffset * innerSize
            + TextAlign * (innerSize - textSize * TextScale)
            - TextPercentOrigin * TextSize * TextScale;
        textPos.Y += TextScale * yAxisOffset;

        var textSnippets = FinalSnippets.ToArray();
        DrawColorCodedStringShadow(spriteBatch, Font, textSnippets,
            textPos, TextBorderColor, 0f, Vector2.Zero, new Vector2(TextScale));
        DrawColorCodedString(spriteBatch, Font, textSnippets,
            textPos, TextColor, 0f, Vector2.Zero, new Vector2(TextScale), out _, -1f);
    }

    protected static readonly Vector2[] ShadowOffsets = [-Vector2.UnitX, Vector2.UnitX, -Vector2.UnitY, Vector2.UnitY];

    protected static void DrawColorCodedStringShadow(SpriteBatch spriteBatch, DynamicSpriteFont font,
        TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin,
        Vector2 baseScale, float maxWidth = -1f,
        float spread = 2f)
    {
        foreach (var offset in ShadowOffsets)
            DrawColorCodedString(spriteBatch, font, snippets, position + offset * spread, baseColor,
                rotation, origin, baseScale, out var _, maxWidth, ignoreColors: true);
    }

    protected static Vector2 DrawColorCodedString(SpriteBatch spriteBatch, DynamicSpriteFont font,
        TextSnippet[] snippets,
        Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet,
        float maxWidth, bool ignoreColors = false)
    {
        var num1 = -1;
        var vec = new Vector2((float)Main.mouseX, (float)Main.mouseY);
        var vector2_1 = position;
        var vector2_2 = vector2_1;
        var x = font.MeasureString(" ").X;
        var color = baseColor;
        var num2 = 0.0f;
        for (var index1 = 0; index1 < snippets.Length; ++index1)
        {
            var snippet = snippets[index1];
            snippet.Update();
            if (!ignoreColors)
                color = snippet.GetVisibleColor();
            var scale = snippet.Scale;
            if (snippet.UniqueDraw(false, out var size, spriteBatch, vector2_1, color, baseScale.X * scale))
            {
                if (vec.Between(vector2_1, vector2_1 + size))
                    num1 = index1;
                vector2_1.X += size.X;
                vector2_2.X = Math.Max(vector2_2.X, vector2_1.X);
            }
            else
            {
                snippet.Text.Split('\n');
                string[] strArray1 = Regex.Split(snippet.Text, "(\n)");
                bool flag = true;
                foreach (string input in strArray1)
                {
                    Regex.Split(input, "( )");
                    string[] strArray2 = input.Split(' ');
                    if (input == "\n")
                    {
                        vector2_1.Y += (float)font.LineSpacing * num2 * baseScale.Y;
                        vector2_1.X = position.X;
                        vector2_2.Y = Math.Max(vector2_2.Y, vector2_1.Y);
                        num2 = 0.0f;
                        flag = false;
                    }
                    else
                    {
                        for (int index2 = 0; index2 < strArray2.Length; ++index2)
                        {
                            if (index2 != 0)
                                vector2_1.X += x * baseScale.X * scale;
                            if ((double)maxWidth > 0.0)
                            {
                                float num3 = font.MeasureString(strArray2[index2]).X * baseScale.X * scale;
                                if ((double)vector2_1.X - (double)position.X + (double)num3 > (double)maxWidth)
                                {
                                    vector2_1.X = position.X;
                                    vector2_1.Y += (float)font.LineSpacing * num2 * baseScale.Y;
                                    vector2_2.Y = Math.Max(vector2_2.Y, vector2_1.Y);
                                    num2 = 0.0f;
                                }
                            }

                            if ((double)num2 < (double)scale)
                                num2 = scale;
                            spriteBatch.DrawString(font, strArray2[index2], vector2_1, color, rotation, origin,
                                baseScale * snippet.Scale * scale, SpriteEffects.None, 0.0f);
                            Vector2 vector2_3 = font.MeasureString(strArray2[index2]);
                            if (vec.Between(vector2_1, vector2_1 + vector2_3))
                                num1 = index1;
                            vector2_1.X += vector2_3.X * baseScale.X * scale;
                            vector2_2.X = Math.Max(vector2_2.X, vector2_1.X);
                        }

                        if (strArray1.Length > 1 & flag)
                        {
                            vector2_1.Y += (float)font.LineSpacing * num2 * baseScale.Y;
                            vector2_1.X = position.X;
                            vector2_2.Y = Math.Max(vector2_2.Y, vector2_1.Y);
                            num2 = 0.0f;
                        }

                        flag = true;
                    }
                }
            }
        }

        hoveredSnippet = num1;
        return vector2_2;
    }
}