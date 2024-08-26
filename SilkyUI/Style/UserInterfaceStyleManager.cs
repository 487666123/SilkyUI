namespace SilkyUI.Style;

public class UserInterfaceStyleManager
{
    private readonly Dictionary<string, UserInterfaceStyle> _styles = [];
    public UserInterfaceStyle Style { get; private set; }

    public UserInterfaceStyleManager()
    {
        var defaultStyle = new UserInterfaceStyle("Default");
        Style = defaultStyle;
        _styles["Default"] = defaultStyle;
    }

    /// <summary>
    /// 更换样式
    /// </summary>
    public bool ChangeStyle(string styleName)
    {
        if (!_styles.TryGetValue(styleName, out var value))
            return false;

        Style = value;
        return true;
    }
}