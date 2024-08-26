namespace SilkyUI.Style;

public class UserInterfaceStyle
{
    public readonly string StyleName;
    private readonly Dictionary<string, object> _properties = [];

    public UserInterfaceStyle(string styleName)
    {
        StyleName = styleName;

        _properties["BorderColor"] = new Color(18, 18, 38);
        _properties["BorderFavoriteColor"] = new Color(0, 0, 0);
        _properties["BackgroundColor"] = new Color(63, 65, 151);

        _properties["SmallRounded"] = 8f;
        _properties["MediumRounded"] = 10f;
        _properties["LargeRounded"] = 12f;
    }

    public void Add<T>(T value, string propertyName) where T : new()
    {
        _properties[propertyName] = value;
    }

    public Color GetColor(string propertyName)
    {
        return GetProperty<Color>(propertyName);
    }

    public float GetFloat(string propertyName)
    {
        return GetProperty<float>(propertyName);
    }

    public T GetProperty<T>(string propertyName) where T : new()
    {
        if (_properties.TryGetValue(propertyName, out var value) && value is T t)
        {
            return t;
        }

        return new T();
    }
}