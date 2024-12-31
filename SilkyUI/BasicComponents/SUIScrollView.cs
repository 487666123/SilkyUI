using SilkyUI.Animation;

namespace SilkyUI.BasicComponents;

/// <summary>
/// 滚动方向
/// </summary>
public enum ScrollDirection
{
    Horizontal,
    Vertical
}

/// <summary>
/// 为了更方便使用滚动列表而设计，支持横向滚动和纵向滚动 <br/>
/// 了解工作逻辑请查看 <see cref="FixedSizeMode"/> <see cref="Update(GameTime)"/> 两处代码
/// </summary>
public class SUIScrollView : View
{
    public readonly ScrollDirection ScrollDirection;

    /// <summary>
    /// 固定大小
    /// </summary>
    public bool FixedSizeMode
    {
        get => _fixedSizeMode;
        set
        {
            _fixedSizeMode = value;

            if (_fixedSizeMode)
            {
                switch (ScrollDirection)
                {
                    default:
                    case ScrollDirection.Horizontal:
                        SpecifyHeight = true;

                        MaskView.SpecifyHeight = false;
                        MaskView.Width.Percent = 1f;

                        ListView.SpecifyHeight = true;
                        break;
                    case ScrollDirection.Vertical:
                        SpecifyWidth = true;

                        MaskView.SpecifyWidth = true;
                        MaskView.Width.Percent = 1f;

                        ListView.SpecifyWidth = true;
                        break;
                }

                ListView.Width.Percent = 1f;
            }
            else
            {
                switch (ScrollDirection)
                {
                    default:
                    case ScrollDirection.Horizontal:
                        SpecifyHeight = false;

                        MaskView.SpecifyHeight = false;
                        MaskView.Width.Percent = 0f;

                        ListView.SpecifyHeight = false;
                        break;
                    case ScrollDirection.Vertical:
                        SpecifyWidth = false;

                        MaskView.SpecifyWidth = false;
                        MaskView.Width.Percent = 0f;

                        ListView.SpecifyWidth = false;
                        break;
                }

                ListView.Width.Percent = 0f;
            }

            Recalculate();
        }
    }

    private bool _fixedSizeMode;

    /// <summary>
    /// 滚动条是否常驻
    /// </summary>
    public bool ScrollbarPermanent;

    /// <summary>
    /// 蒙版
    /// </summary>
    public View MaskView { get; } = new();

    /// <summary>
    /// 列表
    /// </summary>
    public View ListView { get; } = new();

    /// <summary>
    /// 滚动条
    /// </summary>
    public SUIScrollbar ScrollBar { get; } = new();

    /// <param name="scrollDirection">滚动朝向</param>
    /// <param name="fixedSizeMode">固定大小</param>
    public SUIScrollView(ScrollDirection scrollDirection, bool fixedSizeMode = true)
    {
        #region Mask ListView 蒙版 列表

        ScrollDirection = scrollDirection;
        FixedSizeMode = fixedSizeMode;

        // MaskView.BgColor = Color.White * 0.5f;
        MaskView.OverflowHidden = true;
        MaskView.SetSize(0, 0, 1f, 1f);
        MaskView.Join(this);

        switch (ScrollDirection)
        {
            case ScrollDirection.Horizontal:
                ListView.Height.Percent = 1f;
                ListView.SpecifyWidth = false;
                break;
            default:
            case ScrollDirection.Vertical:
                ListView.Width.Percent = 1f;
                ListView.SpecifyHeight = false;
                break;
        }

        ListView.HideFullyOverflowedElements = true;
        ListView.Join(MaskView);

        #endregion

        #region Scrollbar 滚动条

        ScrollBar.BgColor = Color.Transparent;
        ScrollBar.BorderColor = Color.Transparent;

        switch (ScrollDirection)
        {
            case ScrollDirection.Horizontal:
                ScrollBar.SetSize(0f, 8f, 1f, 0f);

                ScrollBar.OnUpdate += _ =>
                {
                    var maskSize = new Vector2(MaskView.GetInnerDimensions().Width, 1f);
                    var targetSize = new Vector2(ListView.Children.Any() ? ListView.Width.Pixels : 0, 1f);

                    ScrollBar.SetSizeForMaskAndTarget(maskSize, targetSize);
                };
                break;
            default:
            case ScrollDirection.Vertical:
                ScrollBar.SetSize(8f, 0f, 0f, 1f);

                ScrollBar.OnUpdate += _ =>
                {
                    var maskSize = new Vector2(1f, MaskView.GetInnerDimensions().Height);
                    var targetSize = new Vector2(1f, ListView.Children.Any() ? ListView.Height.Pixels : 0);

                    ScrollBar.SetSizeForMaskAndTarget(maskSize, targetSize);
                };
                break;
        }

        ScrollBar.Join(this);

        #endregion
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        switch (ScrollDirection)
        {
            default:
            case ScrollDirection.Horizontal:
                ScrollBar.TargetScrollPosition -= new Vector2(evt.ScrollWheelValue, 0f);
                break;
            case ScrollDirection.Vertical:
                ScrollBar.TargetScrollPosition -= new Vector2(0f, evt.ScrollWheelValue);
                break;
        }

        base.ScrollWheel(evt);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        var recalculate = false;

        switch (ScrollDirection)
        {
            default:
            case ScrollDirection.Horizontal:
                // 滚动条位置对准
                if (ListView.Left.Pixels != -ScrollBar.CurrentScrollPosition.X)
                {
                    ListView.Left.Pixels = -ScrollBar.CurrentScrollPosition.X;
                    recalculate = true;
                }

                break;
            case ScrollDirection.Vertical:
                // 滚动条位置对准
                if (ListView.Top.Pixels != -ScrollBar.CurrentScrollPosition.Y)
                {
                    ListView.Top.Pixels = -ScrollBar.CurrentScrollPosition.Y;
                    recalculate = true;
                }

                break;
        }

        if (FixedSizeMode)
        {
            switch (ScrollDirection)
            {
                default:
                case ScrollDirection.Horizontal:
                    // 设置正确的 蒙版 高度
                    if (ScrollBar.IsBeUsableH)
                    {
                        if (MaskView.Height.Pixels != -(ScrollBar.Height.Pixels + ScrollBar.Spacing.Y))
                        {
                            MaskView.Height.Pixels = -ScrollBar.Height.Pixels - ScrollBar.Spacing.Y;
                            recalculate = true;
                        }
                    }
                    else if (MaskView.Height.Pixels != 0)
                    {
                        MaskView.Height.Pixels = 0f;
                        recalculate = true;
                    }

                    break;
                case ScrollDirection.Vertical:
                    // 设置正确的 蒙版 宽度
                    if (ScrollBar.IsBeUsableV)
                    {
                        if (MaskView.Width.Pixels != -(ScrollBar.Width.Pixels + ScrollBar.Spacing.X))
                        {
                            _widthTimer.StartForwardUpdate();

                            MaskView.Width.Pixels =
                                _widthTimer.Lerp(0, -(ScrollBar.Width.Pixels + ScrollBar.Spacing.X));
                            recalculate = true;
                        }
                    }
                    else if (MaskView.Width.Pixels != 0)
                    {
                        _widthTimer.StartReverseUpdate();

                        MaskView.Width.Pixels =
                            _widthTimer.Lerp(0, -(ScrollBar.Width.Pixels + ScrollBar.Spacing.X));
                        recalculate = true;
                    }

                    break;
            }
        }

        if (recalculate) Recalculate();
    }

    protected override void UpdateAnimationTimer()
    {
        _widthTimer.Update();
        _heightTimer.Update();

        base.UpdateAnimationTimer();
    }

    private readonly AnimationTimer _widthTimer = new();
    private readonly AnimationTimer _heightTimer = new();
}