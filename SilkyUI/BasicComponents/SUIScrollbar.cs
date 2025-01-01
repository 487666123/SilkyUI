using SilkyUI.Animation;

namespace SilkyUI.BasicComponents;

public class SUIScrollbar : View
{
    #region Basic Fields

    /// <summary>
    /// 滚动条当前位置
    /// </summary>
    public virtual Vector2 CurrentScrollPosition
    {
        get => Vector2.Clamp(_currentScrollPosition, Vector2.Zero, GetScrollRange());
        set => _currentScrollPosition = Vector2.Clamp(value, Vector2.Zero, GetScrollRange());
    }

    private Vector2 _currentScrollPosition;

    public float ScrollMultiplier = 1f;

    /// <summary>
    /// 滚动条目标位置
    /// </summary>
    public Vector2 TargetScrollPosition
    {
        get => Vector2.Clamp(_targetScrollPosition, Vector2.Zero, GetScrollRange());
        set => _targetScrollPosition = Vector2.Clamp(value * ScrollMultiplier, Vector2.Zero, GetScrollRange());
    }

    private Vector2 _targetScrollPosition;

    public Vector2 OriginalScrollPosition;
    public Vector2 LastTargetScrollPosition;

    public Color BarColor = Color.Black * 0.4f;
    public Color BarHoverColor = Color.Black * 0.5f;

    public bool IsBarSizeLimited = true;

    /// <summary>
    /// 若鼠标不在滚动条上，缩小滚动条内部部分宽度
    /// </summary>
    public bool ShrinkIfNotHovering = false;

    public Vector2 MaskSize
    {
        get => Vector2.Max(Vector2.One, _scrollViewSize);
        set => _scrollViewSize = Vector2.Max(Vector2.One, value);
    }

    private Vector2 _scrollViewSize = Vector2.One;

    public Vector2 TargetSize
    {
        get => Vector2.Max(Vector2.One, _scrollableContentSize);
        set => _scrollableContentSize = Vector2.Max(Vector2.One, value);
    }

    private Vector2 _scrollableContentSize = Vector2.One;

    public bool AutomaticallyDisabled = true;

    /// <summary>
    /// <see cref="TargetSize"/> 宽度是否大于 <see cref="MaskSize"/> 宽度<br/>
    /// 如果小于等于，无论如何调节都无效，所以就是不可用
    /// </summary>
    public bool IsBeUsableH => !AutomaticallyDisabled || TargetSize.X > MaskSize.X;

    /// <summary>
    /// <see cref="TargetSize"/> 高度是否大于 <see cref="MaskSize"/> 高度<br/>
    /// 如果小于等于，无论如何调节都无效，所以就是不可用
    /// </summary>
    public bool IsBeUsableV => !AutomaticallyDisabled || TargetSize.Y > MaskSize.Y;

    #endregion

    #region Basic Method

    public void SetArea(Vector2 maskSize, Vector2 targetSize)
    {
        MaskSize = maskSize;
        TargetSize = targetSize;
    }

    /// <summary>
    /// 获取滚动范围
    /// </summary>
    public Vector2 GetScrollRange()
    {
        var result = Vector2.Max(Vector2.Zero, TargetSize - MaskSize);
        result.X = MathF.Round(result.X, 2);
        result.Y = MathF.Round(result.Y, 2);
        return result;
    }

    public Vector2 BarPosition => CurrentScrollPosition / TargetSize * GetInnerDimensions().Size();

    public Vector2 GetBarSize()
    {
        var innerWidth = GetInnerDimensions().Width;
        var innerHeight = GetInnerDimensions().Height;

        var barSize = GetInnerDimensions().Size() * (MaskSize / TargetSize);

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

        CurrentScrollPosition = position;
        OriginalScrollPosition = position;
        TargetScrollPosition = position;
        LastTargetScrollPosition = position;
    }

    public void SetBarPositionDirectly(Vector2 barPosition) =>
        SetScrollPositionDirectly(TargetSize * barPosition / GetInnerDimensions().Size());

    public bool IsMouseOverScrollbar()
    {
        var focus = Main.MouseScreen;
        var barPos = GetBarScreenPosition();
        var barSize = GetBarSize();

        return focus.X > barPos.X && focus.Y > barPos.Y && focus.X < barPos.X + barSize.X &&
               focus.Y < barPos.Y + barSize.Y;
    }

    #endregion

    #region private static Fields

    protected readonly AnimationTimer ShrinkTimer = new(10f);
    protected bool IsScrollbarDragging; // 滚动条拖动中
    protected Vector2 ScrollbarDragOffset; // 滚动条拖动偏移

    protected readonly AnimationTimer ScrollTimer = new(10f);

    #endregion

    #region Override Method

    public override bool ContainsPoint(Vector2 point)
    {
        return (IsBeUsableH || IsBeUsableV) && base.ContainsPoint(point);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        if ((!IsBeUsableH && !IsBeUsableV) || !IsMouseOverScrollbar()) return;

        IsScrollbarDragging = true;
        ScrollbarDragOffset = Main.MouseScreen - GetBarScreenPosition();
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        base.LeftMouseUp(evt);

        IsScrollbarDragging = false;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        // if (!IsBeUsableH && !IsBeUsableV) return;
        if (IsScrollbarDragging)
        {
            SetBarPositionDirectly(Main.MouseScreen - ScrollbarDragOffset - GetInnerDimensions().Position());
        }

        UpdateScrollPosition();

        ScrollTimer.Update();
        ShrinkTimer.Update();

        base.DrawSelf(spriteBatch);

        DrawScrollbar();
    }

    public override void Update(GameTime gameTime)
    {
        // 这个不知放到if外面还是里面好，我就放外面了先
        base.Update(gameTime);

        if (IsBeUsableH || IsBeUsableV)
        {
            if (IsMouseHovering || IsScrollbarDragging)
                ShrinkTimer.StartReverseUpdate();
            else
                ShrinkTimer.StartForwardUpdate();
        }
    }

    #endregion

    public SUIScrollbar()
    {
        DragIgnore = false;
    }

    protected virtual void UpdateScrollPosition()
    {
        if (LastTargetScrollPosition != TargetScrollPosition)
        {
            LastTargetScrollPosition = TargetScrollPosition;
            OriginalScrollPosition = CurrentScrollPosition;
            ScrollTimer.StartForwardUpdateAndReset();
        }

        if (CurrentScrollPosition != TargetScrollPosition)
        {
            CurrentScrollPosition = ScrollTimer.Lerp(OriginalScrollPosition, TargetScrollPosition);
        }
    }

    protected virtual void DrawScrollbar()
    {
        var barSize = GetBarSize();
        var barPos = GetInnerDimensions().Position() + BarPosition;

        if (ShrinkIfNotHovering)
        {
            const float shrinkFactor = 0.4f;
            barPos.X += ShrinkTimer.Lerp(0, barSize.X * (1f - shrinkFactor));
            barSize.X = ShrinkTimer.Lerp(barSize.X, barSize.X * shrinkFactor);
        }

        if (barSize is not { X: > 0, Y: > 0 }) return;

        var barBgColor = IsMouseOverScrollbar() || IsScrollbarDragging ? BarHoverColor : BarColor;

        Bar.CornerRadius = new Vector4(Math.Min(barSize.X, barSize.Y) / 2f);
        Bar.BgColor = barBgColor;
        Bar.Draw(barPos, barSize, false, FinalMatrix);
    }

    public RoundedRectangle Bar = new();
}