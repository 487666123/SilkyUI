namespace SilkyUI.BasicElements;

/// <summary>
/// 主轴排列方式
/// </summary>
public enum MainAxisAlign
{
    Start, Center, End,
    /// <summary>
    /// 平分空间
    /// </summary>
    SpaceEvenly,

    /// <summary>
    /// 两端对齐
    /// </summary>
    SpaceBetween,
}

/// <summary>
/// 交叉轴对齐
/// </summary>
public enum CrossAxisAlign { Start, Center, End }

public partial class View : UIElement
{
    /// <summary>
    /// 主轴对齐方式
    /// </summary>
    public MainAxisAlign MainAxisAlign { get; set; } = MainAxisAlign.Start;

    /// <summary>
    /// 交叉轴对齐方式
    /// </summary>
    public CrossAxisAlign CrossAxisAlign { get; set; } = CrossAxisAlign.Start;
}