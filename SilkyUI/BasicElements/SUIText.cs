using System.Text.RegularExpressions;
using Terraria.UI.Chat;

namespace SilkyUI.BasicElements;

public partial class SUIText : View
{
    /// <summary>
    /// 原字符串
    /// </summary>
    public string OriginalString => _keyMode ? Language.GetTextValue(_textOrKey) : _textOrKey;

    #region 常规可控属性

    /// <summary>
    /// 使用的字体
    /// </summary>
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

    public void UseDeathText() => Font = FontAssets.DeathText.Value;
    public void UseMouseText() => Font = FontAssets.MouseText.Value;
    public bool IsLarge => Font == FontAssets.DeathText.Value;

    /// <summary>
    /// 可以作为 HJson 的 Key，也可直接作为文本。<br/>
    /// 如果要作为 Key 请设置 <see cref="UseKey"/> = true。
    /// </summary>
    public string TextOrKey
    {
        get => _keyMode ? _textOrKey : Language.GetTextValue(_textOrKey);
        set
        {
            if (_textOrKey == value) return;
            _textOrKey = value;
            RecalculateText();
        }
    }

    protected string _textOrKey = "";

    /// <summary>
    /// 使 TextOrKey 作为 HJson 的 Key 使用
    /// </summary>
    public bool UseKey
    {
        get => _keyMode;
        set
        {
            if (_keyMode == value) return;
            _keyMode = value;
            RecalculateText();
        }
    }

    protected bool _keyMode = false;

    /// <summary>
    /// 文字是否换行
    /// </summary>
    public bool IsWrapped
    {
        get => _isWrapped;
        set
        {
            if (_isWrapped == value) return;
            _isWrapped = value;
            RecalculateText();
        }
    }

    protected bool _isWrapped;

    /// <summary>
    /// 一个单词里最多可以含有的字符数，如果单词字符数超过该值，就会在超过maxWidth时直接就地换行，
    /// 而不是向前寻找第一个空格。根据宽度和字符尺寸适当调整
    /// </summary>
    public int MaxCharacterCount
    {
        get => _maxCharacterCount;
        set
        {
            if (_maxCharacterCount == value) return;
            _maxCharacterCount = value;
            RecalculateText();
        }
    }

    protected int _maxCharacterCount = 19;

    /// <summary>
    /// 最大行数，-1即不作限制。超过该值的行数将不会被显示
    /// </summary>
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

    protected int _maxLines = -1;

    /// <summary>
    /// 文字缩放比例
    /// </summary>
    public float TextScale = 1f;

    /// <summary>
    /// 文字颜色
    /// </summary>
    public Color TextColor = Color.White;

    public float TextBorder = 1.5f;

    /// <summary>
    /// 文字边框颜色
    /// </summary>
    public Color TextBorderColor = Color.Black;

    public Vector2 TextOffset = Vector2.Zero;
    public Vector2 TextOrigin = Vector2.Zero;
    public Vector2 TextAlign = Vector2.Zero;
    public Vector2 TextPercentOffset = Vector2.Zero;

    #endregion

    #region 非常规可控属性

    /// <summary>
    /// 最终展示文本
    /// </summary>
    public TextSnippet[] FinalTextSnippets { get; protected set; }

    /// <summary>
    /// 最后一次的 inner 宽度
    /// </summary>
    public float LastInnerWidth { get; protected set; }

    /// <summary>
    /// 最后一次的文本
    /// </summary>
    public string LastString { get; protected set; }

    /// <summary>
    /// 文字大小
    /// </summary>
    public Vector2 TextSize { get; protected set; } = Vector2.Zero;

    #endregion

    /// <summary>
    /// 修改后会影响 “文本大小” 的属性
    /// <br/>
    /// - keyMode
    /// <br/>
    /// - text
    /// <br/>
    /// - isLarge
    /// <br/>
    /// 当开启 isWrapped 的时候，除了以上属性变动，在宽度限制改变时也需要刷新
    /// </summary>
    public void RecalculateText()
    {
        LastString = OriginalString;

        if (_isWrapped)
        {
            /* 原来的有Bug，不支持英文空格换行，暂时换掉
            List<TextSnippet> finalSnippets = [];
            List<List<TextSnippet>> firstSnippets = TextSnippetHelper.WordwrapString(LastString, TextColor, Font, GetInnerDimensions().Width / TextScale);

            foreach (List<TextSnippet> snippets in firstSnippets)
            {
                finalSnippets.AddRange(snippets);
                finalSnippets.Add(new TextSnippet("\n"));
            }

            if (finalSnippets.Count > 0 && finalSnippets[^1].Text == "\n")
            {
                finalSnippets.RemoveAt(finalSnippets.Count - 1);
            }

            FinalTextSnippets = [.. finalSnippets];
            */
            FinalTextSnippets = TextSnippetHelper.WordwrapString(LastString, TextColor, Font,
                (int)(GetInnerDimensions().Width / TextScale), out _, MaxCharacterCount, MaxLines);
        }
        else
        {
            FinalTextSnippets =
                [.. TextSnippetHelper.ConvertNormalSnippets(TextSnippetHelper.ParseMessage(LastString, TextColor))];
        }

        TextSize = ChatManager.GetStringSize(Font, FinalTextSnippets, new Vector2(1f));
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        var innerSize = GetInnerDimensions().Size();
        var innerPos = GetInnerDimensions().Position();

        // 文本变化时或者宽度变化时, 重新计算
        if (LastString != OriginalString || (_isWrapped && LastInnerWidth != innerSize.X))
        {
            LastInnerWidth = innerSize.X;
            RecalculateText();
        }

        var textSize = TextSize;
        // 无字符时会出问题，加上下面这两行就好了
        if (textSize.Y < Font.LineSpacing)
            textSize.Y = Font.LineSpacing;
        var textPos = innerPos + TextOffset;
        textPos += TextPercentOffset * innerSize;
        textPos += TextAlign * (innerSize - textSize * TextScale);
        textPos -= TextOrigin * TextSize * TextScale;

        SilkyUserInterfaceSystem.Instance.Configuration.FontYAxisOffset.TryGetValue(Font, out var yAxisOffset);
        textPos.Y += TextScale * yAxisOffset;

        DrawColorCodedStringShadow(spriteBatch, Font, FinalTextSnippets,
            textPos, TextBorderColor, 0f, Vector2.Zero, new Vector2(TextScale), -1f, TextBorder * TextScale);

        DrawColorCodedString(spriteBatch, Font, FinalTextSnippets,
            textPos, TextColor, 0f, Vector2.Zero, new Vector2(TextScale), out var _, -1f);
    }

    public Vector2[] ShadowDirections = [-Vector2.UnitX, Vector2.UnitX, -Vector2.UnitY, Vector2.UnitY];

    public void DrawColorCodedStringShadow(SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets,
        Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f,
        float spread = 2f)
    {
        foreach (var offset in ShadowDirections)
        {
            DrawColorCodedString(spriteBatch, font, snippets, position + offset * spread, baseColor,
                rotation, origin, baseScale, out var _, maxWidth, ignoreColors: true);
        }
    }

    public static Vector2 DrawColorCodedString(SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets,
        Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet,
        float maxWidth, bool ignoreColors = false)
    {
        var num = -1;

        var mousePosition = Main.MouseScreen;
        var vector = position;
        var result = vector;

        var x = font.MeasureString(" ").X;
        var color = baseColor;
        var num3 = 0f;

        for (var i = 0; i < snippets.Length; i++)
        {
            var textSnippet = snippets[i];
            textSnippet.Update();
            if (!ignoreColors)
            {
                color = textSnippet.GetVisibleColor();
            }

            var num2 = textSnippet.Scale;
            if (textSnippet.UniqueDraw(justCheckingString: false, out var size, spriteBatch, vector, color,
                    baseScale.X * num2))
            {
                if (mousePosition.Between(vector, vector + size))
                {
                    num = i;
                }

                vector.X += size.X;
                result.X = Math.Max(result.X, vector.X);
                continue;
            }

            var stringArray = textSnippet.Text.Split('\n');
            var flag = true;
            foreach (var obj in stringArray)
            {
                var array3 = obj.Split(' ');
                if (obj == "\n")
                {
                    vector.Y += font.LineSpacing * num3 * baseScale.Y;
                    vector.X = position.X;
                    result.Y = Math.Max(result.Y, vector.Y);
                    num3 = 0f;
                    flag = false;
                    continue;
                }

                for (var k = 0; k < array3.Length; k++)
                {
                    if (k != 0)
                    {
                        vector.X += x * baseScale.X * num2;
                    }

                    if (maxWidth > 0f)
                    {
                        var num4 = font.MeasureString(array3[k]).X * baseScale.X * num2;
                        if (vector.X - position.X + num4 > maxWidth)
                        {
                            vector.X = position.X;
                            vector.Y += font.LineSpacing * num3 * baseScale.Y;
                            result.Y = Math.Max(result.Y, vector.Y);
                            num3 = 0f;
                        }
                    }

                    if (num3 < num2)
                    {
                        num3 = num2;
                    }

                    spriteBatch.DrawString(font, array3[k], vector, color, rotation, origin,
                        baseScale * textSnippet.Scale * num2, SpriteEffects.None, 0f);
                    var vector2 = font.MeasureString(array3[k]);
                    if (mousePosition.Between(vector, vector + vector2))
                    {
                        num = i;
                    }

                    vector.X += vector2.X * baseScale.X * num2;
                    result.X = Math.Max(result.X, vector.X);
                }

                if (stringArray.Length > 1 && flag)
                {
                    vector.Y += font.LineSpacing * num3 * baseScale.Y;
                    vector.X = position.X;
                    result.Y = Math.Max(result.Y, vector.Y);
                    num3 = 0f;
                }

                flag = true;
            }
        }

        hoveredSnippet = num;
        return result;
    }

    /*/// <summary>
    /// 绘制偏移
    /// </summary>
    public readonly static Vector2[] StringOffsets = [new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0)];

    /// <summary>
    /// 绘制带有边框的文字
    /// </summary>
    public static void DrawBorderString(SpriteBatch spriteBatch, DynamicSpriteFont font, Vector2 originalPosition,
        string text, Color textColor, Color borderColor, Vector2 origin, float textScale, float border = 2f)
    {
        border *= textScale;

        Vector2 position;
        Color color = borderColor;

        if (borderColor.Equals(Color.Transparent))
        {
            spriteBatch.DrawString(font, text, originalPosition, textColor, 0f, origin, textScale, 0, 0f);
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                position = originalPosition + StringOffsets[i] * border;

                // 最上层
                if (i is 4)
                {
                    color = textColor;
                }

                spriteBatch.DrawString(font, text, position, color, 0f, origin, textScale, 0, 0f);
            }
        }
    }*/
}