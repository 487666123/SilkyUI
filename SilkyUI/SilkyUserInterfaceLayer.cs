namespace SilkyUI;

public class SilkyUserInterfaceLayer : GameInterfaceLayer
{
    public SilkyUserInterface UserInterface { get; }

    public event EventHandler PreDraw;
    public event EventHandler PostDraw;

    public SilkyUserInterfaceLayer(SilkyUserInterface userInterface, string name,
        InterfaceScaleType scaleType, EventHandler onPreDraw = null, EventHandler onPostDraw = null) : base(name,
        scaleType)
    {
        UserInterface = userInterface;
        PreDraw += onPreDraw;
        PostDraw += onPostDraw;
    }

    private void OnPreDraw() => PreDraw?.Invoke(this, EventArgs.Empty);
    private void OnPostDraw() => PostDraw?.Invoke(this, EventArgs.Empty);

    public override bool DrawSelf()
    {
        var transformMatrix = ScaleType switch
        {
            InterfaceScaleType.Game => Main.GameViewMatrix.ZoomMatrix,
            InterfaceScaleType.UI => Main.UIScaleMatrix,
            InterfaceScaleType.None or _ => Matrix.Identity,
        };

        UserInterface.TransformMatrix = transformMatrix;

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred,
            null, null, null, null, null, transformMatrix);

        OnPreDraw();
        var result = UserInterface.Draw();
        OnPostDraw();
        return result;
    }
}