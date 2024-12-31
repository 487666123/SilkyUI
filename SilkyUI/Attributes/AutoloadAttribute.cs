namespace SilkyUI.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AutoloadAttribute(string insertionPoint, string name, int defaultPriority = 0,
    InterfaceScaleType interfaceScaleType = InterfaceScaleType.UI) : Attribute
{
    public string Name = name;

    public InterfaceScaleType InterfaceScaleType = interfaceScaleType;

    public string InsertionPoint = insertionPoint;

    public int DefaultPriority = defaultPriority;
}