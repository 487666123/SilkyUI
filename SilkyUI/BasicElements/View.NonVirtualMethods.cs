namespace SilkyUI.BasicElements;

public partial class View
{
    /// <summary>
    /// 对元素进行分类
    /// </summary>
    public void ClassifyElements()
    {
        FlowElements.Clear();
        AbsoluteElements.Clear();

        Elements.ForEach(element =>
        {
            if (element is View { Positioning: Positioning.Relative } view)
                FlowElements.Add(view);
            else
                AbsoluteElements.Add(element);
        });
    }

    public IOrderedEnumerable<UIElement> GetChildrenByZIndex() =>
        Elements.OrderBy(el => el is View v ? v.ZIndex : 0f);

    /// <summary>
    /// 会将 <see cref="SpecifyWidth"/> 设为 true
    /// </summary>
    public void SetWidth(float pixel, float? percent = null)
    {
        Width.Pixels = pixel;
        if (percent is { } value)
            Width.Percent = value;
        SpecifyWidth = true;
    }

    /// <summary>
    /// 会将 <see cref="SpecifyWidth"/> 设为 true
    /// </summary>
    public void SetWidth(StyleDimension width)
    {
        Width = width;
        SpecifyWidth = true;
    }

    /// <summary>
    /// 会将 <see cref="SpecifyHeight"/> 设为 true
    /// </summary>
    public void SetHeight(float pixel, float? percent = null)
    {
        Height.Pixels = pixel;
        if (percent is { } value)
            Height.Percent = value;
        SpecifyHeight = true;
    }

    /// <summary>
    /// 会将 <see cref="SpecifyHeight"/> 设为 true
    /// </summary>
    public void SetHeight(StyleDimension height)
    {
        Height = height;
        SpecifyHeight = true;
    }

    public void SetSize(float widthPixels, float heightPixels, float? widthPercent = null, float? heightPercent = null)
    {
        Width.Pixels = widthPixels;
        if (widthPercent is { } value1)
            Width.Percent = value1;

        Height.Pixels = heightPixels;
        if (heightPercent is { } value2)
            Height.Percent = value2;

        SpecifyWidth = SpecifyHeight = true;
    }

    public void SetSize(StyleDimension width, StyleDimension height)
    {
        Width = width;
        Height = height;

        SpecifyWidth = SpecifyHeight = true;
    }
}