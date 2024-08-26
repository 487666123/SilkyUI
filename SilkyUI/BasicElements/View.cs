namespace SilkyUI.BasicElements;

public partial class View : UIElement
{
    public View()
    {
        MaxWidth = MaxHeight = new StyleDimension(114514f, 0f);
    }

    #region Methods
    public CalculatedStyle GetParentDimensions()
    {
        if (Parent is null)
            return UserInterface.ActiveInstance.GetDimensions();

        CalculatedStyle parentDimensions = Parent.GetInnerDimensions();

        // 相对定位, 若父元素明确 [没有指定] 大小, 视为父元素没有大小 (因为需要其称其父元素)
        // 绝对定位, 一定需要父元素大小 (因为不会撑起父元素)
        if (Parent is View parent && Positioning is Positioning.Relative)
        {
            var previousElement = this.PreviousElement();
            if (previousElement != null)
            {
                var previousOuterDimensions = previousElement.GetOuterDimensions();
                if (parent.FlowDirection is FlowDirection.Column)
                    parentDimensions.Y = previousOuterDimensions.Bottom();
                else
                    parentDimensions.X = previousOuterDimensions.Right();
            }

            if (!parent.SpecifyWidth)
                parentDimensions.Width = 0;

            if (!parent.SpecifyHeight)
                parentDimensions.Height = 0;
        }

        return parentDimensions;
    }

    public override void Recalculate()
    {
        _outerDimensions = CalculateDimensionsByParentDimensions(GetParentDimensions());
        _dimensions = CalculateDimensions(_outerDimensions);
        _innerDimensions = CalculateInnerDimensions(_dimensions);

        RecalculateChildren();
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

    /// <summary>
    /// 重新计算子元素
    /// </summary>
    public override void RecalculateChildren()
    {
        // 绝对定位, 即脱离标准文档留流
        List<UIElement> absoluteChildren = [];

        var leftTop = _innerDimensions.Position();
        var rightBottom = leftTop;

        // 标准文档
        foreach (UIElement element in Elements)
        {
            if (element is View view && view.Positioning is Positioning.Relative)
            {
                element.Recalculate();

                var right = element.GetOuterDimensions().Right();
                var bottom = element.GetOuterDimensions().Bottom();

                if (right > rightBottom.X)
                    rightBottom.X = right;

                if (bottom > rightBottom.Y)
                    rightBottom.Y = bottom;

                continue;
            }

            absoluteChildren.Add(element);
        }

        var contentSize = rightBottom - leftTop;
        if (contentSize.X > 0 || contentSize.Y > 0)
        {
            SecondaryRecalculate(contentSize);
        }

        // 绝对定位
        foreach (UIElement element in absoluteChildren)
        {
            element.Recalculate();
        }
    }

    /// <summary>
    /// 计算位置与偏移
    /// </summary>
    public virtual void SecondaryRecalculate(Vector2 contentSize)
    {
        if (Parent is not View view)
            return;

        var offset = new Vector2();

        if (contentSize.X > 0 && !SpecifyWidth)
        {
            _outerDimensions.Width =
                 contentSize.X + this.HMargin() + this.HPadding() + Border * 2;

            if (Positioning is Positioning.Absolute ||
                (Positioning is Positioning.Relative && view.SpecifyWidth))
            {
                offset.X = -_outerDimensions.Width * HAlign;
            }
        }

        if (contentSize.Y > 0 && !SpecifyHeight)
        {
            _outerDimensions.Height =
                 contentSize.Y + this.VMargin() + this.VPadding() + Border * 2;

            if (Positioning is Positioning.Absolute ||
                (Positioning is Positioning.Relative && view.SpecifyHeight))
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

    /// <summary>
    /// 计算属性，基于父元素尺寸<br/>
    /// 不使用 <see cref="UIElement.GetDimensionsBasedOnParentDimensions(CalculatedStyle)"/>
    /// </summary>
    public virtual CalculatedStyle CalculateDimensionsByParentDimensions(CalculatedStyle parentDimensions)
    {
        var minWidth = MinWidth.GetValue(parentDimensions.Width);
        var maxWidth = MaxWidth.GetValue(parentDimensions.Width);
        var minHeight = MinHeight.GetValue(parentDimensions.Height);
        var maxHeight = MaxHeight.GetValue(parentDimensions.Height);

        var rX = Left.GetValue(parentDimensions.Width) + parentDimensions.X;
        var rY = Top.GetValue(parentDimensions.Height) + parentDimensions.Y;
        var rWidth = SpecifyWidth ? MathHelper.Clamp(Width.GetValue(parentDimensions.Width), minWidth, maxWidth) : 0;
        var rHeight = SpecifyHeight ? MathHelper.Clamp(Height.GetValue(parentDimensions.Height), minHeight, maxHeight) : 0;

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

        if (Positioning is Positioning.Absolute)
        {
            result.X += (parentDimensions.Width - result.Width) * HAlign;
            result.Y += (parentDimensions.Height - result.Height) * VAlign;
        }
        else if (Positioning is Positioning.Relative && Parent is View view)
        {
            if (view.SpecifyWidth)
                result.X += (parentDimensions.Width - result.Width) * HAlign;
            if (view.SpecifyHeight)
                result.Y += (parentDimensions.Height - result.Height) * VAlign;
        }

        return result;
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