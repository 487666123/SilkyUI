namespace SilkyUI;

public class SilkyPlayer : ModPlayer
{
    public override void OnEnterWorld()
    {
        SilkyUserInterfaceManager uiManager = SilkyUserInterfaceManager.Instance;

        foreach (var userInterfaces in uiManager.SilkyUserInterfaces)
        {
            foreach (var userInterface in userInterfaces.Value)
            {
                if (uiManager.BasicBodyTypes.TryGetValue(userInterface, out var type))
                {
                    if (userInterface != null && Activator.CreateInstance(type) is BasicBody basicBody)
                        userInterface.SetBasicBody(basicBody);
                    else
                        userInterface.SetBasicBody(null);
                }
            }
        }
    }
}