namespace SilkyUI;

public class UIBounds
{
    public float Left { get; set; }
    public float Top { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }

    public UIBounds(float left, float top, float width, float height)
    {
        Left = left;
        Top = top;
        Width = width;
        Height = height;
    }

    public UIBounds(Vector2 position, Vector2 size)
    {
        Left = position.X;
        Top = position.Y;
        Width = size.X;
        Height = size.Y;
    }

    public CalculatedStyle ToCalculatedStyle()
    {
        return new CalculatedStyle(Left, Top, Width, Height);
    }
}