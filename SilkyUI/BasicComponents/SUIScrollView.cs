namespace SilkyUI.BasicComponents;

/// <summary> 滚动方向 </summary>
public enum Direction
{
    Horizontal,
    Vertical
}

public class SUIScrollView : View
{
    public readonly Direction Direction;

    public readonly View Container;
    public readonly SUIScrollbar ScrollBar;

    public SUIScrollView(Direction direction)
    {
        Direction = direction;

        Display = Display.Flexbox;
        FlexDirection = FlexDirection.Row;
        Gap = new Vector2(8f);

        Container = new View
        {
            OverflowHidden = true,
            Display = Display.Flexbox,
            FlexDirection = FlexDirection.Column,
            Gap = new Vector2(8f),
            FlexWrap = false,
        }.Join(this);
        Container.SetSize(-16f, 0, 1f, 1f);


        ScrollBar = new SUIScrollbar(Container)
        {
            CornerRadius = new Vector4(4f),
            BgColor = Color.Black * 0.25f,
        }.Join(this);
        ScrollBar.OnCurrentScrollPositionChanged += UpdateScrollPosition;

        switch (Direction)
        {
            case Direction.Horizontal:
                ScrollBar.SetSize(0f, 8f, 1f, 0f);
                break;
            default:
            case Direction.Vertical:
                ScrollBar.SetSize(8f, 0f, 0f, 1f);
                break;
        }
    }

    public void UpdateScrollPosition(Vector2 currentScrollPosition) =>
        Container.ScrollPosition = -currentScrollPosition;

    public override void Recalculate()
    {
        base.Recalculate();

        if (Container is null) return;
        Container.GetContentSize(out var size);
        switch (Direction)
        {
            case Direction.Horizontal:
                ScrollBar?.SetHScrollRange(Container._innerDimensions.Width, size.X);
                break;
            default:
            case Direction.Vertical:
                ScrollBar?.SetVScrollRange(_innerDimensions.Height, size.Y);
                break;
        }
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        switch (Direction)
        {
            case Direction.Horizontal:
                ScrollBar.SetHScrollTarget(-evt.ScrollWheelValue);
                break;
            default:
            case Direction.Vertical:
                ScrollBar.SetVScrollTarget(-evt.ScrollWheelValue);
                break;
        }

        base.ScrollWheel(evt);
    }
}