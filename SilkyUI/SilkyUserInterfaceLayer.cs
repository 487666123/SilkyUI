namespace SilkyUI;

public class SilkyUserInterfaceLayer(SilkyUserInterface userInterface, string name,
    InterfaceScaleType scaleType,
    Action preDraw = null, Action postDraw = null)
    : GameInterfaceLayer(name, scaleType)
{
    public SilkyUserInterface UserInterface => userInterface;

    public override bool DrawSelf()
    {
        var transformMatrix = ScaleType switch
        {
            InterfaceScaleType.Game => Main.GameViewMatrix.ZoomMatrix,
            InterfaceScaleType.UI => Main.UIScaleMatrix,
            InterfaceScaleType.None or _ => Matrix.Identity,
        };

        userInterface.TransformMatrix = transformMatrix;

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred,
            null, null, null, null, null, transformMatrix);

        preDraw?.Invoke();
        var result = userInterface.Draw();
        postDraw?.Invoke();
        return result;
    }
}