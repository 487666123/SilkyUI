namespace SilkyUI;

public class SilkyUserInterfaceSystem
{
    private static readonly Lazy<SilkyUserInterfaceSystem> LazyInstance = new(() => new SilkyUserInterfaceSystem());
    public static SilkyUserInterfaceSystem Instance => LazyInstance.Value;

    private SilkyUserInterfaceSystem()
    {
    }

    public readonly SilkyUserInterfaceConfiguration UserInterfaceConfiguration = new();

    // public readonly RenderTargetPool RenderTargetPool = new();

    private bool _initialized = false;

    /// <summary>
    /// 初始化系统
    /// </summary>
    public void Initialize()
    {
        if (_initialized) return;

        var types = Assembly.GetExecutingAssembly().GetTypes();
        var basicBodyType = typeof(BasicBody);

        // 遍历所有继承自 BasicBody 的类型 (不包含 BasicBody 本身)
        foreach (var type in types.Where(type => type.IsSubclassOf(basicBodyType)))
        {
            if (type.GetCustomAttribute<AutoloadUserInterfaceAttribute>() is not
                { } autoloadUserInterface) continue;

            SilkyUserInterfaceManager.Instance.RegisterUserInterface(autoloadUserInterface, type);
        }

        _initialized = true;
    }
}