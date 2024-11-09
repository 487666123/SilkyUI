using System.Text.RegularExpressions;

namespace SilkyUI.BasicElements;

public partial struct LayoutUnit(LayoutUnitType unitType, float value)
{
    public static LayoutUnit Zero { get; } = new(LayoutUnitType.Pixels, 0);

    public readonly LayoutUnitType LayoutUnitType = unitType;
    public readonly float Value = value;

    public static implicit operator LayoutUnit(string unit)
    {
        var layoutUnit = Parse(unit);
        if (layoutUnit is not null)
            return (LayoutUnit)layoutUnit;

        return Zero;
    }

    #region Parse

    [GeneratedRegex(@"^(\d+(?:\.\d)?)(px|%|fr)$", RegexOptions.Compiled)]
    private static partial Regex UnitRegex();

    public static LayoutUnit? Parse(string unit)
    {
        var match = UnitRegex().Match(unit);
        // 尝试解析
        if (!match.Success ||
            !LayoutUnitTypeHelper.TryParse(match.Groups[2].Value, out var unitType) ||
            float.TryParse(match.Groups[1].Value, out var value))
            return null;
        return new LayoutUnit(unitType, value);
    }

    public static bool TryParse(string unit, out LayoutUnit layoutUnit)
    {
        if (Parse(unit) is { } parsed)
        {
            layoutUnit = parsed;
            return true;
        }

        layoutUnit = Zero;
        return false;
    }

    #endregion
}