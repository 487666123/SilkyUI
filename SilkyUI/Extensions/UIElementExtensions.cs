namespace SilkyUI.Extensions;

public static class UIElementExtensions
{
    public static View RecentParentView(this UIElement uie)
    {
        if (uie == null)
            return null;

        var parent = uie.Parent;

        if (parent is null)
            return null;

        do
        {
            if (parent is View view)
            {
                return view;
            }

            parent = parent.Parent;
        } while (parent is not null);

        return null;
    }

    public static UIElement PreviousElement(this UIElement uie, Func<UIElement, bool> predicate)
    {
        var parent = uie.Parent;
        if (predicate == null || parent == null || uie == null || parent.Elements.Count <= 1 || parent.Elements[0] == uie)
            return null;

        var index = parent.Elements.IndexOf(uie);

        while (index >= 1)
        {
            index--;
            var previous = parent.Elements[index];
            if (predicate(previous))
            {
                return parent.Elements[(parent.Elements.IndexOf(uie) - 1)];
            }
        }

        return null;
    }

    public static UIElement PreviousRelativeElement(this UIElement uie) =>
        uie.PreviousElement(
            previous => previous is View view && view.Positioning is Positioning.Relative);

    public static UIElement SetPositionPixels(this UIElement uie, float x, float y)
    {
        uie.Left.Pixels = x; uie.Top.Pixels = y;
        return uie;
    }

    #region Offset
    public static void OffsetX(this UIElement uie, float offset)
    {
        uie._outerDimensions.X += offset;

        uie._dimensions.X += offset;

        uie._innerDimensions.X += offset;

        foreach (var child in uie.Elements)
        {
            child.OffsetX(offset);
        }
    }

    public static void OffsetY(this UIElement uie, float offset)
    {
        uie._outerDimensions.Y += offset;

        uie._dimensions.Y += offset;

        uie._innerDimensions.Y += offset;

        foreach (var child in uie.Elements)
        {
            child.OffsetY(offset);
        }
    }

    public static void Offset(this UIElement uie, Vector2 offset)
    {
        uie._outerDimensions.X += offset.X;
        uie._outerDimensions.Y += offset.Y;

        uie._dimensions.X += offset.X;
        uie._dimensions.Y += offset.Y;

        uie._innerDimensions.X += offset.X;
        uie._innerDimensions.Y += offset.Y;

        foreach (var child in uie.Elements)
        {
            child.Offset(offset);
        }
    }
    #endregion

    public static void Join
        (this UIElement uie, UIElement parent) => parent.Append(uie);

    #region Margin Padding
    public static float HMargin(this UIElement uie) => uie.MarginLeft + uie.MarginRight;

    public static float VMargin(this UIElement uie) => uie.MarginTop + uie.MarginBottom;

    public static float HPadding(this UIElement uie) => uie.PaddingLeft + uie.PaddingRight;

    public static float VPadding(this UIElement uie) => uie.PaddingTop + uie.PaddingBottom;
    #endregion
}