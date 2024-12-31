namespace SilkyUI.BasicElements;

/// <summary>
/// 所有 SilkyUI 元素的父类
/// </summary>
public partial class View : UIElement
{
    public View()
    {
        MaxWidth = MaxHeight = new StyleDimension(114514f, 0f);

        OnLeftMouseDown += (_, _) => LeftMousePressed = true;
        OnRightMouseDown += (_, _) => RightMousePressed = true;
        OnMiddleMouseDown += (_, _) => MiddleMousePressed = true;

        OnLeftMouseUp += (_, _) => LeftMousePressed = false;
        OnRightMouseUp += (_, _) => RightMousePressed = false;
        OnMiddleMouseUp += (_, _) => MiddleMousePressed = false;
    }

    protected virtual Vector2 GetStartingPosition() =>
        Parent is null ? Vector2.Zero : Parent._innerDimensions.Position();

    protected virtual Vector2 GetContainerSize()
    {
        // 无父元素
        if (Parent is null)
            return SilkyUIHelper.GetScreenScaledSize();

        var size = Parent._innerDimensions.Size();

        if (IsAbsolute || Parent is not View parent) return size;

        if (!parent.SpecifyWidth) size.X = 0;
        if (!parent.SpecifyHeight) size.Y = 0;

        return size;
    }

    protected virtual ElementBounds GetContainerBounds() => new(GetStartingPosition(), GetContainerSize());

    protected float RelativeLeft;
    protected float RelativeTop;

    protected float? OuterWidth;
    protected float? OuterHeight;
    protected float? InnerWidth;
    protected float? InnerHeight;

    protected virtual void RecalculateSize()
    {
        if (Parent is View parent)
        {
            if (SpecifyWidth)
            {
                var innerWidth = MathHelper.Clamp(Width.GetValue(parent.InnerWidth ?? 0),
                    MinWidth.GetValue(parent.InnerWidth ?? 0),
                    MaxWidth.GetValue(parent.InnerWidth ?? 0));

                InnerWidth = BoxSizing is not BoxSizing.ContentBox
                    ? MathHelper.Max(0, innerWidth - Border * 2 - this.HPadding())
                    : innerWidth;
            }

            if (SpecifyHeight)
            {
                var innerHeight = MathHelper.Clamp(Height.GetValue(parent.InnerHeight ?? 0),
                    MinHeight.GetValue(parent.InnerHeight ?? 0),
                    MaxHeight.GetValue(parent.InnerHeight ?? 0));
                InnerHeight = BoxSizing is not BoxSizing.ContentBox
                    ? MathHelper.Max(0, innerHeight - Border * 2 - this.VPadding())
                    : innerHeight;
            }
        }
        else
        {
            var size = Parent is null
                ? SilkyUIHelper.GetBasicBodyDimensions().Size()
                : Parent.GetInnerDimensions().Size();

            InnerWidth = MathHelper.Clamp(Width.GetValue(size.X),
                MinWidth.GetValue(size.X),
                MaxWidth.GetValue(size.X));

            InnerHeight = MathHelper.Clamp(Width.GetValue(size.Y),
                MinWidth.GetValue(size.Y),
                MaxWidth.GetValue(size.Y));
        }

        var children = Elements.OfType<View>().ToList();
        children.ForEach(child => child.RecalculateSize());

        OuterWidth = InnerWidth + this.HMargin() + Border * 2 + this.HPadding();
        OuterHeight = InnerHeight + this.VMargin() + Border * 2 + this.VPadding();
    }

    protected virtual void RecalculateByTop()
    {
        // RecalculateSize();
    }

    protected virtual void PostRecalculateByTop()
    {
        // ApplyLayoutOffset();
    }

    protected bool ParentIsCalculating() => Parent is View { Calculating: false };

    protected bool Calculating { get; set; }

    public override void Recalculate()
    {
        Calculating = true;
        try
        {
            if (!ParentIsCalculating()) RecalculateByTop();

            var parentDimensions = GetContainerBounds().ToCalculatedStyle();
            _outerDimensions = CalculateOuterDimensionsByParentDimensions(parentDimensions);
            _dimensions = CalculateDimensions(_outerDimensions);
            _innerDimensions = CalculateInnerDimensions(_dimensions);

            ClassifyElements();
            RecalculateChildren();

            if (Display is Display.Flexbox)
                CalculateFlexLayout();

            if (Parent is View parent && (!SpecifyWidth || !SpecifyHeight))
                FinalCalculate(parent);

            // 绝对定位
            AbsoluteElements.ForEach(element => element.Recalculate());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            if (!ParentIsCalculating()) PostRecalculateByTop();
        }

        Calculating = false;
    }

    #region CalculateOuterDimensions

    protected virtual float CalculateLeft(float parentLeft, float parentWidth)
    {
        return Left.GetValue(parentWidth) + parentLeft;
    }

    protected virtual float CalculateTop(float parentTop, float parentHeight)
    {
        return Top.GetValue(parentHeight) + parentTop;
    }

    protected virtual float CalculateWidth(float container)
    {
        return MathHelper.Clamp(Width.GetValue(container),
            MinWidth.GetValue(container),
            MaxWidth.GetValue(container));
    }

    protected virtual float CalculateHeight(float container)
    {
        return MathHelper.Clamp(Height.GetValue(container),
            MinHeight.GetValue(container),
            MaxHeight.GetValue(container));
    }

    protected virtual CalculatedStyle CalculateOuterDimensions(float left, float top, float width, float height)
    {
        return BoxSizing switch
        {
            BoxSizing.BorderBox => new CalculatedStyle(
                left, top,
                width + this.HMargin(),
                height + this.VMargin()),
            BoxSizing.ContentBox => new CalculatedStyle(
                left, top,
                width + this.HPadding() + this.HMargin() + Border * 2f,
                height + this.VPadding() + this.VMargin() + Border * 2f),
            _ => new CalculatedStyle()
        };
    }

    #endregion

    /// <summary>
    /// 计算属性，基于父元素尺寸<br/>
    /// 不使用 <see cref="UIElement.GetDimensionsBasedOnParentDimensions(CalculatedStyle)"/>
    /// </summary>
    protected virtual CalculatedStyle CalculateOuterDimensionsByParentDimensions(CalculatedStyle parentDimensions)
    {
        var left = CalculateLeft(parentDimensions.X, parentDimensions.Width);
        var top = CalculateTop(parentDimensions.Y, parentDimensions.Height);
        var width = SpecifyWidth ? CalculateWidth(parentDimensions.Width) : 0;
        var height = SpecifyHeight ? CalculateHeight(parentDimensions.Height) : 0;

        var outerDimensions = CalculateOuterDimensions(left, top, width, height);

        if (IsAbsolute)
        {
            outerDimensions.X += (parentDimensions.Width - outerDimensions.Width) * HAlign;
            outerDimensions.Y += (parentDimensions.Height - outerDimensions.Height) * VAlign;
        }
        else if (IsRelative && Parent is View parent)
        {
            if (parent.SpecifyWidth)
                outerDimensions.X += (parentDimensions.Width - outerDimensions.Width) * HAlign;
            if (parent.SpecifyHeight)
                outerDimensions.Y += (parentDimensions.Height - outerDimensions.Height) * VAlign;
        }

        return outerDimensions;
    }

    #region CalculateDimensions

    public CalculatedStyle CalculateDimensions(CalculatedStyle outerDimensions)
    {
        var dimensions = outerDimensions;
        dimensions.X += MarginLeft;
        dimensions.Y += MarginTop;
        dimensions.Width -= MarginLeft + MarginRight;
        dimensions.Height -= MarginTop + MarginBottom;
        return dimensions;
    }

    public CalculatedStyle CalculateInnerDimensions(CalculatedStyle dimensions)
    {
        var innerDimensions = dimensions;
        innerDimensions.X += PaddingLeft + Border;
        innerDimensions.Y += PaddingTop + Border;
        innerDimensions.Width -= this.HPadding() + Border * 2;
        innerDimensions.Height -= this.VPadding() + Border * 2;
        return innerDimensions;
    }

    #endregion

    /// <summary>
    /// 重新计算子元素
    /// </summary>
    public override void RecalculateChildren() =>
        FlowElements?.ForEach(element => element.Recalculate());

    protected virtual bool CalculateContentSize(out Vector2 content)
    {
        var start = _innerDimensions.Position();
        var end = FlowElements.Aggregate(start,
            (current, element) => Vector2.Max(current, element.GetOuterDimensions().RightBottom()));

        content = end - start;
        return content is { X: > 0 } or { Y: > 0 };
    }

    protected float CalculateOuterWidthByContent(Vector2 content) =>
        content.X + this.HMargin() + this.HPadding() + Border * 2;

    protected float CalculateOuterHeightByContent(Vector2 content) =>
        content.Y + this.VMargin() + this.VPadding() + Border * 2;

    protected virtual void FinalCalculate(View parent)
    {
        if (!CalculateContentSize(out var content)) return;

        var offset = new Vector2();

        // width: auto
        if (!SpecifyWidth)
        {
            _outerDimensions.Width = CalculateOuterWidthByContent(content);

            if (Position is Position.Absolute || (Position is Position.Relative && parent.SpecifyWidth))
                offset.X = -_outerDimensions.Width * HAlign;
        }

        // height: auto
        if (!SpecifyHeight)
        {
            _outerDimensions.Height = CalculateOuterHeightByContent(content);

            if (Position is Position.Absolute || (Position is Position.Relative && parent.SpecifyHeight))
                offset.Y = -_outerDimensions.Height * VAlign;
        }

        if (offset.X != 0)
            if (offset.Y != 0) this.DimensionsOffset(offset);
            else this.DimensionsOffsetX(offset.X);
        else if (offset.Y != 0)
            this.DimensionsOffsetY(offset.Y);

        _dimensions = CalculateDimensions(_outerDimensions);
        _innerDimensions = CalculateInnerDimensions(_dimensions);
    }

    /// <summary>
    /// 修正偏移
    /// </summary>
    protected Vector2 LayoutOffset;

    protected void ApplyLayoutOffset()
    {
        _outerDimensions.X += LayoutOffset.X;
        _outerDimensions.Y += LayoutOffset.Y;

        _dimensions.X += LayoutOffset.X;
        _dimensions.Y += LayoutOffset.Y;

        _innerDimensions.X += LayoutOffset.X;
        _innerDimensions.Y += LayoutOffset.Y;

        foreach (var child in Elements.OfType<View>())
        {
            child.LayoutOffset += LayoutOffset;
            child.ApplyLayoutOffset();
        }

        LayoutOffset = Vector2.Zero;
    }

    public virtual UIElement GetElementAtFromView(Vector2 point)
    {
        var children =
            GetChildrenByZIndex().OfType<View>().Where(el => !el.IgnoresMouseInteraction).Reverse().ToArray();

        if (OverflowHidden && !ContainsPoint(point)) return null;

        foreach (var child in children)
        {
            if (child.GetElementAt(point) is { } target) return target;
        }

        if (IgnoresMouseInteraction) return null;

        return ContainsPoint(point) ? this : null;
    }

    /// <summary>
    /// 判断点是否在元素内, 会计算<see cref="FinalMatrix"/>
    /// </summary>
    public override bool ContainsPoint(Vector2 point) =>
        base.ContainsPoint(Vector2.Transform(point, Matrix.Invert(FinalMatrix)));

    /// <summary>
    /// 鼠标悬停声音
    /// </summary>
    public SoundStyle? MouseOverSound { get; set; }

    public void UseMenuTickSoundForMouseOver() => MouseOverSound = SoundID.MenuTick;

    public override void MouseOver(UIMouseEvent evt)
    {
        if (MouseOverSound != null)
            SoundEngine.PlaySound(MouseOverSound);
        base.MouseOver(evt);
    }

    public virtual View AppendFromView(View child)
    {
        child.Remove();
        child.Parent = this;
        Elements.Add(child);
        Recalculate();
        return this;
    }
}