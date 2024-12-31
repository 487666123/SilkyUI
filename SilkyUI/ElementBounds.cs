namespace SilkyUI;

public class ElementBounds(float left, float top, float width, float height)
{
    public float Left { get; set; } = left;
    public float Top { get; set; } = top;
    public float Width { get; set; } = width;
    public float Height { get; set; } = height;

    public ElementBounds(Vector2 position, Vector2 size) : this(position.X, position.Y, size.X, size.Y)
    {
    }

    public CalculatedStyle ToCalculatedStyle()
    {
        return new CalculatedStyle(Left, Top, Width, Height);
    }
}