namespace SilkyUI.Extensions;

public static class CalculatedStyleExtensions
{
    public static Vector2 Size(this CalculatedStyle calculatedStyle)
    {
        return new Vector2(calculatedStyle.Width, calculatedStyle.Height);
    }

    public static Vector2 RightBottom(this CalculatedStyle calculatedStyle)
    {
        return new Vector2(calculatedStyle.X + calculatedStyle.Width, calculatedStyle.Y + calculatedStyle.Height);
    }

    public static float Right(this CalculatedStyle calculatedStyle)
    {
        return calculatedStyle.X + calculatedStyle.Width;
    }

    public static float Bottom(this CalculatedStyle calculatedStyle)
    {
        return calculatedStyle.Y + calculatedStyle.Height;
    }
}
