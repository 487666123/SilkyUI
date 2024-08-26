namespace SilkyUI.BasicElements;

#region ENUMS

/// <summary>
/// FlexBox 布局方向
/// </summary>
public enum FlexDirection { Row, Column }

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

#endregion

/// <summary>
/// 别用 别用 别用
/// </summary>
public class SUIFlexLayout : View
{
    /// <summary>
    /// 布局方向
    /// </summary>
    public FlexDirection FlexDirection { get; set; } = FlexDirection.Row;

    /// <summary>
    /// 主轴对齐方式
    /// </summary>
    public MainAxisAlign MainAxisAlign { get; set; } = MainAxisAlign.Start;

    /// <summary>
    /// 交叉轴对齐方式
    /// </summary>
    public CrossAxisAlign CrossAxisAlign { get; set; } = CrossAxisAlign.Start;

    /// <summary>
    /// 调整子元素间距<br/>
    /// <see cref="Gap"/>.X 为主轴方向, <see cref="Gap"/>.Y 为交叉轴方向, 
    /// </summary>
    public Vector2 Gap;

    public override void RecalculateChildren()
    {
        base.RecalculateChildren();
    }
}