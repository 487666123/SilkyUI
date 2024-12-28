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
    private void HorizontalArrangeFlowElements(float startingPoint = 0f)
    {
        HorizontalArrangeElements(FlowElements, startingPoint, Gap);
    }

    /// <summary>
    /// 横向排列元素
    /// </summary>
    private void HorizontalArrangeElements(List<View> elements, float startingPoint, Vector2 gap)
    {
        if (elements is { Count: <= 0 })
            return;

        var maxHeight = elements.DefaultIfEmpty().Max(el => el._outerDimensions.Height);

        foreach (var element in elements)
        {
            float offsetY;
            switch (CrossAxisAlignment)
            {
                default:
                case CrossAxisAlignment.Start:
                    offsetY = 0f;
                    break;
                case CrossAxisAlignment.Center:
                    offsetY = (maxHeight - element._outerDimensions.Height) / 2;
                    break;
                case CrossAxisAlignment.End:
                    offsetY = maxHeight - element._outerDimensions.Height;
                    break;
            }

            // element.CorrectionOffset.X += startingPoint;
            // element.CorrectionOffset.Y += offsetY;
            element.DimensionsOffset(startingPoint, offsetY);
            startingPoint += element.GetOuterDimensions().Width + gap.X;
        }
    }

    /// <summary>
    /// 纵向排列元素
    /// </summary>
    private void VerticalArrangeFlowElements(float startingPoint = 0f)
    {
        VerticalArrangeElements(FlowElements, startingPoint, Gap);
    }

    /// <summary>
    /// 纵向排列元素
    /// </summary>
    private void VerticalArrangeElements(List<View> elements, float startingPoint, Vector2 gap)
    {
        if (elements is { Count: <= 0 })
            return;

        var maxWidth = elements.Max(el => el._outerDimensions.Width);

        foreach (var element in elements)
        {
            float offsetX;
            switch (CrossAxisAlignment)
            {
                default:
                case CrossAxisAlignment.Start:
                    offsetX = 0f;
                    break;
                case CrossAxisAlignment.Center:
                    offsetX = (maxWidth - element._outerDimensions.Width) / 2;
                    break;
                case CrossAxisAlignment.End:
                    offsetX = maxWidth - element._outerDimensions.Width;
                    break;
            }

            // element.CorrectionOffset.X += offsetX;
            // element.CorrectionOffset.Y += startingPoint;
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

    public List<FlexItem> FlexItems { get; } = [];

    /// <summary>
    /// 计算 FlexItem
    /// </summary>
    protected void CalculateFlexItem()
    {
        if (Display is not Display.Flexbox || !FlexWrap || FlowElements.Count == 0) return;

        FlexItems.Clear();

        var flexItem = new FlexItem();
        FlexItems.Add(flexItem);
        flexItem.Add(FlowElements[0]);

        switch (FlexDirection)
        {
            default:
            case FlexDirection.Row:
                var containerWidth = _innerDimensions.Width;

                for (var i = 1; i < FlowElements.Count; i++)
                {
                    var element = FlowElements[i];
                    var elementWidth = element._outerDimensions.Width;

                    if (flexItem.Width + elementWidth > containerWidth)
                    {
                        flexItem = new FlexItem();
                        FlexItems.Add(flexItem);
                    }
                    else
                    {
                        flexItem.Width += elementWidth;
                    }

                    flexItem.Add(element);
                }

                break;
            case FlexDirection.Column:
                var containerHeight = _innerDimensions.Height;

                for (var i = 1; i < FlowElements.Count; i++)
                {
                    var element = FlowElements[i];
                    var elementHeight = element._outerDimensions.Height;

                    if (flexItem.Height + elementHeight > containerHeight)
                    {
                        flexItem = new FlexItem();
                        FlexItems.Add(flexItem);
                    }
                    else
                    {
                        flexItem.Height += elementHeight;
                    }

                    flexItem.Add(element);
                }

                break;
        }
    }

    protected void CalculateFlexLayout()
    {
        float gap;
        switch (FlexDirection)
        {
            default:
            case FlexDirection.Row:
                float width;
                if (MainAxisAlignment is MainAxisAlignment.Start || !SpecifyWidth)
                {
                    HorizontalArrangeFlowElements();
                }
                else
                    switch (MainAxisAlignment)
                    {
                        default:
                            HorizontalArrangeFlowElements();
                            break;
                        case MainAxisAlignment.End:

                            width =
                                FlowElements.Sum(el => el._outerDimensions.Width) +
                                Gap.X * (FlowElements.Count - 1);
                            HorizontalArrangeFlowElements(_innerDimensions.Width - width);
                            break;
                        case MainAxisAlignment.Center:
                            width =
                                FlowElements.Sum(el => el._outerDimensions.Width) +
                                Gap.X * (FlowElements.Count - 1);
                            HorizontalArrangeFlowElements((_innerDimensions.Width - width) / 2f);
                            break;
                            break;
                        case MainAxisAlignment.SpaceEvenly:
                            width =
                                FlowElements.Sum(el => el._outerDimensions.Width) +
                                Gap.X * (FlowElements.Count - 1);
                            gap = (_innerDimensions.Width - width) / (FlowElements.Count + 1);
                            HorizontalArrangeElements(FlowElements, gap, new Vector2(gap));
                            break;
                        case MainAxisAlignment.SpaceBetween:
                            var totalWidth = FlowElements.Aggregate(0f,
                                (current, elements) => current + elements._outerDimensions.Width);
                            gap = (_innerDimensions.Width - totalWidth) / (FlowElements.Count - 1);
                            HorizontalArrangeElements(FlowElements, 0, new Vector2(gap));
                            break;
                    }

                break;
            case FlexDirection.Column:
                float height;
                if (MainAxisAlignment is MainAxisAlignment.Start || !SpecifyWidth)
                {
                    VerticalArrangeFlowElements();
                }
                else
                    switch (MainAxisAlignment)
                    {
                        default:
                            VerticalArrangeFlowElements();
                            break;
                        case MainAxisAlignment.End:

                            height =
                                FlowElements.Sum(el => el._outerDimensions.Height) +
                                Gap.Y * (FlowElements.Count - 1);
                            VerticalArrangeFlowElements(_innerDimensions.Height - height);
                            break;
                        case MainAxisAlignment.Center:
                            height =
                                FlowElements.Sum(el => el._outerDimensions.Height) +
                                Gap.Y * (FlowElements.Count - 1);
                            VerticalArrangeFlowElements((_innerDimensions.Height - height) / 2);
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