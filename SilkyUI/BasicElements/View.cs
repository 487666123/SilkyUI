namespace SilkyUI.BasicElements;

/// <summary>
/// 简单说明: 2024.8.26 <br/>
/// 只有 View 能用, 别的都不建议用. <br/>
/// 使用固定宽高必须设置 <see cref="SpecifyWidth"/> <see cref="SpecifyHeight"/> 为 true <br/>
/// 使用这套系统所有 UI 元素都必须基于 <see cref="View"/> <br/>
/// </summary>
public partial class View
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

    protected virtual Vector2 GetStartingPosition()
    {
        return Parent is null ? Vector2.Zero : Parent._innerDimensions.Position();
    }

    protected virtual Vector2 GetContainerSize()
    {
        if (Parent is null)
            return SilkyUIHelper.GetScreenScaledSize();

        if (Parent is not View parent)
            return Parent._innerDimensions.Size();

        var size = parent._innerDimensions.Size();

        // 可能是负数, 所以要设为 0
        if (!parent.SpecifyWidth) size.X = 0;
        if (!parent.SpecifyHeight) size.Y = 0;

        return size;
    }

    protected virtual UIBounds GetContainerBounds()
    {
        return new UIBounds(GetStartingPosition(), GetContainerSize());
    }

    protected bool ParentIsCalculating() => Parent is View { Calculating: false };

    protected bool Calculating { get; set; }

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

        var children = Children.OfType<View>().ToList();
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

    public override void Recalculate()
    {
        Calculating = true;
        try
        {
            if (!ParentIsCalculating())
                RecalculateByTop();

            ClassifyElements();

            var parentDimensions = GetContainerBounds().ToCalculatedStyle();

            _outerDimensions = CalculateOuterDimensionsByParentDimensions(parentDimensions);
            _dimensions = CalculateDimensions(_outerDimensions);
            _innerDimensions = CalculateInnerDimensions(_dimensions);

            RecalculateChildren();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            if (Calculating)
                PostRecalculateByTop();
        }

        Calculating = false;
    }

    #region CalculateOuterDimensions

    protected float CalculateLeft(float parentLeft, float parentWidth)
    {
        return Left.GetValue(parentWidth) + parentLeft;
    }

    protected float CalculateTop(float parentTop, float parentHeight)
    {
        return Top.GetValue(parentHeight) + parentTop;
    }

    protected float CalculateWidth(float container)
    {
        return MathHelper.Clamp(Width.GetValue(container),
            MinWidth.GetValue(container),
            MaxWidth.GetValue(container));
    }

    protected float CalculateHeight(float container)
    {
        return MathHelper.Clamp(Height.GetValue(container),
            MinHeight.GetValue(container),
            MaxHeight.GetValue(container));
    }

    protected CalculatedStyle CalculateOuterDimensions(float left, float top, float width, float height)
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
    public virtual CalculatedStyle CalculateOuterDimensionsByParentDimensions(CalculatedStyle parentDimensions)
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
        else if (IsRelative && Parent is View view)
        {
            if (view.SpecifyWidth)
                outerDimensions.X += (parentDimensions.Width - outerDimensions.Width) * HAlign;
            if (view.SpecifyHeight)
                outerDimensions.Y += (parentDimensions.Height - outerDimensions.Height) * VAlign;
        }

        // if (IsRelativePosition && Parent is View { Display: Display.InlineBlock } parent)
        // {
        //     var preElement = this.PreviousRelativeElement();
        //     var preElementRightBottom = preElement._outerDimensions.RightBottom();
        //
        //     if (parent.SpecifyWidth)
        //     {
        //         if (preElementRightBottom.X + result.Width > parentDimensions.Right())
        //         {
        //             result.X = parentDimensions.X;
        //             result.Y = preElementRightBottom.Y;
        //         }
        //         else
        //         {
        //             result.X = (result.X - parentDimensions.X) + preElementRightBottom.X;
        //         }
        //     }
        //     else
        //     {
        //         result.X = preElementRightBottom.X +
        //     }
        // }

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
        FlowElements.ForEach(element => element.Recalculate());

        if (Display is Display.Flexbox)
            CalculateFlexLayout();

        if (Parent is View parent && (!SpecifyWidth || !SpecifyHeight))
            AgainRecalculate(parent);

        // 绝对定位
        AbsoluteElements.ForEach(element => element.Recalculate());
    }

    protected bool CalculateContentSize(out Vector2 content)
    {
        var start = _innerDimensions.Position();
        var end = FlowElements.Aggregate(start,
            (current, element) => Vector2.Max(current, element.GetOuterDimensions().RightBottom()));

        content = end - start;
        return content is { X: > 0 } or { Y: > 0 };
    }

    /// <summary>
    /// 计算位置与偏移
    /// </summary>
    protected virtual void AgainRecalculate(View parent)
    {
        if (!CalculateContentSize(out var content)) return;

        var offset = new Vector2();

        // 宽度: auto
        if (!SpecifyWidth)
        {
            _outerDimensions.Width = content.X + this.HMargin() + this.HPadding() + Border * 2;

            if (Position is Position.Absolute ||
                (Position is Position.Relative && parent.SpecifyWidth))
                offset.X = -_outerDimensions.Width * HAlign;
        }

        // 高度: auto
        if (!SpecifyHeight)
        {
            _outerDimensions.Height = content.Y + this.VMargin() + this.VPadding() + Border * 2;

            if (Position is Position.Absolute ||
                (Position is Position.Relative && parent.SpecifyHeight))
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

        foreach (var child in Children.OfType<View>())
        {
            child.LayoutOffset += LayoutOffset;
            child.ApplyLayoutOffset();
        }

        LayoutOffset = Vector2.Zero;
    }

    public virtual UIElement GetElementAtFromView(Vector2 point)
    {
        var children =
            Elements.OfType<View>()
                .Where(el => !el.IgnoresMouseInteraction).Reverse().ToArray();

        if (OverflowHidden && !ContainsPoint(point)) return null;

        foreach (var child in children)
        {
            var target = child.GetElementAt(point);
            if (target is not null)
                return target;
        }

        if (IgnoresMouseInteraction)
            return null;

        return ContainsPoint(point) ? this : null;

        // for (var index = Elements.Count - 1; index >= 0; --index)
        // {
        //     var element = Elements[index];
        //     if (element.IgnoresMouseInteraction || !element.ContainsPoint(point)) continue;
        //     target = element;
        //     break;
        // }
        //
        // if (target != null)
        //     return target.GetElementAt(point);
        // if (IgnoresMouseInteraction)
        //     return null;
        // return ContainsPoint(point) ? this : null;
    }

    /// <summary>
    /// 判断点是否在元素内, 会计算<see cref="FinalMatrix"/>
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public override bool ContainsPoint(Vector2 point) =>
        base.ContainsPoint(Vector2.Transform(point, Matrix.Invert(FinalMatrix)));

    public SoundStyle? MouseOverSound { get; set; }
    public void UseMenuTickSoundForMouseOver() => MouseOverSound = SoundID.MenuTick;

    public override void MouseOver(UIMouseEvent evt)
    {
        if (MouseOverSound is not null)
            SoundEngine.PlaySound(MouseOverSound);
        base.MouseOver(evt);
    }

    public delegate bool OnAppendEventHandler(View parent, View child);

    public event OnAppendEventHandler OnViewAppend;

    public virtual View ViewAppend(View child)
    {
        OnViewAppend?.Invoke(this, child);
        child.Remove();
        child.Parent = this;
        Elements.Add(child);
        child.Recalculate();
        return this;
    }
}