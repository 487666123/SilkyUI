using SilkyUI.Animation;

namespace SilkyUI.BasicComponents;

public class SUIScrollbar : View
{
    #region Basic Fields

    public event Action<Vector2> OnCurrentScrollPositionChanged;

    public virtual Vector2 CurrentScrollPosition
    {
        get => Vector2.Clamp(_currentScrollPosition, Vector2.Zero, GetScrollRange());
        set
        {
            _currentScrollPosition = Vector2.Clamp(value, Vector2.Zero, GetScrollRange());
            OnCurrentScrollPositionChanged?.Invoke(_currentScrollPosition);
        }
    }

    private Vector2 _currentScrollPosition;

    public float ScrollMultiplier = 1f;

    public void SetHScrollTarget(float value)
    {
        _scrollTarget.X = MathHelper.Clamp((_scrollTarget.X + value) * ScrollMultiplier, 0f, GetScrollRange().X);
    }

    public void SetVScrollTarget(float value)
    {
        _scrollTarget.Y = MathHelper.Clamp((_scrollTarget.Y + value) * ScrollMultiplier, 0f, GetScrollRange().Y);
    }

    public void SetScrollTarget(Vector2 value)
    {
        _scrollTarget = Vector2.Clamp((_scrollTarget + value) * ScrollMultiplier, Vector2.Zero, GetScrollRange());
    }

    /// <summary>
    /// 滚动条目标位置
    /// </summary>
    public Vector2 TargetScroll => Vector2.Clamp(_scrollTarget, Vector2.Zero, GetScrollRange());

    private Vector2 _scrollTarget;

    protected Vector2 OriginalScrollPosition;
    protected Vector2 LastTargetScrollPosition;

    public Color BarColor = Color.Black * 0.2f;
    public Color BarHoverColor = Color.Black * 0.3f;

    public bool IsBarSizeLimited = true;

    public Vector2 ContainerSize
    {
        get => Vector2.Max(Vector2.One, _scrollViewSize);
        set => _scrollViewSize = Vector2.Max(Vector2.One, value);
    }

    private Vector2 _scrollViewSize = Vector2.One;

    public Vector2 ChildrenSize
    {
        get => Vector2.Max(Vector2.One, _scrollableContentSize);
        set => _scrollableContentSize = Vector2.Max(Vector2.One, value);
    }

    private Vector2 _scrollableContentSize = Vector2.One;

    public bool AutomaticallyDisabled = true;

    // /// <summary>
    // /// <see cref="ChildrenSize"/> 宽度是否大于 <see cref="ContainerSize"/> 宽度<br/>
    // /// 如果小于等于，无论如何调节都无效，所以就是不可用
    // /// </summary>
    // public bool IsBeUsableH => !AutomaticallyDisabled || ChildrenSize.X > ContainerSize.X;
    //
    // /// <summary>
    // /// <see cref="ChildrenSize"/> 高度是否大于 <see cref="ContainerSize"/> 高度<br/>
    // /// 如果小于等于，无论如何调节都无效，所以就是不可用
    // /// </summary>
    // public bool IsBeUsableV => !AutomaticallyDisabled || ChildrenSize.Y > ContainerSize.Y;

    #endregion

    #region Basic Method

    #region Scroll Range

    public void SetScrollRange(Vector2 maskSize, Vector2 targetSize)
    {
        ContainerSize = maskSize;
        ChildrenSize = targetSize;
    }

    public void SetHScrollRange(float containerWidth, float childrenWidth)
    {
        ContainerSize = new Vector2(containerWidth, 1f);
        ChildrenSize = new Vector2(childrenWidth, 1f);
    }

    public void SetVScrollRange(float containerHeight, float childrenHeight)
    {
        ContainerSize = new Vector2(1f, containerHeight);
        ChildrenSize = new Vector2(1f, childrenHeight);
    }

    #endregion

    public Vector2 GetScrollRange()
    {
        var result = Vector2.Max(Vector2.Zero, ChildrenSize - ContainerSize);
        result.X = MathF.Round(result.X, 2);
        result.Y = MathF.Round(result.Y, 2);
        return result;
    }

    public Vector2 BarPosition => CurrentScrollPosition / ChildrenSize * GetInnerDimensions().Size();

    public Vector2 GetBarSize()
    {
        var innerWidth = GetInnerDimensions().Width;
        var innerHeight = GetInnerDimensions().Height;

        var barSize = GetInnerDimensions().Size() * (ContainerSize / ChildrenSize);

        if (!IsBarSizeLimited) return barSize;

        var min = new Vector2(Math.Min(innerWidth, innerHeight));
        barSize = Vector2.Clamp(barSize, min, GetInnerDimensions().Size());

        return barSize;
    }

    public Vector2 GetBarScreenPosition() => GetInnerDimensions().Position() + BarPosition;

    /// <summary>
    /// 直接设置滚动位置
    /// </summary>
    public void SetScrollPositionDirectly(Vector2 position)
    {
        position = Vector2.Clamp(position, Vector2.Zero, GetScrollRange());

        SetScrollTarget(position);
        CurrentScrollPosition = position;
        OriginalScrollPosition = position;
        LastTargetScrollPosition = position;
    }

    public void SetBarPositionDirectly(Vector2 barPosition) =>
        SetScrollPositionDirectly(ChildrenSize * barPosition / GetInnerDimensions().Size());

    public bool IsMouseOverScrollbar()
    {
        var focus = Main.MouseScreen;
        var barPos = GetBarScreenPosition();
        var barSize = GetBarSize();

        return focus.X > barPos.X && focus.Y > barPos.Y && focus.X < barPos.X + barSize.X &&
               focus.Y < barPos.Y + barSize.Y;
    }

    #endregion

    protected bool IsScrollbarDragging; // 滚动条拖动中
    protected Vector2 ScrollbarDragOffset; // 滚动条拖动偏移

    protected readonly AnimationTimer ScrollTimer = new(10f);

    #region Override Method

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);
        IsScrollbarDragging = true;
        ScrollbarDragOffset = Main.MouseScreen - GetBarScreenPosition();
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        base.LeftMouseUp(evt);
        IsScrollbarDragging = false;
    }

    #endregion

    public readonly RoundedRectangle ControlBar = new();
    public View TargetView { get; set; }

    public SUIScrollbar(View targetView)
    {
        DragIgnore = false;
        TargetView = targetView;
    }

    protected virtual void UpdateScrollPosition()
    {
        if (LastTargetScrollPosition != TargetScroll)
        {
            LastTargetScrollPosition = TargetScroll;
            OriginalScrollPosition = CurrentScrollPosition;
            ScrollTimer.StartForwardUpdateAndReset();
        }

        if (CurrentScrollPosition != TargetScroll)
        {
            CurrentScrollPosition = ScrollTimer.Lerp(OriginalScrollPosition, TargetScroll);
        }
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (IsScrollbarDragging)
        {
            SetBarPositionDirectly(Main.MouseScreen - ScrollbarDragOffset - GetInnerDimensions().Position());
        }

        UpdateScrollPosition();

        ScrollTimer.Update();

        base.DrawSelf(spriteBatch);

        DrawScrollbar();
    }

    protected virtual void DrawScrollbar()
    {
        var barSize = GetBarSize();
        var barPos = GetInnerDimensions().Position() + BarPosition;

        if (barSize is not { X: > 0, Y: > 0 }) return;

        var barBgColor = IsMouseOverScrollbar() || IsScrollbarDragging ? BarHoverColor : BarColor;

        ControlBar.CornerRadius = new Vector4(Math.Min(barSize.X, barSize.Y) / 2f);
        ControlBar.BgColor = barBgColor;
        ControlBar.Draw(barPos, barSize, false, FinalMatrix);
    }
}