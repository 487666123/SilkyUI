namespace SilkyUI.BasicElements;

public partial class View
{
    public Vector2 Gap;
    public RoundedRectangle RoundedRectangle { get; } = new();


    protected bool TransformMatrixHasChanges = true;

    private Matrix _transformMatrix = Matrix.CreateScale(1, 1, 1);

    public Matrix TransformMatrix
    {
        get => _transformMatrix;
        set
        {
            if (value == _transformMatrix) return;
            _transformMatrix = value;
            TransformMatrixHasChanges = true;
        }
    }

    public Matrix FinalMatrix { get; set; } = Main.UIScaleMatrix;
}