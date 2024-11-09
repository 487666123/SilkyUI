namespace SilkyUI.BasicElements;

/// <summary>
/// 主轴排列方式
/// </summary>
public enum MainAxisAlign
{
    /// <summary> 总体靠左 </summary>
    Start,

    /// <summary> 总体靠右 </summary>
    End,

    /// <summary> 总体居中 </summary>
    Center,

    /// <summary> 平分空间 </summary>
    SpaceEvenly,

    /// <summary> 两端对齐 </summary>
    SpaceBetween,
}

/// <summary>
/// 交叉轴对齐
/// </summary>
public enum CrossAxisAlign
{
    /// <summary> 总体靠上 </summary>
    Start,

    /// <summary> 总体居中 </summary>
    Center,

    /// <summary> 总体靠下 </summary>
    End
}

public enum FlexDirection
{
    Row,
    Column
}

public partial class View
{
    /// <summary>
    /// 主轴对齐方式
    /// </summary>
    public MainAxisAlign MainAxisAlign { get; set; } = MainAxisAlign.Start;

    /// <summary>
    /// 交叉轴对齐方式
    /// </summary>
    public CrossAxisAlign CrossAxisAlign { get; set; } = CrossAxisAlign.Start;

    public FlexDirection FlexDirection { get; set; } = FlexDirection.Row;

    public void CalculateFlexLayout()
    {
        switch (MainAxisAlign)
        {
            case MainAxisAlign.Start:
            {
                switch (FlexDirection)
                {
                    case FlexDirection.Row:
                    {
                        var currentOffset = Vector2.Zero;

                        foreach (var element in RelativeElements)
                        {
                            element.OffsetX(currentOffset.X);
                            currentOffset.X += element._outerDimensions.Width + Gap.X;
                        }

                        break;
                    }
                    case FlexDirection.Column:
                    {
                        var currentOffset = Vector2.Zero;

                        foreach (var element in RelativeElements)
                        {
                            element.OffsetY(currentOffset.Y);
                            currentOffset.Y += element._outerDimensions.Height + Gap.Y;
                        }

                        break;
                    }
                    default: break;
                }

                break;
            }
            case MainAxisAlign.End:
                switch (FlexDirection)
                {
                    case FlexDirection.Row:
                    {
                        if (!SpecifyWidth)
                            break;

                        var totalWidth = RelativeElements.Aggregate(0f,
                            (current, elements) => current + elements._outerDimensions.Width + Gap.X) - Gap.X;

                        var currentOffset = new Vector2(_innerDimensions.Width - totalWidth, 0f);

                        foreach (var element in RelativeElements)
                        {
                            element.OffsetX(currentOffset.X);
                            currentOffset.X += element._outerDimensions.Width + Gap.X;
                        }

                        break;
                    }

                    case FlexDirection.Column:
                    {
                        if (!SpecifyHeight)
                            break;

                        var totalHeight = RelativeElements.Aggregate(0f,
                            (current, elements) => current + elements._outerDimensions.Height + Gap.Y) - Gap.Y;

                        var currentOffset = new Vector2(0f, _innerDimensions.Height - totalHeight);

                        foreach (var element in RelativeElements)
                        {
                            element.OffsetY(currentOffset.Y);
                            currentOffset.Y += element._outerDimensions.Height + Gap.Y;
                        }

                        break;
                    }

                    default: break;
                }

                break;
            case MainAxisAlign.Center:
                switch (FlexDirection)
                {
                    case FlexDirection.Row:
                    {
                        if (!SpecifyWidth)
                            break;

                        var totalWidth = RelativeElements.Aggregate(0f,
                            (current, elements) => current + elements._outerDimensions.Width + Gap.X) - Gap.X;

                        var currentOffset = new Vector2(_innerDimensions.Width - totalWidth, 0f) / 2f;

                        foreach (var element in RelativeElements)
                        {
                            element.OffsetX(currentOffset.X);
                            currentOffset.X += element._outerDimensions.Width + Gap.X;
                        }

                        break;
                    }

                    case FlexDirection.Column:
                    {
                        if (!SpecifyHeight)
                            break;

                        var totalHeight = RelativeElements.Aggregate(0f,
                            (current, elements) => current + elements._outerDimensions.Height + Gap.Y) - Gap.Y;

                        var currentOffset = new Vector2(0f, _innerDimensions.Height - totalHeight) / 2f;

                        foreach (var element in RelativeElements)
                        {
                            element.OffsetY(currentOffset.Y);
                            currentOffset.Y += element._outerDimensions.Height + Gap.Y;
                        }

                        break;
                    }

                    default: break;
                }

                break;
            case MainAxisAlign.SpaceEvenly:
                switch (FlexDirection)
                {
                    case FlexDirection.Row:
                    {
                        if (!SpecifyWidth)
                            break;

                        var totalWidth = RelativeElements.Aggregate(0f,
                            (current, elements) => current + elements._outerDimensions.Width);

                        var gap = (_innerDimensions.Width - totalWidth) / (RelativeElements.Count + 1);

                        var currentOffset = new Vector2(gap, 0f);

                        foreach (var element in RelativeElements)
                        {
                            element.OffsetX(currentOffset.X);
                            currentOffset.X += element._outerDimensions.Width + gap;
                        }

                        break;
                    }

                    case FlexDirection.Column:
                    {
                        if (!SpecifyHeight)
                            break;

                        var totalHeight = RelativeElements.Aggregate(0f,
                            (current, elements) => current + elements._outerDimensions.Height);

                        var gap = (_innerDimensions.Height - totalHeight) / (RelativeElements.Count + 1);

                        var currentOffset = new Vector2(0f, gap);

                        foreach (var element in RelativeElements)
                        {
                            element.OffsetY(currentOffset.Y);
                            currentOffset.Y += element._outerDimensions.Height + gap;
                        }

                        break;
                    }

                    default: break;
                }

                break;
            case MainAxisAlign.SpaceBetween:
                switch (FlexDirection)
                {
                    case FlexDirection.Row:
                    {
                        if (!SpecifyWidth)
                            break;

                        var totalWidth = RelativeElements.Aggregate(0f,
                            (current, elements) => current + elements._outerDimensions.Width);

                        var gap = (_innerDimensions.Width - totalWidth) / (RelativeElements.Count - 1);

                        var currentOffset = new Vector2(0f, 0f);

                        foreach (var element in RelativeElements)
                        {
                            element.OffsetX(currentOffset.X);
                            currentOffset.X += element._outerDimensions.Width + gap;
                        }

                        break;
                    }

                    case FlexDirection.Column:
                    {
                        if (!SpecifyHeight)
                            break;

                        var totalHeight = RelativeElements.Aggregate(0f,
                            (current, elements) => current + elements._outerDimensions.Height);

                        var gap = (_innerDimensions.Height - totalHeight) / (RelativeElements.Count - 1);

                        var currentOffset = new Vector2(0f, 0f);

                        foreach (var element in RelativeElements)
                        {
                            element.OffsetY(currentOffset.Y);
                            currentOffset.Y += element._outerDimensions.Height + gap;
                        }

                        break;
                    }

                    default: break;
                }

                break;
            default: break;
        }
    }
}