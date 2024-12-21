namespace SilkyUI;

public class SilkyUserInterfaceSystem
{
    private static readonly Lazy<SilkyUserInterfaceSystem> LazyInstance = new(() => new SilkyUserInterfaceSystem());
    public static SilkyUserInterfaceSystem Instance => LazyInstance.Value;

    private SilkyUserInterfaceSystem()
    {
    }

    public readonly SilkyUserInterfaceConfiguration Configuration = new();

    // public readonly RenderTargetPool RenderTargetPool = new();

    private bool _initialized;

    /// <summary>
    /// 初始化系统
    /// </summary>
    public void Initialize()
    {
        if (_initialized) return;

        var assemblies = ModLoader.Mods.Select(mod => mod.Code);
        var basicBodyType = typeof(BasicBody);

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types.Where(type => type.IsSubclassOf(basicBodyType)))
            {
                if (type.GetCustomAttribute<AutoloadUserInterfaceAttribute>() is not
                    { } autoloadUserInterface) continue;

                SilkyUserInterfaceManager.Instance.RegisterUserInterface(autoloadUserInterface, type);
            }
        }

        _initialized = true;
    }
}