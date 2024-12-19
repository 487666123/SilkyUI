namespace SilkyUI;

public class SilkyUIPlayer : ModPlayer
{
    public override void OnEnterWorld()
    {
        CreateBasicBodyInstancesToSetupUI();
    }

    private static void CreateBasicBodyInstancesToSetupUI()
    {
        var manager = SilkyUserInterfaceManager.Instance;

        foreach (var ui in manager.SilkyUserInterfaces.SelectMany(uis => uis.Value))
        {
            if (manager.BasicBodyTypes.TryGetValue(ui, out var type))
                ui.SetBasicBody(CreateBasicBodyInstanceByType(type));
        }
    }

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