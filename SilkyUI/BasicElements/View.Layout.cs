namespace SilkyUI.BasicElements;

public partial class View
{
    protected virtual Vector2 GetContainerSize()
    {
        if (Parent is null) return SilkyUIHelper.GetScreenScaledSize();

        var size = Parent._innerDimensions.Size();

        if (IsAbsolute || Parent is not View parent) return size;

        if (!parent.SpecifyWidth) size.X = 0;
        if (!parent.SpecifyHeight) size.Y = 0;

        return size;
    }

    protected void RecalculateSize(Vector2 containerSize)
    {
        var outerSize = GetOuterSizeByContainer(containerSize);

        _outerDimensions.Width = outerSize.X;
        _outerDimensions.Height = outerSize.Y;

        _dimensions.Width = _outerDimensions.Width - MarginLeft - MarginRight;
        _dimensions.Height = _outerDimensions.Height - MarginTop - MarginBottom;

        _innerDimensions.Width = _dimensions.Width - this.HPadding() - Border * 2;
        _innerDimensions.Height = _dimensions.Height - this.VPadding() - Border * 2;
    }

    protected virtual void LayingChildren()
    {
        switch (Display)
        {
            default:
            case Display.Flow:
                ReflowFlowLayout();
                break;
            case Display.Flexbox:
                ProcessFlexItems();
                ReflowFlexLayout();
                break;
        }
    }

    public override void Recalculate()
    {
        var containerSize = GetContainerSize();
        Position = new Vector2(Left.GetValue(containerSize.X), Top.GetValue(containerSize.Y));

        RecalculateSize(containerSize);

        ClassifyElements();

        RecalculateChildren();

        LayingChildren();

        AdaptingChildren();

        var offset = new Vector2();

        if (IsAbsolute)
        {
            offset.X = (containerSize.X - _outerDimensions.Width) * HAlign;
            offset.Y = (containerSize.Y - _outerDimensions.Height) * VAlign;
        }
        else if (IsRelative)
        {
            if (Parent is View parent)
            {
                if (parent.SpecifyWidth)
                    offset.X += (containerSize.X - _outerDimensions.Width) * HAlign;
                if (parent.SpecifyHeight)
                    offset.Y += (containerSize.Y - _outerDimensions.Height) * VAlign;
            }
        }

        Position += offset;

        AbsoluteElements.ForEach(element => element.Recalculate());
    }

    #region CalculateOuterDimensions

    protected virtual float GetWidthValue(float container)
    {
        return MathHelper.Clamp(Width.GetValue(container),
            MinWidth.GetValue(container),
            MaxWidth.GetValue(container));
    }

    protected virtual float GetHeightValue(float container)
    {
        return MathHelper.Clamp(Height.GetValue(container),
            MinHeight.GetValue(container),
            MaxHeight.GetValue(container));
    }

    protected virtual Vector2 CalculateOuterSize(float width, float height)
    {
        return BoxSizing switch
        {
            BoxSizing.BorderBox => new Vector2(
                width + this.HMargin(),
                height + this.VMargin()),
            BoxSizing.ContentBox => new Vector2(
                width + this.HPadding() + this.HMargin() + Border * 2f,
                height + this.VPadding() + this.VMargin() + Border * 2f),
            _ => new Vector2(
                width + this.HMargin(),
                height + this.VMargin())
        };
    }

    #endregion

    /// <summary>
    /// 计算属性，基于父元素尺寸<br/>
    /// 不使用 <see cref="UIElement.GetDimensionsBasedOnParentDimensions(CalculatedStyle)"/>
    /// </summary>
    protected virtual Vector2 GetOuterSizeByContainer(Vector2 container)
    {
        var width = SpecifyWidth ? GetWidthValue(container.X) : 0;
        var height = SpecifyHeight ? GetHeightValue(container.Y) : 0;

        return CalculateOuterSize(width, height);
    }

    public override void RecalculateChildren() => FlowElements?.ForEach(element => element.Recalculate());

    protected virtual bool GetContentSize(out Vector2 content)
    {
        switch (Display)
        {
            default:
            case Display.Flow:
                content = GetFlowSize();
                break;
            case Display.Flexbox:
                content = GetFlexboxSize();
                break;
        }

        return content is { X: > 0 } or { Y: > 0 };
    }

    public Vector2 GetFlowSize() =>
        new(0f, FlowElements.Sum(element => element._outerDimensions.Height + Gap.X) - Gap.X);

    protected float OuterWidthByContent(float width) =>
        width + this.HMargin() + this.HPadding() + Border * 2;

    protected float OuterHeightByContent(float height) =>
        height + this.VMargin() + this.VPadding() + Border * 2;

    protected virtual void AdaptingChildren()
    {
        if (SpecifyWidth && SpecifyHeight || !GetContentSize(out var content)) return;

        if (!SpecifyWidth)
        {
            _outerDimensions.Width = OuterWidthByContent(content.X);
            _dimensions.Width = _outerDimensions.Width - MarginLeft - MarginRight;
            _innerDimensions.Width -= this.HPadding() + Border * 2;
        }

        if (!SpecifyHeight)
        {
            _outerDimensions.Height = OuterHeightByContent(content.Y);
            _dimensions.Height = _outerDimensions.Height - MarginTop - MarginBottom;
            _innerDimensions.Height -= this.VPadding() + Border * 2;
        }
    }

    protected bool PositionChanged = true;
    private Vector2 _position;

    protected Vector2 Position
    {
        get => _position;
        set
        {
            if (_position == value)
                return;
            _position = value;
            PositionChanged = true;
        }
    }

    private Vector2 _scrollPosition;

    public Vector2 ScrollPosition
    {
        get => _scrollPosition;
        set
        {
            if (_scrollPosition == value)
                return;
            _scrollPosition = value;
            PositionChanged = true;
        }
    }

    private void TrackPositionChange()
    {
        if (PositionChanged) ApplyPosition(GetStart());
        foreach (var child in Elements.OfType<View>())
            child.TrackPositionChange();
    }

    public Vector2 GetStart()
    {
        if (Parent is not { } uie) return Vector2.Zero;
        if (uie is not View view) return uie._innerDimensions.Position();
        return view._innerDimensions.Position() + view.ScrollPosition;
    }

    public void ApplyPosition(Vector2 start)
    {
        _outerDimensions.X = start.X + Position.X;
        _outerDimensions.Y = start.Y + Position.Y;

        _dimensions.X = _outerDimensions.X + MarginLeft;
        _dimensions.Y = _outerDimensions.Y + MarginTop;

        _innerDimensions.X = _dimensions.X + Border + PaddingLeft;
        _innerDimensions.Y = _dimensions.Y + Border + PaddingTop;

        foreach (var child in Elements.OfType<View>())
            child.ApplyPosition(_innerDimensions.Position() + ScrollPosition);
        PositionChanged = false;
    }
}