namespace SilkyUI;

public static class SilkyUIHelper
{
    public static CalculatedStyle GetBasicBodyDimensions()
    {
        var originalScreenSize = PlayerInput.OriginalScreenSize;

        return new CalculatedStyle(0f, 0f,
            originalScreenSize.X / Main.UIScale,
            originalScreenSize.Y / Main.UIScale);
    }
}