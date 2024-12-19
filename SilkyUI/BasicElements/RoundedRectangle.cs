namespace SilkyUI.BasicElements;

public class RoundedRectangle
{
    public Vector4 Rounded = new(0f);
    public float Border = 0f;
    public Color BgColor = Color.Transparent;
    public Color BorderColor = Color.Transparent;

    /// <summary>
    /// 绘制 SDF 圆角矩形
    /// </summary>
    public void Draw(Vector2 position, Vector2 size, bool noBorder, Matrix matrix)
    {
        if (Border > 0)
        {
            if (BorderColor == Color.Transparent || noBorder)
            {
                if (BgColor != Color.Transparent)
                    SDFRectangle.NoBorder(position + new Vector2(Border), size - new Vector2(Border * 2f),
                        Rounded - new Vector4(Border), BgColor, matrix);
            }
            else
            {
                SDFRectangle.HasBorder(position, size, Rounded, BgColor, Border, BorderColor, matrix);
            }
        }
        else if (BgColor != Color.Transparent)
        {
            SDFRectangle.NoBorder(position, size, Rounded, BgColor, matrix);
        }
    }

    public void DrawBorder(Vector2 position, Vector2 size, Matrix matrix)
    {
        SDFRectangle.HasBorder(position, size, Rounded,
            Color.Transparent, Border, BorderColor, matrix);
    }
}