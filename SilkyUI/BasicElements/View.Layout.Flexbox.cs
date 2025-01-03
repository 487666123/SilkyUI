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
    #region Flexbox 属性

    /// <summary> 主轴对齐方式 </summary>
    public MainAxisAlignment MainAxisAlignment { get; set; } = MainAxisAlignment.Start;

    /// <summary> 交叉轴对齐方式 </summary>
    public CrossAxisAlignment CrossAxisAlignment { get; set; } = CrossAxisAlignment.Start;

    /// <summary> Flex 布局方向 </summary>
    public FlexDirection FlexDirection { get; set; } = FlexDirection.Row;

    /// <summary> 是否换行 </summary>
    public bool FlexWrap { get; set; } = true;

    #endregion

    public Vector2 GetFlexboxSize()
    {
        if (FlexboxItems is null || FlexboxItems.Count == 0) return Vector2.Zero;

        switch (FlexDirection)
        {
            default:
            case FlexDirection.Row:
                return new Vector2(FlexboxItems.Max(item => item.Width),
                    FlexboxItems.Sum(item => item.Height + Gap.Y) - Gap.Y);
            case FlexDirection.Column:
                return new Vector2(FlexboxItems.Sum(item => item.Width + Gap.X) - Gap.X,
                    FlexboxItems.Max(item => item.Height));
        }
    }

    protected readonly List<FlexboxItem> FlexboxItems = [];

    /// <summary>
    /// 组织 Flexbox 项目: 分行分列
    /// </summary>
    protected void OrganizingFlexboxItems()
    {
        FlexboxItems.Clear();
        if (Display is not Display.Flexbox || FlowElements.Count == 0) return;

        FlexboxItem flexboxItem;

        var firstElement = FlowElements[0];
        FlexboxItems.Add(flexboxItem = new FlexboxItem());
        flexboxItem.Add(firstElement);
        flexboxItem.Width = firstElement._outerDimensions.Width;
        flexboxItem.Height = firstElement._outerDimensions.Height;

        // 不换行
        if (!FlexWrap || (FlexDirection is FlexDirection.Row && !SpecifyWidth) ||
            (FlexDirection is FlexDirection.Column && !SpecifyHeight))
        {
            switch (FlexDirection)
            {
                default:
                case FlexDirection.Row:
                    for (var i = 1; i < FlowElements.Count; i++)
                    {
                        var element = FlowElements[i];

                        flexboxItem.Width += element._outerDimensions.Width + Gap.X;
                        flexboxItem.Height = Math.Max(flexboxItem.Height, element._outerDimensions.Height);

                        flexboxItem.Add(element);
                    }

                    return;
                case FlexDirection.Column:
                    for (var i = 1; i < FlowElements.Count; i++)
                    {
                        var element = FlowElements[i];

                        flexboxItem.Height += element._outerDimensions.Height + Gap.Y;
                        flexboxItem.Width = Math.Max(flexboxItem.Width, element._outerDimensions.Width);

                        flexboxItem.Add(element);
                    }

                    return;
            }
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

                    if (flexboxItem.Width + element._outerDimensions.Width + Gap.X > maxWidth)
                    {
                        FlexboxItems.Add(flexboxItem = new FlexboxItem());
                        flexboxItem.Width = element._outerDimensions.Width;
                    }
                    else flexboxItem.Width += element._outerDimensions.Width + Gap.X;

                    flexboxItem.Height = Math.Max(flexboxItem.Height, element._outerDimensions.Height);
                    flexboxItem.Add(element);
                }

                break;
            case FlexDirection.Column:
                var maxHeight = SpecifyHeight ? _innerDimensions.Height : float.MaxValue;

                for (var i = 1; i < FlowElements.Count; i++)
                {
                    var element = FlowElements[i];

                    if (flexboxItem.Height + element._outerDimensions.Height + Gap.Y > maxHeight)
                    {
                        FlexboxItems.Add(flexboxItem = new FlexboxItem());
                        flexboxItem.Height += element._outerDimensions.Height;
                    }
                    else flexboxItem.Height += element._outerDimensions.Height + Gap.Y;

                    flexboxItem.Width = Math.Max(flexboxItem.Width, element._outerDimensions.Width);
                    flexboxItem.Add(element);
                }

                break;
        }
    }

    protected virtual void FlexboxArrange()
    {
        float gap;
        var crossAxis = 0f;
        switch (FlexDirection)
        {
            default:
            case FlexDirection.Row:
                if (!SpecifyWidth) FlexboxHArrange(FlowElements, 0f, Gap.X);
                else
                    switch (MainAxisAlignment)
                    {
                        default:
                        case MainAxisAlignment.Start:
                            foreach (var flexItem in FlexboxItems)
                            {
                                FlexboxHArrange(flexItem.Elements, 0f, Gap.X, crossAxis);
                                crossAxis += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.End:
                            foreach (var flexItem in FlexboxItems)
                            {
                                FlexboxHArrange(flexItem.Elements,
                                    _innerDimensions.Width - flexItem.Width, Gap.X, crossAxis);
                                crossAxis += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.Center:
                            foreach (var flexItem in FlexboxItems)
                            {
                                FlexboxHArrange(flexItem.Elements,
                                    (_innerDimensions.Width - flexItem.Width) / 2f, Gap.X, crossAxis);
                                crossAxis += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.SpaceEvenly:
                            foreach (var flexItem in FlexboxItems)
                            {
                                var sum = flexItem.Elements.Sum(el => el._outerDimensions.Width);
                                gap = (_innerDimensions.Width - sum) / (flexItem.Elements.Count + 1);
                                FlexboxHArrange(flexItem.Elements, gap, gap, crossAxis);
                                crossAxis += flexItem.Height + Gap.Y;
                            }

                            break;
                        case MainAxisAlignment.SpaceBetween:
                            foreach (var flexItem in FlexboxItems)
                            {
                                var sum = flexItem.Elements.Sum(el => el._outerDimensions.Width);
                                gap = (_innerDimensions.Width - sum) / (flexItem.Elements.Count - 1);
                                FlexboxHArrange(flexItem.Elements, 0f, gap, crossAxis);
                                crossAxis += flexItem.Height + Gap.Y;
                            }

                            break;
                    }

                break;
            case FlexDirection.Column:
                if (!SpecifyHeight) FlexboxVArrange(FlowElements, 0f, Gap.Y);
                else
                    switch (MainAxisAlignment)
                    {
                        default:
                        case MainAxisAlignment.Start:
                            foreach (var flexItem in FlexboxItems)
                            {
                                FlexboxVArrange(flexItem.Elements, 0f, Gap.Y, crossAxis);
                                crossAxis += flexItem.Width + Gap.X;
                            }

                            break;
                        case MainAxisAlignment.End:
                            foreach (var flexItem in FlexboxItems)
                            {
                                FlexboxVArrange(flexItem.Elements,
                                    _innerDimensions.Height - flexItem.Height, Gap.Y, crossAxis);
                                crossAxis += flexItem.Width + Gap.X;
                            }

                            break;
                        case MainAxisAlignment.Center:
                            foreach (var flexItem in FlexboxItems)
                            {
                                FlexboxVArrange(flexItem.Elements,
                                    (_innerDimensions.Height - flexItem.Height) / 2f, Gap.Y, crossAxis);
                                crossAxis += flexItem.Width + Gap.X;
                            }

                            break;
                        case MainAxisAlignment.SpaceEvenly:
                            foreach (var flexItem in FlexboxItems)
                            {
                                var sum = flexItem.Elements.Sum(el => el._outerDimensions.Height);
                                gap = (_innerDimensions.Height - sum) / (flexItem.Elements.Count + 1);
                                FlexboxVArrange(flexItem.Elements, gap, gap, crossAxis);
                                crossAxis += flexItem.Width + Gap.X;
                            }

                            break;
                        case MainAxisAlignment.SpaceBetween:
                            foreach (var flexItem in FlexboxItems)
                            {
                                var sum = flexItem.Elements.Sum(el => el._outerDimensions.Height);
                                gap = (_innerDimensions.Height - sum) / (flexItem.Elements.Count - 1);
                                FlexboxVArrange(flexItem.Elements, 0f, gap, crossAxis);
                                crossAxis += flexItem.Width + Gap.X;
                            }

                            break;
                    }

                break;
        }
    }

    private void FlexboxHArrange(List<View> elements, float mainAxis, float gap, float crossAxis = 0f)
    {
        if (elements is null || elements.Count == 0) return;

        var maxHeight = elements.DefaultIfEmpty().Max(el => el._outerDimensions.Height);

        foreach (var element in elements)
        {
            var offsetY = crossAxis;
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

            element.Position += new Vector2(mainAxis, offsetY);
            mainAxis += element._outerDimensions.Width + gap;
        }
    }

    private void FlexboxVArrange(List<View> elements, float mainAxis, float gap, float crossAxis = 0f)
    {
        if (elements is null || elements.Count == 0) return;

        var maxWidth = elements.Max(el => el._outerDimensions.Width);

        foreach (var element in elements)
        {
            var offsetX = crossAxis;
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

            element.Position += new Vector2(offsetX, mainAxis);
            mainAxis += element._outerDimensions.Height + gap;
        }
    }
}