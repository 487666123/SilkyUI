using AutoloadAttribute = SilkyUI.Attributes.AutoloadAttribute;

namespace SilkyUI;

// ReSharper disable once ClassNeverInstantiated.Global
public class SilkyUISystem : ModSystem
{
    public static SilkyUISystem Instance => ModContent.GetInstance<SilkyUISystem>();

    public readonly SilkyUserInterfaceConfiguration Configuration = new();

    public override void PostSetupContent()
    {
        InitializeSystem();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        SilkyUserInterfaceManager.Instance.UpdateUI(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        SilkyUserInterfaceManager.Instance.ModifyInterfaceLayers(layers);
    }

    private bool _initialized;

    /// <summary>
    /// 加载所有 Mod 中的程序集, 查找所有程序集中的所有类<br/>
    /// 将带有 <see cref="Attributes.AutoloadAttribute"/> 注解的 <see cref="BasicBody"/> 类筛选出来
    /// </summary>
    private void InitializeSystem()
    {
        if (_initialized) return;

        var assemblies = ModLoader.Mods.Select(mod => mod.Code);
        var basicBodyType = typeof(BasicBody);

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types.Where(type => type.IsSubclassOf(basicBodyType)))
            {
                if (!IsAutoloadType(type, out var attribute)) continue;
                SilkyUserInterfaceManager.Instance.RegisterUI(type, attribute);
            }
        }

        _initialized = true;
    }

    private static bool IsAutoloadType(Type type, out AutoloadAttribute autoloadAttribute)
    {
        autoloadAttribute = type.GetCustomAttribute<AutoloadAttribute>();
        return autoloadAttribute is not null;
    }
}

public class SilkyUIPlayer : ModPlayer
{
    public override void OnEnterWorld()
    {
        SetupUI();
    }

    private static void SetupUI()
    {
        var manager = SilkyUserInterfaceManager.Instance;

        foreach (var ui in manager.SilkyUserInterfaces.SelectMany(uis => uis.Value))
        {
            if (!manager.BasicBodyTypes.TryGetValue(ui, out var type)) continue;
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