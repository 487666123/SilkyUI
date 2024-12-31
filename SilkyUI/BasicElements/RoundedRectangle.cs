namespace SilkyUI.BasicElements;

public class RoundedRectangle
{
    public float Border;
    public Color BgColor = Color.Transparent;
    public Color BorderColor = Color.Transparent;
    public Vector4 CornerRadius = Vector4.Zero;

    public void Draw(Vector2 position, Vector2 size, bool noBorder, Matrix matrix)
    {
        if (Border > 0)
        {
            if (BorderColor == Color.Transparent || noBorder)
            {
                if (BgColor != Color.Transparent)
                    SDFRectangle.NoBorder(position + new Vector2(Border), size - new Vector2(Border * 2f),
                        CornerRadius - new Vector4(Border), BgColor, matrix);
            }
            else
            {
                SDFRectangle.HasBorder(position, size, CornerRadius, BgColor, Border, BorderColor, matrix);
            }
        }
        else if (BgColor != Color.Transparent)
        {
            SDFRectangle.NoBorder(position, size, CornerRadius, BgColor, matrix);
        }
    }

    public void DrawOnlyTheBorder(Vector2 position, Vector2 size, Matrix matrix) =>
        SDFRectangle.HasBorder(position, size, CornerRadius, Color.Transparent, Border, BorderColor, matrix);

    public void CopyStyle(RoundedRectangle roundedRectangle)
    {
        CornerRadius = roundedRectangle.CornerRadius;
        Border = roundedRectangle.Border;
        BgColor = roundedRectangle.BgColor;
        BorderColor = roundedRectangle.BorderColor;
    }

    public RoundedRectangle Clone() => MemberwiseClone() as RoundedRectangle;
}