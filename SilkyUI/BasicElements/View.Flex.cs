namespace SilkyUI.BasicElements;

/// <summary> 主轴对其方式 </summary>
public enum MainAxisAlignment
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

/// <summary> 交叉轴对齐方式 </summary>
public enum CrossAxisAlignment
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
    #region Flex Properties

    /// <summary> 主轴对齐方式 </summary>
    public MainAxisAlignment MainAxisAlignment { get; set; } = MainAxisAlignment.Start;

    /// <summary> 交叉轴对齐方式 </summary>
    public CrossAxisAlignment CrossAxisAlignment { get; set; } = CrossAxisAlignment.Start;

    /// <summary> Flex 布局方向 </summary>
    public FlexDirection FlexDirection { get; set; } = FlexDirection.Row;

    public bool FlexWrap { get; set; } = true;

    #endregion

    /// <summary>
    /// 横向排列元素
    /// </summary>
    private void HorizontalArrangeElements(List<View> elements, float startingPoint, float gap, float cross = 0f)
    {
        if (elements is { Count: <= 0 }) return;

        var maxHeight = elements.DefaultIfEmpty().Max(el => el._outerDimensions.Height);

        foreach (var element in elements)
        {
            var offsetY = cross;
            switch (CrossAxisAlignment)
            {
                default:
                case CrossAxisAlignment.Start:
                    offsetY += 0f;
                    break;
                case CrossAxisAlignment.Center:
                    offsetY += (maxHeight - element._outerDimensions.Height) / 2;
                    break;
                case CrossAxisAlignment.End:
                    offsetY += maxHeight - element._outerDimensions.Height;
                    break;
            }

            element.DimensionsOffset(startingPoint, offsetY);
            startingPoint += element.GetOuterDimensions().Width + gap;
        }
    }

    /// <summary>
    /// 纵向排列元素
    /// </summary>
    private void VerticalArrangeElements(List<View> elements, float startingPoint, Vector2 gap, float cross = 0f)
    {
        if (elements is { Count: <= 0 })
            return;

        var maxWidth = elements.Max(el => el._outerDimensions.Width);

        foreach (var element in elements)
        {
            var offsetX = cross;
            switch (CrossAxisAlignment)
            {
                default:
                case CrossAxisAlignment.Start:
                    offsetX += 0f;
                    break;
                case CrossAxisAlignment.Center:
                    offsetX += (maxWidth - element._outerDimensions.Width) / 2;
                    break;
                case CrossAxisAlignment.End:
                    offsetX += maxWidth - element._outerDimensions.Width;
                    break;
            }

            element.DimensionsOffset(offsetX, startingPoint);
            startingPoint += element.GetOuterDimensions().Height + gap.Y;
        }
    }

    public class FlexItem
    {
        public readonly List<View> Elements = [];
        public float Width { get; set; }
        public float Height { get; set; }

        public void Add(View element)
        {
            Elements.Add(element);
        }
    }

    protected readonly List<FlexItem> FlexItems = [];

    /// <summary>
    /// 计算 FlexItem
    /// </summary>
    protected void CalculateFlexItem()
    {
        if (Display is not Display.Flexbox || FlowElements.Count == 0) return;

        FlexItems.Clear();

        FlexItem flexItem;

        FlexItems.Add(flexItem = new FlexItem());
        flexItem.Add(FlowElements[0]);
        flexItem.Width = FlowElements[0]._outerDimensions.Width;
        flexItem.Height = FlowElements[0]._outerDimensions.Height;

        // 不换行
        if (!FlexWrap)
        {
            switch (FlexDirection)
            {
                default:
                case FlexDirection.Row:
                    for (var i = 1; i < FlowElements.Count; i++)
                    {
                        var element = FlowElements[i];

                        flexItem.Width += element._outerDimensions.Width + Gap.X;
                        flexItem.Height = Math.Max(flexItem.Height, element._outerDimensions.Height);

                        flexItem.Add(element);
                    }

                    break;
                case FlexDirection.Column:
                    for (var i = 1; i < FlowElements.Count; i++)
                    {
                        var element = FlowElements[i];

                        flexItem.Height += element._outerDimensions.Height + Gap.Y;
                        flexItem.Width = Math.Max(flexItem.Width, element._outerDimensions.Width);

                        flexItem.Add(element);
                    }

                    break;
            }

            return;
        }

        // 换行
        switch (FlexDirection)
        {
            default:
            case FlexDirection.Row:
                var containerWidth = _innerDimensions.Width;

                for (var i = 1; i < FlowElements.Count; i++)
                {
                    var element = FlowElements[i];

                    if (flexItem.Width + element._outerDimensions.Width + Gap.X > containerWidth)
                    {
                        FlexItems.Add(flexItem = new FlexItem());

                        flexItem.Width = element._outerDimensions.Width;
                    }
                    else flexItem.Width += element._outerDimensions.Width + Gap.X;

                    flexItem.Height = Math.Max(flexItem.Height, element._outerDimensions.Height);
                    flexItem.Add(element);
                }

                break;
            case FlexDirection.Column:
                var containerHeight = _innerDimensions.Height;

                for (var i = 1; i < FlowElements.Count; i++)
                {
                    var element = FlowElements[i];
                    var elementHeight = element._outerDimensions.Height;

                    if (flexItem.Height + elementHeight + Gap.Y > containerHeight)
                    {
                        FlexItems.Add(flexItem = new FlexItem());
                        flexItem.Height += elementHeight;
                    }
                    else flexItem.Height += elementHeight + Gap.Y;

                    flexItem.Width = Math.Max(flexItem.Width, element._outerDimensions.Width);
                    flexItem.Add(element);
                }

                break;
        }
    }

    protected virtual void CalculateFlexLayout()
    {
        CalculateFlexItem();

        float gap;
        var currentCross = 0f;
        switch (FlexDirection)
        {
            default:
            case FlexDirection.Row:
                float width;
                if (!SpecifyWidth) HorizontalArrangeElements(FlowElements, 0f, Gap.X);
                else
                    switch (MainAxisAlignment)
                    {
                        default:
                        case MainAxisAlignment.Start:
                            foreach (var flexItem in FlexItems)
                            {
                                HorizontalArrangeElements(flexItem.Elements, 0f, Gap.X, currentCross);
                                currentCross += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.End:
                            width = FlexItems.Max(item => item.Width);
                            foreach (var flexItem in FlexItems)
                            {
                                var widthSum = flexItem.Elements.Sum(el => el._outerDimensions.Width + Gap.X) - Gap.X;
                                HorizontalArrangeElements(flexItem.Elements,
                                    _innerDimensions.Width - flexItem.Width, Gap.X, currentCross);
                                currentCross += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.Center: // Center
                            foreach (var flexItem in FlexItems)
                            {
                                HorizontalArrangeElements(flexItem.Elements,
                                    (_innerDimensions.Width - flexItem.Width) / 2f, Gap.X, currentCross);
                                currentCross += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.SpaceEvenly:
                            foreach (var flexItem in FlexItems)
                            {
                                var sum = flexItem.Elements.Sum(el => el._outerDimensions.Width);
                                gap = (_innerDimensions.Width - sum) / (flexItem.Elements.Count + 1);
                                HorizontalArrangeElements(flexItem.Elements, gap, gap, currentCross);
                                currentCross += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.SpaceBetween:
                            foreach (var flexItem in FlexItems)
                            {
                                var sum = flexItem.Elements.Sum(el => el._outerDimensions.Width);
                                gap = (_innerDimensions.Width - sum) / (flexItem.Elements.Count - 1);
                                HorizontalArrangeElements(flexItem.Elements, 0f, gap, currentCross);
                                currentCross += flexItem.Height + Gap.Y;
                            }

                            break;
                    }

                break;
            case FlexDirection.Column:
                float height;
                if (!SpecifyWidth) VerticalArrangeElements(FlowElements, 0f, Gap);
                else
                    switch (MainAxisAlignment)
                    {
                        default:
                        case MainAxisAlignment.Start:
                            VerticalArrangeElements(FlowElements, 0f, Gap);
                            break;
                        case MainAxisAlignment.End:
                            height =
                                FlowElements.Sum(el => el._outerDimensions.Height) +
                                Gap.Y * (FlowElements.Count - 1);
                            VerticalArrangeElements(FlowElements, _innerDimensions.Height - height, Gap);
                            break;
                        case MainAxisAlignment.Center:
                            height =
                                FlowElements.Sum(el => el._outerDimensions.Height) +
                                Gap.Y * (FlowElements.Count - 1);
                            VerticalArrangeElements(FlowElements, (_innerDimensions.Height - height) / 2, Gap);
                            break;
                        case MainAxisAlignment.SpaceEvenly:
                            height =
                                FlowElements.Sum(el => el._outerDimensions.Height) +
                                Gap.Y * (FlowElements.Count - 1);
                            gap = (_innerDimensions.Height - height) / (FlowElements.Count + 1);
                            VerticalArrangeElements(FlowElements, gap, new Vector2(gap));
                            break;
                        case MainAxisAlignment.SpaceBetween:
                            height = FlowElements.Aggregate(0f,
                                (current, elements) => current + elements._outerDimensions.Height);
                            gap = (_innerDimensions.Height - height) / (FlowElements.Count - 1);
                            VerticalArrangeElements(FlowElements, 0, new Vector2(gap));
                            break;
                    }

                break;
        }
    }
}