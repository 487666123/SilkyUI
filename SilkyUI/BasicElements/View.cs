namespace SilkyUI.BasicElements;

/// <summary>
/// 简单说明: 2024.8.26 <br/>
/// 只有 View 能用, 别的都不建议用. <br/>
/// 使用固定宽高必须设置 <see cref="SpecifyWidth"/> <see cref="SpecifyHeight"/> 为 true <br/>
/// 所有 UI 元素都必须基于 <see cref="View"/> <br/>
/// </summary>
public partial class View : UIElement
{
    public View()
    {
        MaxWidth = MaxHeight = new StyleDimension(114514f, 0f);
    }

    #region Methods
    /// <summary>
    /// 获取父元素 <see cref="CalculatedStyle"/><br/>
    /// 绝对定位下与原版相同<br/>
    /// 相对定位下, 父元素不是 <see cref="View"/> 子类与原版相同<br/>
    /// 父元素是 <see cref="View"/> 且父元素 <see cref="SpecifyWidth"/> <see cref="SpecifyHeight"/>
    /// 为 false, 则不使用父元素大小转而设为 0<br/>
    /// 且会根据父元素 <see cref="FlowDirection"/> 决定 <see cref="CalculatedStyle"/> 的 XY,
    /// 排列自身位置<br/>
    /// 位置与 <see cref="UIElementExtensions.PreviousRelativeElement(UIElement)"/> 相关
    /// </summary>
    /// <returns></returns>
    public CalculatedStyle GetParentDimensions()
    {
        if (Parent is not UIElement uie)
            return UserInterface.ActiveInstance.GetDimensions();

        if (uie is not View parent)
            return uie.GetInnerDimensions();

        CalculatedStyle container = parent.GetInnerDimensions();

        // 相对定位, 若父元素明确 [没有指定] 大小, 视为父元素没有大小 (因为需要其称其父元素)
        // 绝对定位, 一定需要父元素大小 (因为不会撑起父元素)
        if (Positioning is Positioning.Relative)
        {
            if (parent.Display is Display.InlineFlex)
            {
                var previousElement = this.PreviousRelativeElement();
                if (previousElement != null)
                {
                    if (parent.FlowDirection is FlowDirection.Column)
                        container.Y =
                            previousElement.GetOuterDimensions().Bottom() + parent.Gap.Y;
                    else
                        container.X =
                            previousElement.GetOuterDimensions().Right() + parent.Gap.X;
                }
            }
            else if (parent.Display is Display.InlineGrid)
            {

            }

            if (!parent.SpecifyWidth)
                container.Width = 0;

            if (!parent.SpecifyHeight)
                container.Height = 0;
        }

        return container;
    }

    public override void Recalculate()
    {
        var parentDimensions = GetParentDimensions();

        _outerDimensions = CalculateDimensionsByParentDimensions(parentDimensions);
        _dimensions = CalculateDimensions(_outerDimensions);
        _innerDimensions = CalculateInnerDimensions(_dimensions);

        RecalculateChildren();
    }

    /// <summary>
    /// 计算属性，基于父元素尺寸<br/>
    /// 不使用 <see cref="UIElement.GetDimensionsBasedOnParentDimensions(CalculatedStyle)"/>
    /// </summary>
    public virtual CalculatedStyle CalculateDimensionsByParentDimensions(CalculatedStyle parentDimensions)
    {
        var rX = Left.GetValue(parentDimensions.Width) + parentDimensions.X;
        var rY = Top.GetValue(parentDimensions.Height) + parentDimensions.Y;

        var rWidth = SpecifyWidth ?
            MathHelper.Clamp(Width.GetValue(parentDimensions.Width),
            MinWidth.GetValue(parentDimensions.Width),
            MaxWidth.GetValue(parentDimensions.Width)) : 0;
        var rHeight = SpecifyHeight ?
            MathHelper.Clamp(Height.GetValue(parentDimensions.Height),
            MinHeight.GetValue(parentDimensions.Height),
            MaxHeight.GetValue(parentDimensions.Height)) : 0;

        var result = BoxSizing switch
        {
            BoxSizing.BorderBox => new CalculatedStyle(
                                rX, rY,
                                rWidth + this.HMargin(),
                                rHeight + this.VMargin()),
            _ => new CalculatedStyle(
                                rX, rY,
                                rWidth + this.HPadding() + this.HMargin() + Border * 2f,
                                rHeight + this.VPadding() + this.VMargin() + Border * 2f),
        };

        if (IsAbsolutePositioning)
        {
            result.X += (parentDimensions.Width - result.Width) * HAlign;
            result.Y += (parentDimensions.Height - result.Height) * VAlign;
        }
        else if (IsRelativePositioning && Parent is View view)
        {
            if (view.SpecifyWidth)
                result.X += (parentDimensions.Width - result.Width) * HAlign;
            if (view.SpecifyHeight)
                result.Y += (parentDimensions.Height - result.Height) * VAlign;
        }

        return result;
    }

    #region CalculateDimensions
    public CalculatedStyle CalculateDimensions(CalculatedStyle outerDimensions)
    {
        CalculatedStyle dimensions = outerDimensions;
        dimensions.X += MarginLeft;
        dimensions.Y += MarginTop;
        dimensions.Width -= MarginLeft + MarginRight;
        dimensions.Height -= MarginTop + MarginBottom;
        return dimensions;
    }

    public CalculatedStyle CalculateInnerDimensions(CalculatedStyle dimensions)
    {
        CalculatedStyle innerDimensions = dimensions;
        innerDimensions.X += PaddingLeft + Border;
        innerDimensions.Y += PaddingTop + Border;
        innerDimensions.Width -= this.HPadding() + Border;
        innerDimensions.Height -= this.VPadding() + Border;
        return innerDimensions;
    }
    #endregion

    protected List<UIElement> _relativeElements = [];
    protected List<UIElement> _absoluteElements = [];

    /// <summary>
    /// 对元素进行分类
    /// </summary>
    public void ClassifyElements()
    {
        _relativeElements.Clear();
        _absoluteElements.Clear();

        Elements.ForEach(e =>
        {
            _relativeIndex = -1;

            if (e is View child && child.Positioning is Positioning.Relative)
                _relativeElements.Add(e);
            else
                _absoluteElements.Add(e);
        });
    }

    protected int _relativeIndex;
    /// <summary>
    /// 重新计算子元素
    /// </summary>
    public override void RecalculateChildren()
    {
        ClassifyElements();

        var innerPosition = _innerDimensions.Position();
        var rightBottom = innerPosition;

        for (int i = 0; i < _relativeElements.Count; i++)
        {
            UIElement e = _relativeElements[i];

            if (e is View view) view._relativeIndex = i;

            e.Recalculate();

            rightBottom.X =
                Math.Max(rightBottom.X, e.GetOuterDimensions().Right());
            rightBottom.Y =
                Math.Max(rightBottom.Y, e.GetOuterDimensions().Bottom());
        }

        var contentSize = rightBottom - innerPosition;
        if ((contentSize.X > 0 || contentSize.Y > 0) && Parent is View parent)
        {
            SecondRecalculate(parent, contentSize);
        }

        // 绝对定位
        _absoluteElements.ForEach(e => e.Recalculate());
    }

    /// <summary>
    /// 计算位置与偏移
    /// </summary>
    public virtual void SecondRecalculate(View parent, Vector2 contentSize)
    {
        var offset = new Vector2();

        if (contentSize.X > 0 && !SpecifyWidth)
        {
            _outerDimensions.Width =
                 contentSize.X + this.HMargin() + this.HPadding() + Border * 2;

            if (Positioning is Positioning.Absolute ||
                (Positioning is Positioning.Relative && parent.SpecifyWidth))
            {
                offset.X = -_outerDimensions.Width * HAlign;
            }
        }

        if (contentSize.Y > 0 && !SpecifyHeight)
        {
            _outerDimensions.Height =
                 contentSize.Y + this.VMargin() + this.VPadding() + Border * 2;

            if (Positioning is Positioning.Absolute ||
                (Positioning is Positioning.Relative && parent.SpecifyHeight))
            {
                offset.Y = -_outerDimensions.Height * VAlign;
            }
        }

        if (offset.X != 0)
        {
            if (offset.Y != 0)
                this.Offset(offset);
            else
                this.OffsetX(offset.X);
        }
        else if (offset.Y != 0)
        {
            this.OffsetY(offset.Y);
        }

        _dimensions = CalculateDimensions(_outerDimensions);
        _innerDimensions = CalculateInnerDimensions(_dimensions);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        LeftMouseButtonPressed = true;
        base.LeftMouseDown(evt);
    }

    public override void RightMouseDown(UIMouseEvent evt)
    {
        RightMouseButtonPressed = true;
        base.RightMouseDown(evt);
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        LeftMouseButtonPressed = false;
        base.LeftMouseUp(evt);
    }

    public override void RightMouseUp(UIMouseEvent evt)
    {
        RightMouseButtonPressed = false;
        base.RightMouseUp(evt);
    }

    public override bool ContainsPoint(Vector2 point) =>
        base.ContainsPoint(Vector2.Transform(point, Matrix.Invert(TransformMatrix)));

    #endregion
}