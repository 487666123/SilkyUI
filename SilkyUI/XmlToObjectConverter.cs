using System.Xml.Serialization;

namespace SilkyUI;

public class XmlToObjectConverter
{
    private readonly XmlAttributeOverrides _overrides = new();
    private readonly Dictionary<Type, List<XmlArrayItemAttribute>> _typeMappings = new();

    /// <summary>
    /// 注册根类型及其属性映射
    /// </summary>
    public void RegisterRoot<T>(string rootName)
    {
        var attributes = new XmlAttributes { XmlRoot = new XmlRootAttribute(rootName) };
        _overrides.Add(typeof(T), attributes);
    }

    /// <summary>
    /// 注册属性为集合类型
    /// </summary>
    public void RegisterArray<TParent>(string propertyName, string arrayName)
    {
        var attributes = new XmlAttributes { XmlArray = new XmlArrayAttribute(arrayName) };
        _overrides.Add(typeof(TParent), propertyName, attributes);
    }

    /// <summary>
    /// 注册集合中可能的子类型
    /// </summary>
    // public void RegisterArrayItem<TParent>(string propertyName, Type itemType, string elementName)
    // {
    //     if (!_typeMappings.TryGetValue(typeof(TParent), out var items))
    //     {
    //         items = new List<XmlArrayItemAttribute>();
    //         _typeMappings[typeof(TParent)] = items;
    //     }
    //
    //     items.Add(new XmlArrayItemAttribute(elementName, itemType));
    //
    //     var attributes = new XmlAttributes();
    //     attributes.XmlArrayItems.AddRange(items);
    //     _overrides.Add(typeof(TParent), propertyName, attributes);
    // }

    /// <summary>
    /// 注册属性为 XML 属性
    /// </summary>
    public void RegisterAttribute<TParent>(string propertyName, string attributeName)
    {
        var attributes = new XmlAttributes { XmlAttribute = new XmlAttributeAttribute(attributeName) };
        _overrides.Add(typeof(TParent), propertyName, attributes);
    }

    /// <summary>
    /// 将 XML 转换为对象
    /// </summary>
    public T Deserialize<T>(string xml)
    {
        var serializer = new XmlSerializer(typeof(T), _overrides);
        using var reader = new StringReader(xml);
        return (T)serializer.Deserialize(reader);
    }
}