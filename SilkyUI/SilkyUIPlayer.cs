namespace SilkyUI;

public class SilkyUIPlayer : ModPlayer
{
    public override void OnEnterWorld()
    {
        CreateBasicBodyInstancesToSetupUI();
    }

    /// <summary>
    /// 创建所有 UI 的 BasicBody 实例
    /// </summary>
    private static void CreateBasicBodyInstancesToSetupUI()
    {
        var manager = SilkyUserInterfaceManager.Instance;

        foreach (var ui in manager.SilkyUserInterfaces.SelectMany(uis => uis.Value))
        {
            if (manager.BasicBodyTypes.TryGetValue(ui, out var type))
                ui.SetBasicBody(CreateBasicBodyInstanceByType(type));
        }
    }

    // 创建 BasicBody 实例
    private static BasicBody CreateBasicBodyInstanceByType(Type type)
    {
        try
        {
            return Activator.CreateInstance(type) as BasicBody;
        }
        catch (Exception)
        {
            return null;
        }
    }
}