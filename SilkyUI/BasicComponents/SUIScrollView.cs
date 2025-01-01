using SilkyUI.Animation;

namespace SilkyUI.BasicComponents;

/// <summary> 滚动方向 </summary>
public enum ScrollDirection
{
    Horizontal,
    Vertical
}

public class SUIScrollView : View
{
    public readonly ScrollDirection ScrollDirection;

    public readonly View MaskView = new();
    public readonly SUIScrollbar ScrollBar = new();

    public SUIScrollView(ScrollDirection scrollDirection)
    {
        Gap = new Vector2(8f);

        ScrollDirection = scrollDirection;

        MaskView.Display = Display.Flexbox;
        MaskView.FlexDirection = FlexDirection.Column;
        MaskView.Gap = new Vector2(12f);
        MaskView.FlexWrap = false;
        MaskView.OverflowHidden = true;
        MaskView.SetSize(0, 0, 1f, 1f);
        MaskView.Join(this);

        #region Scrollbar 滚动条

        ScrollBar.Join(this);
        ScrollBar.CornerRadius = new Vector4(4f);
        ScrollBar.BgColor = Color.Black * 0.25f;
        ScrollBar.Positioning = Positioning.Absolute;
        ScrollBar.HAlign = 1f;

        switch (ScrollDirection)
        {
            case ScrollDirection.Horizontal:
                ScrollBar.SetSize(0f, 8f, 1f, 0f);

                ScrollBar.OnUpdate += _ =>
                {
                    var mask = new Vector2(MaskView.GetInnerDimensions().Width, 1f);
                    var target = new Vector2(MaskView.GetFlexboxSize().X, 1f);

                    ScrollBar.SetArea(mask, target);
                };
                break;
            default:
            case ScrollDirection.Vertical:
                ScrollBar.SetSize(8f, 0f, 0f, 1f);

                ScrollBar.OnUpdate += _ =>
                {
                    var maskSize = new Vector2(1f, MaskView.GetInnerDimensions().Height);
                    var targetSize = new Vector2(1f, MaskView.GetFlexboxSize().Y);

                    ScrollBar.SetArea(maskSize, targetSize);
                };
                break;
        }

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

        if (MaskView.ScrollPosition != -ScrollBar.CurrentScrollPosition)
            MaskView.ScrollPosition = -ScrollBar.CurrentScrollPosition;
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