namespace SilkyUI;

public struct ElementBounds(float left, float top, float width, float height)
{
    public static readonly ElementBounds Default = new(0, 0, 0, 0);

    public float Left { get; set; } = left;
    public float Top { get; set; } = top;
    public float Width { get; set; } = width;
    public float Height { get; set; } = height;

    public ElementBounds(Vector2 position, Vector2 size) : this(position.X, position.Y, size.X, size.Y)
    {
    }

    public void Reset() => Left = Top = Width = Height = 0;

    public CalculatedStyle CalculatedStyle()
    {
        return new CalculatedStyle(Left, Top, Width, Height);
    }
}