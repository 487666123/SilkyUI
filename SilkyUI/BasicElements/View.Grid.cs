namespace SilkyUI.BasicElements;

public partial class View
{
    public Grid Grid;
}

public struct Grid()
{
    /// <summary>
    /// 横向格子
    /// </summary>
    public List<LayoutUnit> TemplateRows { get; init; } = [];

    /// <summary>
    /// 纵向各自
    /// </summary>
    public List<LayoutUnit> TemplateColumns { get; init; } = [];
}