namespace SilkyUI.BasicElements;

/// <summary> 主轴排列方式 </summary>
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

/// <summary> 交叉轴对齐 </summary>
public enum CrossAxisAlign
{
    /// <summary> 总体靠上 </summary>
    Start,

    /// <summary> 总体居中 </summary>
    Center,

    /// <summary> 总体靠下 </summary>
    End
}

/// <summary> Flex 布局方向 </summary>
public enum FlexDirection
{
    Row,
    Column
}

public partial class View
{
    /// <summary> 主轴对齐方式 </summary>
    public MainAxisAlign MainAxisAlign { get; set; } = MainAxisAlign.Start;

    /// <summary> 交叉轴对齐方式 </summary>
    public CrossAxisAlign CrossAxisAlign { get; set; } = CrossAxisAlign.Start;

    /// <summary> Flex 布局方向 </summary>
    public FlexDirection FlexDirection { get; set; } = FlexDirection.Row;

    private void RunActionByFlexDirection(Action rowAction, Action columnAction)
    {
        switch (FlexDirection)
        {
            default:
            case FlexDirection.Row:
                rowAction?.Invoke();
                break;
            case FlexDirection.Column:
                columnAction?.Invoke();
                break;
        }
    }

    private void RunActionByMainAxisAlign(
        Action startAction, Action endAction, Action centerAction,
        Action spaceBetweenAction, Action spaceEvenlyAction)
    {
        switch (MainAxisAlign)
        {
            default:
            case MainAxisAlign.Start:
                startAction?.Invoke();
                break;
            case MainAxisAlign.End:
                endAction?.Invoke();
                break;
            case MainAxisAlign.Center:
                centerAction?.Invoke();
                break;
            case MainAxisAlign.SpaceEvenly:
                spaceEvenlyAction?.Invoke();
                break;
            case MainAxisAlign.SpaceBetween:
                spaceBetweenAction?.Invoke();
                break;
        }
    }

    private void CalculateFlexLayout()
    {
        RunActionByMainAxisAlign(() =>
        {
            RunActionByFlexDirection(() =>
            {
                var currentLeft = 0f;
                if (SpecifyWidth) // 未固定宽度, 无需换行
                {
                    foreach (var el in FlowElements)
                    {
                        el.DimensionsOffsetX(currentLeft);
                        currentLeft += el._outerDimensions.Width + Gap.X;
                    }
                }
                else // 固定款第
                {
                    foreach (var el in FlowElements)
                    {
                        el.DimensionsOffsetX(currentLeft);
                        if (el._outerDimensions.Right() > _innerDimensions.Right())
                            currentLeft = 0;
                        else
                            currentLeft += el._outerDimensions.Width + Gap.X;
                    }
                }
            }, () =>
            {
                if (!SpecifyHeight) return; // 未固定高度, 无需换行
                var currentTop = 0f;

                foreach (var el in FlowElements)
                {
                    el.DimensionsOffsetY(currentTop);
                    currentTop += el._outerDimensions.Height + Gap.Y;
                }
            });
        }, () =>
        {
            switch (FlexDirection)
            {
                case FlexDirection.Row:
                {
                    if (!SpecifyWidth)
                        break;

                    // 未固定宽度, 无需换行
                    var gapCount = FlowElements.Count - 1;
                    var width = FlowElements.Sum(el => el._outerDimensions.Width) + Gap.X * gapCount;
                    var currentLeft = _innerDimensions.Width - width;

                    foreach (var el in FlowElements)
                    {
                        el.DimensionsOffsetX(currentLeft);
                        currentLeft += el._outerDimensions.Width + Gap.X;
                    }

                    break;
                }

                case FlexDirection.Column:
                {
                    if (!SpecifyHeight)
                        break;

                    // 未固定高度, 无需换行
                    var gapCount = FlowElements.Count - 1;
                    var height = FlowElements.Sum(el => el._outerDimensions.Height) + Gap.Y * gapCount;

                    var currentTop = _innerDimensions.Height - height;

                    foreach (var el in FlowElements)
                    {
                        el.DimensionsOffsetY(currentTop);
                        currentTop += el._outerDimensions.Height + Gap.Y;
                    }

                    break;
                }

                default: break;
            }
        }, () =>
        {
            switch (FlexDirection)
            {
                case FlexDirection.Row:
                {
                    if (!SpecifyWidth)
                        break;

                    var totalWidth = FlowElements.Aggregate(0f,
                        (current, elements) => current + elements._outerDimensions.Width + Gap.X) - Gap.X;

                    var currentOffset = new Vector2(_innerDimensions.Width - totalWidth, 0f) / 2f;

                    foreach (var element in FlowElements)
                    {
                        element.DimensionsOffsetX(currentOffset.X);
                        currentOffset.X += element._outerDimensions.Width + Gap.X;
                    }

                    break;
                }

                case FlexDirection.Column:
                {
                    if (!SpecifyHeight)
                        break;

                    var totalHeight = FlowElements.Aggregate(0f,
                        (current, elements) => current + elements._outerDimensions.Height + Gap.Y) - Gap.Y;

                    var currentOffset = new Vector2(0f, _innerDimensions.Height - totalHeight) / 2f;

                    foreach (var element in FlowElements)
                    {
                        element.DimensionsOffsetY(currentOffset.Y);
                        currentOffset.Y += element._outerDimensions.Height + Gap.Y;
                    }

                    break;
                }

                default: break;
            }
        }, () =>
        {
            switch (FlexDirection)
            {
                case FlexDirection.Row:
                {
                    if (!SpecifyWidth)
                        break;

                    var totalWidth = FlowElements.Aggregate(0f,
                        (current, elements) => current + elements._outerDimensions.Width);

                    var gap = (_innerDimensions.Width - totalWidth) / (FlowElements.Count + 1);

                    var currentOffset = new Vector2(gap, 0f);

                    foreach (var element in FlowElements)
                    {
                        element.DimensionsOffsetX(currentOffset.X);
                        currentOffset.X += element._outerDimensions.Width + gap;
                    }

                    break;
                }

                case FlexDirection.Column:
                {
                    if (!SpecifyHeight)
                        break;

                    var totalHeight = FlowElements.Aggregate(0f,
                        (current, elements) => current + elements._outerDimensions.Height);

                    var gap = (_innerDimensions.Height - totalHeight) / (FlowElements.Count + 1);

                    var currentOffset = new Vector2(0f, gap);

                    foreach (var element in FlowElements)
                    {
                        element.DimensionsOffsetY(currentOffset.Y);
                        currentOffset.Y += element._outerDimensions.Height + gap;
                    }

                    break;
                }

                default: break;
            }
        }, () =>
        {
            switch (FlexDirection)
            {
                case FlexDirection.Row:
                {
                    if (!SpecifyWidth)
                        break;

                    var totalWidth = FlowElements.Aggregate(0f,
                        (current, elements) => current + elements._outerDimensions.Width);

                    var gap = (_innerDimensions.Width - totalWidth) / (FlowElements.Count - 1);

                    var currentOffset = new Vector2(0f, 0f);

                    foreach (var element in FlowElements)
                    {
                        element.DimensionsOffsetX(currentOffset.X);
                        currentOffset.X += element._outerDimensions.Width + gap;
                    }

                    break;
                }

                case FlexDirection.Column:
                {
                    if (!SpecifyHeight)
                        break;

                    var totalHeight = FlowElements.Aggregate(0f,
                        (current, elements) => current + elements._outerDimensions.Height);

                    var gap = (_innerDimensions.Height - totalHeight) / (FlowElements.Count - 1);

                    var currentOffset = new Vector2(0f, 0f);

                    foreach (var element in FlowElements)
                    {
                        element.DimensionsOffsetY(currentOffset.Y);
                        currentOffset.Y += element._outerDimensions.Height + gap;
                    }

                    break;
                }
            }
        });
    }
}