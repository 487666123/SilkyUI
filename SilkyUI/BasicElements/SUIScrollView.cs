using SilkyUI.BasicComponents;

namespace SilkyUI.BasicElements;

public class SUIScrollContainer : View
{
    public SUIScrollContainer()
    {
        OverflowHidden = true;
    }

    public override View AppendFromView(View child)
    {
        child.Remove();
        child.Parent = this;
        Elements.Add(child);
        if (Parent is SUIScrollView sUIScrollView)
            sUIScrollView.Recalculate();
        else Recalculate();
        return this;
    }
}

public class SUIScrollView : View
{
    public readonly Direction Direction;

    public readonly SUIScrollContainer Container;
    public readonly SUIScrollbar ScrollBar;

    public SUIScrollView(Direction direction)
    {
        Direction = direction;

        Display = Display.Flexbox;
        FlexWrap = false;
        Gap = new Vector2(8f);

        Container = new SUIScrollContainer()
        {
            Display = Display.Flexbox,
            Gap = new Vector2(8f),
            MainAxisAlignment = MainAxisAlignment.SpaceBetween,
            FlexWrap = true,
        }.Join(this);

        ScrollBar = new SUIScrollbar(direction, Container)
        {
            CornerRadius = new Vector4(4f),
            BgColor = Color.Black * 0.25f,
        }.Join(this);
        ScrollBar.OnCurrentScrollPositionChanged += UpdateScrollPosition;

        switch (Direction)
        {
            case Direction.Horizontal:
                Container.FlexDirection = FlexDirection.Column;
                Container.SetSize(0, -16f, 1f, 1f);
                ScrollBar.SetSize(0f, 8f, 1f, 0f);
                break;
            default:
            case Direction.Vertical:
                Container.FlexDirection = FlexDirection.Row;
                Container.SetSize(-16f, 0, 1f, 1f);
                ScrollBar.SetSize(8f, 0f, 0f, 1f);
                break;
        }
    }

    public void UpdateScrollPosition(Vector2 currentScrollPosition) =>
        Container.ScrollPosition = -currentScrollPosition;

    public override void Recalculate()
    {
        base.Recalculate();

        if (Container == null) return;

        var content = Container.GetContentSize();
        switch (Direction)
        {
            case Direction.Horizontal:
                ScrollBar?.SetHScrollRange(Container._innerDimensions.Width, content.X);
                break;
            default:
            case Direction.Vertical:
                ScrollBar?.SetVScrollRange(Container._innerDimensions.Height, content.Y);
                break;
        }
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        switch (Direction)
        {
            case Direction.Horizontal:
                ScrollBar.HScrollBy(-evt.ScrollWheelValue);
                break;
            default:
            case Direction.Vertical:
                ScrollBar.VScrollBy(-evt.ScrollWheelValue);
                break;
        }

        base.ScrollWheel(evt);
    }

    public override View AppendFromView(View child)
    {
        return child is SUIScrollbar or SUIScrollContainer
            ? base.AppendFromView(child)
            : Container?.AppendFromView(child);
    }
}