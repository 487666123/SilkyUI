namespace SilkyUI.BasicElements;

public enum LayoutUnitType
{
    Pixels,
    Percent,
    Fraction
}

public static class LayoutUnitTypeHelper
{
    public static LayoutUnitType? SafeParse(string unit, LayoutUnitType? def = LayoutUnitType.Pixels)
    {
        return unit.Trim().ToLower() switch
        {
            "px" => LayoutUnitType.Pixels,
            "%" => LayoutUnitType.Percent,
            "fr" => LayoutUnitType.Fraction,
            _ => def
        };
    }

    public static bool TryParse(string unit, out LayoutUnitType type)
    {
        if (SafeParse(unit) is { } parsedType)
        {
            type = parsedType;
            return true;
        }

        type = LayoutUnitType.Pixels;
        return false;
    }
}