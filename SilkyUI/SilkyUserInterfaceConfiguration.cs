namespace SilkyUI;

public class SilkyUserInterfaceConfiguration
{
    private float _largeFontOffset;

    /// <summary>
    /// 大字体 Y 轴偏移
    /// </summary>
    public float LargeFontOffset
    {
        get { return _largeFontOffset; }
        set { _largeFontOffset = value; }
    }

    private float _normalFontOffset;
    /// <summary>
    /// 正常字体 Y 轴偏移
    /// </summary>
    public float NormalFontOffset
    {
        get { return _normalFontOffset; }
        set { _normalFontOffset = value; }
    }

    /// <summary>
    /// 挑选字体偏移
    /// </summary>
    /// <param name="isLarge">大字</param>
    public float GetFontOffset(bool isLarge) => isLarge ? _largeFontOffset : _normalFontOffset;
}
