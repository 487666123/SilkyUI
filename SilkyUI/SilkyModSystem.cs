namespace SilkyUI;

public class SilkyModSystem : ModSystem
{
    public override void PostSetupContent()
    {
        SilkyUserInterfaceSystem.Instance.Initialize();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        SilkyUserInterfaceManager.Instance.UpdateUI(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        SilkyUserInterfaceManager.Instance.ModifyInterfaceLayers(layers);
    }
}