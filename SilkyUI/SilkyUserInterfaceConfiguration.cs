namespace SilkyUI;

public class SilkyUserInterfaceConfiguration
{
    /// <summary>
    /// 不同字体的 Y 轴偏移量
    /// </summary>
    public Dictionary<DynamicSpriteFont, float> FontYAxisOffset { get; } = [];

    public SilkyUserInterfaceConfiguration()
    {
        // 正常大小的字体, 使用的地方最多
        FontYAxisOffset[FontAssets.MouseText.Value] = 2.5f;
        // 大概是正常大小字体两倍大, 通常是用于标题的字体
        FontYAxisOffset[FontAssets.DeathText.Value] = 6f;
    }
}