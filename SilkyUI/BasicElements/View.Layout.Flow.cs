namespace SilkyUI.BasicElements;

public partial class View
{
    protected virtual void ReflowFlowLayout()
    {
        var currentTop = 0f;

        foreach (var element in FlowElements)
        {
            element.Position += new Vector2(0, currentTop);
            currentTop += element._outerDimensions.Height + Gap.Y;
        }
    }
}