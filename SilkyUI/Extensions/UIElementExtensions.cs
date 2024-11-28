﻿namespace SilkyUI.Extensions;

public static class UIElementExtensions
{
    public static View RecentParentView(this UIElement uie)
    {
        var parent = uie?.Parent;

        while (parent is not null)
        {
            if (parent is View view)
                return view;

            parent = parent.Parent;
        }

        return null;
    }

    public static UIElement PreviousElement(this UIElement uie, Func<UIElement, bool> predicate)
    {
        if (predicate == null ||
            uie?.Parent is not { Elements.Count: > 1 } parent ||
            parent.Elements[0] == uie)
            return null;

        var index = parent.Elements.IndexOf(uie);

        while (index >= 1)
        {
            var previous = parent.Elements[--index];

            if (predicate(previous))
            {
                return parent.Elements[(parent.Elements.IndexOf(uie) - 1)];
            }
        }

        return null;
    }

    public static UIElement PreviousRelativeElement(this UIElement uie) =>
        uie.PreviousElement(previous => previous is View { Position: Position.Relative });

    public static T SetPositionPixels<T>(this T uie, float x, float y) where T : UIElement
    {
        uie.Left.Pixels = x;
        uie.Top.Pixels = y;
        return uie;
    }

    #region Offset Position

    public static T DimensionsSet<T>(this T uie, Vector2 position) where T : UIElement
    {
        uie.DimensionsOffset(position - uie._outerDimensions.Position());

        return uie;
    }

    public static T DimensionsSetX<T>(this T uie, float position) where T : UIElement
    {
        uie.DimensionsOffsetX(position - uie._outerDimensions.X);

        return uie;
    }

    public static T DimensionsSetY<T>(this T uie, float position) where T : UIElement
    {
        uie.DimensionsOffsetY(position - uie._outerDimensions.Y);

        return uie;
    }

    public static T DimensionsOffsetX<T>(this T uie, float offset) where T : UIElement
    {
        uie._outerDimensions.X += offset;
        uie._dimensions.X += offset;
        uie._innerDimensions.X += offset;

        uie.Elements.ForEach(element => element.DimensionsOffsetX(offset));

        return uie;
    }

    public static T DimensionsOffsetY<T>(this T uie, float offset) where T : UIElement
    {
        uie._outerDimensions.Y += offset;
        uie._dimensions.Y += offset;
        uie._innerDimensions.Y += offset;

        uie.Elements.ForEach(element => element.DimensionsOffsetY(offset));

        return uie;
    }

    public static T DimensionsOffset<T>(this T uie, Vector2 offset) where T : UIElement
    {
        uie._outerDimensions.X += offset.X;
        uie._outerDimensions.Y += offset.Y;

        uie._dimensions.X += offset.X;
        uie._dimensions.Y += offset.Y;

        uie._innerDimensions.X += offset.X;
        uie._innerDimensions.Y += offset.Y;

        uie.Elements.ForEach(element => element.DimensionsOffset(offset));

        return uie;
    }

    #endregion

    public static T Join<T>(this T uie, UIElement parent) where T : UIElement
    {
        parent.Append(uie);
        return uie;
    }

    #region [Margin] [Padding]

    public static float HMargin(this UIElement uie) => uie.MarginLeft + uie.MarginRight;

    public static float VMargin(this UIElement uie) => uie.MarginTop + uie.MarginBottom;

    public static float HPadding(this UIElement uie) => uie.PaddingLeft + uie.PaddingRight;

    public static float VPadding(this UIElement uie) => uie.PaddingTop + uie.PaddingBottom;

    #endregion
}