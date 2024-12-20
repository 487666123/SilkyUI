using SilkyUI.Animation;

namespace SilkyUI.BasicElements;

public enum BoxSizing
{
    BorderBox,
    ContentBox
}

public enum Display
{
    InlineBlock,
    InlineFlex,
    InlineGrid
}

public enum Position
{
    Relative,
    Absolute
}

public partial class View : UIElement
{
    public Vector4 Rounded
    {
        get => RoundedRectangle.Rounded;
        set => RoundedRectangle.Rounded = value;
    }

    public float Border
    {
        get => RoundedRectangle.Border;
        set => RoundedRectangle.Border = value;
    }

    public Color BgColor
    {
        get => RoundedRectangle.BgColor;
        set => RoundedRectangle.BgColor = value;
    }

    public Color BorderColor
    {
        get => RoundedRectangle.BorderColor;
        set => RoundedRectangle.BorderColor = value;
    }

    /// <summary>
    /// 是相对定位
    /// </summary>
    public bool IsRelative => Position is Position.Relative;

    /// <summary>
    /// 是绝对定位
    /// </summary>
    public bool IsAbsolute => Position is Position.Absolute;

    public Display Display { get; set; } = Display.InlineFlex;

    public AnimationTimer HoverTimer { get; } = new();

    /// <summary>
    /// 其他一切绘制都结束之后再绘制边框
    /// </summary>
    public bool FinallyDrawBorder { get; set; } = false;

    /// <summary>
    /// 隐藏完全溢出元素
    /// </summary>
    public bool HideFullyOverflowedElements { get; set; }

    /// <summary>
    /// 决定盒子模型计算方式
    /// </summary>
    public BoxSizing BoxSizing { get; set; } = BoxSizing.BorderBox;

    /// <summary>
    /// 元素定位<br/>
    /// 所有非 <see cref="View"/> 及其子元素的 <see cref="UIElement"/> 都为 <see cref="Position.Absolute"/><br/>
    /// 所以并不建议使用原版的任何元素<br/>
    /// 如果现有元素不能满足你的需求，可以向此项目 issue 或 Pr
    /// </summary>
    public Position Position { get; set; } = Position.Relative;

    /// <summary>
    /// 拖动忽略，默认为 <see langword="false"/> 不会影响长辈中可拖动元素拖动
    /// </summary>
    public bool DragIgnore { get; set; }

    public bool LeftMousePressed { get; set; }
    public bool RightMousePressed { get; set; }
    public bool MiddleMousePressed { get; set; }

    /// <summary>
    /// 指定宽, 否则会被子元素撑起
    /// </summary>
    public bool SpecifyWidth { get; set; }

    /// <summary>
    /// 指定高, 否则会被子元素撑起
    /// </summary>
    public bool SpecifyHeight { get; set; }

    /// <summary>
    /// 文档流元素
    /// </summary>
    protected List<View> FlowElements { get; } = [];

    /// <summary>
    /// 非文档流元素
    /// </summary>
    protected List<UIElement> AbsoluteElements { get; } = [];
}