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

    /// <summary>
    /// Flexbox 占用剩余空间分数<br/>
    /// 如果使用, 主轴方向上的大小将会不生效, 转而由 FlexFraction 控制
    /// </summary>
    public bool UseFlexFraction { get; set; }

    public float FlexFraction { get; set; }

    public int MaxMainAxisElementCount { get; set; }

    #endregion

    private void FlexboxHPlacement(List<View> elements, float start, float gap, float cross = 0f)
    {
        if (elements is null || elements.Count == 0) return;

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

            element.Position += new Vector2(start, offsetY);
            start += element._outerDimensions.Width + gap;
        }
    }

    private void FlexboxVPlacement(List<View> elements, float start, float gap, float cross = 0f)
    {
        if (elements is null || elements.Count == 0) return;

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

            element.Position += new Vector2(offsetX, start);
            start += element._outerDimensions.Height + gap;
        }
    }

    public Vector2 GetFlexboxSize()
    {
        if (FlexItems is null || FlexItems.Count == 0) return Vector2.Zero;

        switch (FlexDirection)
        {
            default:
            case FlexDirection.Row:
                return new Vector2(FlexItems.Max(item => item.Width),
                    FlexItems.Sum(item => item.Height + Gap.Y) - Gap.Y);
            case FlexDirection.Column:
                return new Vector2(FlexItems.Sum(item => item.Width + Gap.X) - Gap.X,
                    FlexItems.Max(item => item.Height));
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

    protected void ProcessFlexItems()
    {
        if (Display is not Display.Flexbox || FlowElements.Count == 0) return;

        FlexItems.Clear();

        FlexItem flexItem;

        var firstElement = FlowElements[0];
        FlexItems.Add(flexItem = new FlexItem());
        flexItem.Add(firstElement);
        flexItem.Width = firstElement._outerDimensions.Width;
        flexItem.Height = firstElement._outerDimensions.Height;

        // 不换行
        if (!FlexWrap
            || (FlexDirection is FlexDirection.Row && !SpecifyWidth)
            || (FlexDirection is FlexDirection.Column && !SpecifyHeight))
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
                var maxWidth = SpecifyWidth ? _innerDimensions.Width : float.MaxValue;

                for (var i = 1; i < FlowElements.Count; i++)
                {
                    var element = FlowElements[i];

                    if (flexItem.Width + element._outerDimensions.Width + Gap.X > maxWidth)
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
                var maxHeight = SpecifyHeight ? _innerDimensions.Height : float.MaxValue;

                for (var i = 1; i < FlowElements.Count; i++)
                {
                    var element = FlowElements[i];

                    if (flexItem.Height + element._outerDimensions.Height + Gap.Y > maxHeight)
                    {
                        FlexItems.Add(flexItem = new FlexItem());
                        flexItem.Height += element._outerDimensions.Height;
                    }
                    else flexItem.Height += element._outerDimensions.Height + Gap.Y;

                    flexItem.Width = Math.Max(flexItem.Width, element._outerDimensions.Width);
                    flexItem.Add(element);
                }

                break;
        }
    }

    protected virtual void ReflowFlexLayout()
    {
        float gap;
        var currentCross = 0f;
        switch (FlexDirection)
        {
            default:
            case FlexDirection.Row:
                if (!SpecifyWidth) FlexboxHPlacement(FlowElements, 0f, Gap.X);
                else
                    switch (MainAxisAlignment)
                    {
                        default:
                        case MainAxisAlignment.Start:
                            foreach (var flexItem in FlexItems)
                            {
                                FlexboxHPlacement(flexItem.Elements, 0f, Gap.X, currentCross);
                                currentCross += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.End:
                            foreach (var flexItem in FlexItems)
                            {
                                FlexboxHPlacement(flexItem.Elements,
                                    _innerDimensions.Width - flexItem.Width, Gap.X, currentCross);
                                currentCross += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.Center: // Center
                            foreach (var flexItem in FlexItems)
                            {
                                FlexboxHPlacement(flexItem.Elements,
                                    (_innerDimensions.Width - flexItem.Width) / 2f, Gap.X, currentCross);
                                currentCross += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.SpaceEvenly:
                            foreach (var flexItem in FlexItems)
                            {
                                var sum = flexItem.Elements.Sum(el => el._outerDimensions.Width);
                                gap = (_innerDimensions.Width - sum) / (flexItem.Elements.Count + 1);
                                FlexboxHPlacement(flexItem.Elements, gap, gap, currentCross);
                                currentCross += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.SpaceBetween:
                            foreach (var flexItem in FlexItems)
                            {
                                var sum = flexItem.Elements.Sum(el => el._outerDimensions.Width);
                                gap = (_innerDimensions.Width - sum) / (flexItem.Elements.Count - 1);
                                FlexboxHPlacement(flexItem.Elements, 0f, gap, currentCross);
                                currentCross += flexItem.Height + Gap.Y;
                            }

                            break;
                    }

                break;
            case FlexDirection.Column:
                if (!SpecifyHeight) FlexboxVPlacement(FlowElements, 0f, Gap.Y);
                else
                    switch (MainAxisAlignment)
                    {
                        default:
                        case MainAxisAlignment.Start:
                            foreach (var flexItem in FlexItems)
                            {
                                FlexboxVPlacement(flexItem.Elements, 0f, Gap.Y, currentCross);
                                currentCross += flexItem.Width + Gap.X;
                            }

                            break;
                        case MainAxisAlignment.End:
                            foreach (var flexItem in FlexItems)
                            {
                                FlexboxVPlacement(flexItem.Elements,
                                    _innerDimensions.Height - flexItem.Height, Gap.Y, currentCross);
                                currentCross += flexItem.Width + Gap.X;
                            }

                            break;
                        case MainAxisAlignment.Center:
                            foreach (var flexItem in FlexItems)
                            {
                                FlexboxVPlacement(flexItem.Elements,
                                    (_innerDimensions.Height - flexItem.Height) / 2f, Gap.Y, currentCross);
                                currentCross += flexItem.Width + Gap.X;
                            }

                            break;
                        case MainAxisAlignment.SpaceEvenly:
                            foreach (var flexItem in FlexItems)
                            {
                                var sum = flexItem.Elements.Sum(el => el._outerDimensions.Height);
                                gap = (_innerDimensions.Height - sum) / (flexItem.Elements.Count + 1);
                                FlexboxVPlacement(flexItem.Elements, gap, gap, currentCross);
                                currentCross += flexItem.Width + Gap.X;
                            }

                            break;
                        case MainAxisAlignment.SpaceBetween:
                            foreach (var flexItem in FlexItems)
                            {
                                var sum = flexItem.Elements.Sum(el => el._outerDimensions.Height);
                                gap = (_innerDimensions.Height - sum) / (flexItem.Elements.Count - 1);
                                FlexboxVPlacement(flexItem.Elements, 0f, gap, currentCross);
                                currentCross += flexItem.Width + Gap.X;
                            }

                            break;
                    }

                break;
        }
    }
}