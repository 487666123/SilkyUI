namespace SilkyUI.UIStyle;

public class SilkyUIStyle : ISilkyUIStyle
{
    public Mod Mod { get; protected set; }
    public String StyleName { get; protected set; }

    public virtual void OnInitialize()
    {

    }

    public virtual void OnChangeStyle(Mod mod, string styleName)
    {
        if (Mod == mod && !StyleName.Equals(styleName))
        {

        }
    }

    public Vector4 BorderCornerRadiusLarge { get; set; } = new Vector4(12);
    public Vector4 BorderCornerRadiusNormal { get; set; } = new Vector4(10);
    public Vector4 BorderCornerRadiusSmall { get; set; } = new Vector4(8);
    public Color BorderColor { get; set; } = new Color(18, 18, 38);
    public Color BorderFavoriteColor { get; set; } = new Color(0, 0, 0);
    public Color BackgroundColor { get; set; } = new Color(63, 65, 151);
}
