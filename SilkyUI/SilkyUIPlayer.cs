namespace SilkyUI;

public class SilkyUIPlayer : ModPlayer
{
    public override void OnEnterWorld()
    {
        var uiManager = SilkyUserInterfaceManager.Instance;

        // Dictionary => List<SilkyUserInterface>
        foreach (var userInterface in uiManager.SilkyUserInterfaces.SelectMany(userInterfaces => userInterfaces.Value))
        {
            if (!uiManager.BasicBodyTypes.TryGetValue(userInterface, out var type)) continue;

            if (userInterface != null && Activator.CreateInstance(type) is BasicBody basicBody)
                userInterface.SetBasicBody(basicBody);
            else
                userInterface.SetBasicBody(null);
        }
    }
}