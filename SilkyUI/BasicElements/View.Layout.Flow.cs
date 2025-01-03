namespace SilkyUI.BasicElements;

public partial class View
{
    public Vector2 GetFlowSize()
    {
        if (FlowElements is null || FlowElements.Count == 0) return Vector2.Zero;
        return new Vector2(0f, FlowElements.Sum(element => element._outerDimensions.Height + Gap.X) - Gap.X);
    }

    protected virtual void FlowArrange()
    {
        var currentTop = 0f;

        foreach (var element in FlowElements)
        {
            element.Position += new Vector2(0, currentTop);
            currentTop += element._outerDimensions.Height + Gap.Y;
        }
    }
}