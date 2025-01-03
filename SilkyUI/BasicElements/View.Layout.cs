using SilkyUI.Core;

namespace SilkyUI.BasicElements;

public enum Display
{
    /// <summary>
    /// 对 Update 无效, 因为有 event OnUpdate
    /// </summary>
    None,
    Flow,
    Flexbox,
}

public partial class View
{
    public Display Display { get; set; } = Display.Flow;

    /// <summary> 盒子模型计算方式 </summary>
    public BoxSizing BoxSizing { get; set; } = BoxSizing.BorderBox;

    /// <summary> 元素定位 </summary>
    public Positioning Positioning { get; set; } = Positioning.Relative;

    /// <summary> 文档流元素 </summary>
    protected readonly List<View> FlowElements = [];

    /// <summary> 非文档流元素 </summary>
    protected readonly List<UIElement> AbsoluteElements = [];

    protected virtual Vector2 GetContainer() =>
        Parent is null ? SilkyUIHelper.GetScreenScaledSize() : Parent._innerDimensions.Size();

    protected Vector2 MinSize;
    protected Vector2 MaxSize;

    protected void RecalculateSize(Vector2 containerSize)
    {
        MinSize = new Vector2(MinWidth.GetValue(containerSize.X), MinHeight.GetValue(containerSize.Y));
        MaxSize = new Vector2(MaxWidth.GetValue(containerSize.X), MaxHeight.GetValue(containerSize.Y));

        var width = SpecifyWidth ? MathHelper.Clamp(Width.GetValue(containerSize.X), MinSize.X, MaxSize.X) : 0;
        var height = SpecifyHeight ? MathHelper.Clamp(Height.GetValue(containerSize.Y), MinSize.Y, MaxSize.Y) : 0;

        var outerSize = GetOuterSize(width, height);

        _outerDimensions.Width = outerSize.X;
        _outerDimensions.Height = outerSize.Y;

        _dimensions.Width = _outerDimensions.Width - MarginLeft - MarginRight;
        _dimensions.Height = _outerDimensions.Height - MarginTop - MarginBottom;

        _innerDimensions.Width = _dimensions.Width - this.HPadding() - Border * 2;
        _innerDimensions.Height = _dimensions.Height - this.VPadding() - Border * 2;
    }

    /// <summary> 重排 </summary>
    protected virtual void Reflow()
    {
        switch (Display)
        {
            default:
            case Display.Flow:
                FlowArrange();
                break;
            case Display.Flexbox:
                OrganizingFlexboxItems();
                FlexboxArrange();
                break;
            case Display.None:
                break;
        }
    }

    public override void Recalculate()
    {
        var container = GetContainer();
        Position = new Vector2(Left.GetValue(container.X), Top.GetValue(container.Y));

        RecalculateSize(container);

        ClassifyElements();

        RecalculateChildren();

        Reflow();

        AdaptingContent();

        #region HAlign VAlign

        switch (Positioning)
        {
            default:
            case Positioning.Absolute:
                Position += (container - _outerDimensions.Size()) * new Vector2(HAlign, VAlign);
                break;
            case Positioning.Sticky:
            case Positioning.Relative:
            {
                if (Parent is not View parent) break;
                var offset = Vector2.Zero;
                if (parent.SpecifyWidth)
                    offset.X += (container.X - _outerDimensions.Width) * HAlign;
                if (parent.SpecifyHeight)
                    offset.Y += (container.Y - _outerDimensions.Height) * VAlign;
                Position += offset;
                break;
            }
        }

        #endregion

        AbsoluteElements.ForEach(element => element.Recalculate());
    }

    #region CalculateOuterDimensions

    protected virtual Vector2 GetOuterSize(float width, float height)
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

    public override void RecalculateChildren() => FlowElements?.ForEach(element => element.Recalculate());

    public virtual Vector2 GetContentSize()
    {
        switch (Display)
        {
            default:
            case Display.Flow:
                return GetFlowSize();
            case Display.Flexbox:
                return GetFlexboxSize();
            case Display.None:
                return Vector2.Zero;
        }
    }

    protected virtual void AdaptingContent()
    {
        if (SpecifyWidth && SpecifyHeight) return;
        var content = GetContentSize();

        if (!SpecifyWidth)
        {
            _dimensions.Width = content.X + PaddingLeft + PaddingRight + Border * 2;
            _dimensions.Width = MathHelper.Clamp(_dimensions.Width, MinSize.X, MaxSize.X);
            _outerDimensions.Width = _dimensions.Width + MarginLeft + MarginRight;
            _innerDimensions.Width = content.X;
        }

        if (SpecifyHeight) return;
        _dimensions.Height = content.Y + PaddingTop + PaddingBottom + Border * 2;
        _dimensions.Height = MathHelper.Clamp(_dimensions.Height, MinSize.Y, MaxSize.Y);
        _outerDimensions.Height = _dimensions.Height + MarginTop + MarginBottom;
        _innerDimensions.Height = content.Y;
    }

    #region Apply Position ScrollPosition Offset

    protected bool PositionChanged = true;

    private Vector2 _position;
    private Vector2 _offset;
    private Vector2 _scrollPosition;

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

    protected Vector2 Offset
    {
        get => _offset;
        set
        {
            if (_offset == value)
                return;
            _offset = value;
            PositionChanged = true;
        }
    }

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

    public Vector2 GetStartPoint() =>
        Parent is not { } uie ? Vector2.Zero : uie._innerDimensions.Position();

    public Vector2 GetParentScrollPosition() =>
        Parent is View view ? view.ScrollPosition : Vector2.Zero;

    private void TrackPositionChange()
    {
        if (PositionChanged) ApplyPosition(GetStartPoint(), GetContainer(), GetParentScrollPosition());
        foreach (var child in Elements.OfType<View>())
            child.TrackPositionChange();
    }

    public StickyType StickyType { get; set; } = StickyType.Top;
    public Vector4 Sticky { get; set; }

    public void ApplyPosition(Vector2 start, Vector2 container, Vector2 scroll)
    {
        _outerDimensions.X = start.X + scroll.X + Position.X + Offset.X;
        _outerDimensions.Y = start.Y + scroll.Y + Position.Y + Offset.Y;

        if (Positioning is Positioning.Sticky)
        {
            switch (StickyType)
            {
                case StickyType.Left:
                    _outerDimensions.X = Math.Max(_outerDimensions.X, start.X + Sticky.X);
                    break;
                default:
                case StickyType.Top:
                    _outerDimensions.Y = Math.Max(_outerDimensions.Y, start.Y + Sticky.Y);
                    break;
                case StickyType.Right:
                    _outerDimensions.X = Math.Min(_outerDimensions.X,
                        start.X + container.X - _outerDimensions.Width - Sticky.Z);
                    break;
                case StickyType.Bottom:
                    _outerDimensions.Y = Math.Min(_outerDimensions.Y,
                        start.Y + container.Y - _outerDimensions.Height - Sticky.W);
                    break;
            }
        }

        _dimensions.X = _outerDimensions.X + MarginLeft;
        _dimensions.Y = _outerDimensions.Y + MarginTop;

        _innerDimensions.X = _dimensions.X + Border + PaddingLeft;
        _innerDimensions.Y = _dimensions.Y + Border + PaddingTop;

        foreach (var child in Elements.OfType<View>())
            child.ApplyPosition(_innerDimensions.Position(), _innerDimensions.Size(), ScrollPosition);
        PositionChanged = false;
    }

    #endregion
}