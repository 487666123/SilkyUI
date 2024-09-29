using SilkyUI.Animation;

namespace SilkyUI.BasicElements;

public enum BoxSizing { BorderBox, ContentBox }

public enum Display { InlineFlex, InlineGrid }

public enum Positioning { Relative, Absolute }

public enum FlowDirection { Row, Column }

public partial class View : UIElement
{
    /// <summary>
    /// 横向格子
    /// </summary>
    public List<float> GridTemplateRows { get; } = [];
    public List<float> GridTemplateColumns { get; } = [];

    /// <summary>
    /// 是相对定位
    /// </summary>
    public bool IsRelativePositioning => Positioning is Positioning.Relative;

    /// <summary>
    /// 是绝对定位
    /// </summary>
    public bool IsAbsolutePositioning => Positioning is Positioning.Absolute;

    public FlowDirection FlowDirection { get; set; } = FlowDirection.Row;

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
    /// 所有非 <see cref="View"/> 及其子元素的 <see cref="UIElement"/> 都为 <see cref="Positioning.Absolute"/><br/>
    /// 所以并不建议使用原版的任何元素<br/>
    /// 如果现有元素不能满足你的需求，可以向此项目 issue 或提交你的 Pr
    /// </summary>
    public Positioning Positioning { get; set; } = Positioning.Relative;

    /// <summary>
    /// 拖动忽略，默认为 <see langword="false"/> 不会影响长辈中可拖动元素拖动
    /// </summary>
    public bool DragIgnore { get; set; }

    public bool LeftMouseButtonPressed { get; set; }
    public bool RightMouseButtonPressed { get; set; }

    /// <summary>
    /// 使用指定的宽度
    /// </summary>
    public bool SpecifyWidth { get; set; } = false;

    /// <summary>
    /// 使用指定的高度
    /// </summary>
    public bool SpecifyHeight { get; set; } = false;
}