namespace SilkyUI.BasicElements;

public partial class View
{
    public RoundedRectangle RoundedRectangle { get; } = new();

    public Matrix TransformMatrix = Matrix.Identity;

    public Vector2 Gap;
}