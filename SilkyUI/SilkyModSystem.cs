namespace SilkyUI;

public class SilkyModSystem : ModSystem
{
    // 初始化系统
    public override void PostSetupContent()
    {
        SilkyUserInterfaceSystem.Instance.Initialize();
    }

    // Update UI
    public override void UpdateUI(GameTime gameTime)
    {
        SilkyUserInterfaceManager.Instance.UpdateUI(gameTime);
    }

    // Draw UI
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        SilkyUserInterfaceManager.Instance.ModifyInterfaceLayers(layers);
    }
}