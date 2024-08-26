namespace SilkyUI;

public class SilkyUserInterfaceManager
{
    private static readonly Lazy<SilkyUserInterfaceManager> LazyInstance =
        new(() => new SilkyUserInterfaceManager());

    /// <summary>
    /// 线程安全的单例模式
    /// </summary>
    public static SilkyUserInterfaceManager Instance => LazyInstance.Value;

    private SilkyUserInterfaceManager() { }

    /// <summary>
    /// 鼠标位置 [<see cref="Main.mouseX"/>, <see cref="Main.mouseY"/>]
    /// </summary>
    public static Vector2 MouseScreen => new(Main.mouseX, Main.mouseY);

    #region Fields and Propertices

    /// <summary>
    /// 当前的 <see cref="UserInterface"/>
    /// </summary>
    private SilkyUserInterface CurrentUserInterface { get; set; }

    /// <summary>
    /// 当前插入点, 用于 <see cref="UpdateUI(GameTime)"/>
    /// </summary>
    public string CurrentInsertionPoint { get; protected set; } = string.Empty;

    /// <summary>
    /// 当前的 <see cref="UserInterface"/> 所在 List
    /// </summary>
    public List<SilkyUserInterface> CurrentUserInterfaces { get; protected set; }

    /// <summary>
    /// 鼠标焦点元素
    /// </summary>
    public UIElement MouseFocusUIElement { get; set; } = null;

    /// <summary>
    /// 当前鼠标焦点下是否有元素
    /// </summary>
    public bool MouseFocusHasUIElement => MouseFocusUIElement is not null;

    /// <summary>
    /// <see cref="UserInterface"/> 实例绑定的 <see cref="BasicBody"/> <see cref="Type"/>
    /// </summary>
    public Dictionary<SilkyUserInterface, Type> BasicBodyTypes { get; } = [];

    /// <summary>
    /// <see cref="UserInterface"/> 实例绑定的 <see cref="BasicBody"/> <see cref="AutoloadUserInterfaceAttribute"/>
    /// </summary>
    public Dictionary<SilkyUserInterface, AutoloadUserInterfaceAttribute> BasicBodyTypesAutoloadInfo { get; } = [];

    /// <summary>
    /// 界面层顺序
    /// </summary>
    private readonly List<string> InterfaceLayerOrders = [];

    /// <summary>
    /// <see cref="string"/> 插入点<br/>
    /// <see cref="List{T}"/> 插入 UI <see cref="SilkyUserInterface"/>
    /// </summary>
    public readonly Dictionary<string, List<SilkyUserInterface>> SilkyUserInterfaces = [];

    #endregion

    public SilkyUserInterface RegisterUserInterface(
        AutoloadUserInterfaceAttribute autoloadAttribute, Type basicBodyType)
    {
        var userInterface = new SilkyUserInterface();

        BasicBodyTypes[userInterface] = basicBodyType;
        BasicBodyTypesAutoloadInfo[userInterface] = autoloadAttribute;

        if (SilkyUserInterfaces.TryGetValue(autoloadAttribute.InsertionPoint,
            out List<SilkyUserInterface> userInterfaces))
            userInterfaces.Add(userInterface);
        else
            SilkyUserInterfaces[autoloadAttribute.InsertionPoint] = [userInterface];

        return userInterface;
    }

    /// <summary>
    /// 移动当前 UserInterface 到顶层
    /// </summary>
    /// <param name="userInterface"></param>
    public void MoveCurrentUserIntrerfaceToTop()
    {
        if (CurrentUserInterfaces.Remove(CurrentUserInterface))
        {
            CurrentUserInterfaces.Insert(0, CurrentUserInterface);
        }
    }

    /// <summary>
    /// 更新 UI
    /// </summary>
    public void UpdateUI(GameTime gameTime)
    {
        MouseFocusUIElement = null;

        try
        {
            // 绘制顺序, 所以事件处理要倒叙
            foreach (string insertionPoint in InterfaceLayerOrders
                .Where(SilkyUserInterfaces.ContainsKey).Reverse())
            {
                var userInterfaces = SilkyUserInterfaces[insertionPoint];
                CurrentUserInterfaces = userInterfaces;

                var order = CurrentUserInterfaces.OrderBy(value =>
                    BasicBodyTypesAutoloadInfo[value].DefaultPriority).ToList();
                CurrentUserInterfaces.Clear();
                CurrentUserInterfaces.AddRange(order);

                for (int i = 0; i < userInterfaces.Count; i++)
                {
                    var userInterface = userInterfaces[i];
                    CurrentInsertionPoint = insertionPoint;
                    CurrentUserInterface = userInterface;
                    CurrentUserInterface?.Update(gameTime);
                }
            }
        }
        finally
        {
            CurrentUserInterface = null;
            CurrentInsertionPoint = null;
        }
    }

    /// <summary>
    /// 修改界面层级
    /// </summary>
    public void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int count = InterfaceLayerOrders.Count;

        #region InterfaceLayer.Name 顺序
        InterfaceLayerOrders.Clear();

        for (int i = 0; i < layers.Count; i++)
        {
            var layer = layers[i];
            if (!InterfaceLayerOrders.Contains(layer.Name))
                InterfaceLayerOrders.Add(layer.Name);
        }
        #endregion

        #region 向 InterfaceLayer 插入 UI
        if (count < 1) return;
        foreach (var (insertionPoint, userInterfaces) in SilkyUserInterfaces)
        {
            var index = layers.FindIndex(layer => layer.Name.Equals(insertionPoint));

            if (index > -1)
            {
                foreach (var userInterface in userInterfaces)
                {
                    string name = "UNKNOWN";
                    if (BasicBodyTypesAutoloadInfo.TryGetValue(userInterface, out var autoload))
                    {
                        name = autoload.Name;
                    }

                    CurrentInsertionPoint = insertionPoint;
                    CurrentUserInterface = userInterface;
                    var userInterfaceLayer =
                        new SilkyUserInterfaceLayer(userInterface, name, InterfaceScaleType.UI,
                        () =>
                        {
                            CurrentUserInterface = userInterface;
                            CurrentInsertionPoint = insertionPoint;
                        }, () =>
                        {
                            CurrentUserInterface = null;
                            CurrentInsertionPoint = null;
                        });

                    layers.Insert(index + 1, userInterfaceLayer);
                }
            }
        }
        #endregion
    }
}