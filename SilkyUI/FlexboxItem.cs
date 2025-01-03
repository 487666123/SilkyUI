namespace SilkyUI.BasicElements;

public class FlexboxItem
{
    public readonly List<View> Elements = [];
    public float Width { get; set; }
    public float Height { get; set; }

    public void Add(View element)
    {
        Elements.Add(element);
    }
}