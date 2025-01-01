namespace SilkyUI.BasicElements;

public partial class View
{
    public void ReflowFlowLayout()
    {
        var currentTop = -Gap.Y;

        foreach (var element in FlowElements)
        {
            element.Position += new Vector2(0, currentTop + Gap.Y);
            currentTop += element._outerDimensions.Height;
        }
    }
}