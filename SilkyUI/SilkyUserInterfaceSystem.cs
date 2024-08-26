namespace SilkyUI;

/// <summary>
/// 单例模式
/// </summary>
public class SilkyUserInterfaceSystem
{
    private static readonly Lazy<SilkyUserInterfaceSystem> LazyInstance = new(() => new SilkyUserInterfaceSystem());
    public static SilkyUserInterfaceSystem Instance => LazyInstance.Value;

    private SilkyUserInterfaceSystem()
    {
    }

    public UserInterfaceStyleManager UserInterfaceStyleManager = new();

    public readonly SilkyUserInterfaceConfiguration UserInterfaceConfiguration = new();

    // public readonly RenderTargetPool RenderTargetPool = new();

    private bool _initialized = false;

    public enum InitializeResult
    {
        Success,
        Error,
        Initialized
    }

    /// <summary>
    /// 初始化系统
    /// </summary>
    public InitializeResult Initialize()
    {
        // 确保只执行一次
        if (_initialized)
            return InitializeResult.Initialized;

        // 获取程序集所有的类型
        var types = Assembly.GetExecutingAssembly().GetTypes();

        // 扫描
        foreach (var type in types.Where(type => type.IsSubclassOf(typeof(BasicBody))))
        {
            var autoload = type.GetCustomAttribute<AutoloadUserInterfaceAttribute>();
            if (autoload is null) continue;

            SilkyUserInterfaceManager.Instance.RegisterUserInterface(autoload, type);
        }

        _initialized = true;
        return InitializeResult.Success;
    }
}