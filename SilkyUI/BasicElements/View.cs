namespace SilkyUI.BasicElements;

public interface IInputElement
{
    bool OccupyInput { get; }

    void OnInput(string input);
}

/// <summary>
/// 所有 SilkyUI 元素的父类
/// </summary>
public partial class View : UIElement, IInputElement
{
    public View()
    {
        MaxWidth = MaxHeight = new StyleDimension(114514f, 0f);

        OnLeftMouseDown += (_, _) => LeftMousePressed = true;
        OnRightMouseDown += (_, _) => RightMousePressed = true;
        OnMiddleMouseDown += (_, _) => MiddleMousePressed = true;

        OnLeftMouseUp += (_, _) => LeftMousePressed = false;
        OnRightMouseUp += (_, _) => RightMousePressed = false;
        OnMiddleMouseUp += (_, _) => MiddleMousePressed = false;
    }

    /// <summary> 占用输入 </summary>
    public virtual bool OccupyInput { get; set; }

    /// <summary> 输入 </summary>
    public virtual void OnInput(string input)
    {
    }

    public virtual UIElement GetElementAtFromView(Vector2 point)
    {
        if (Display is Display.None) return null;

        var children =
            GetChildrenByZIndex().OfType<View>().Where(el => !el.IgnoresMouseInteraction).Reverse().ToArray();

        if (OverflowHidden && !ContainsPoint(point)) return null;

        foreach (var child in children)
        {
            if (child.GetElementAt(point) is { } target) return target;
        }

        if (IgnoresMouseInteraction) return null;

        return ContainsPoint(point) ? this : null;
    }

    /// <summary>
    /// 判断点是否在元素内, 会计算<see cref="FinalMatrix"/>
    /// </summary>
    public override bool ContainsPoint(Vector2 point) =>
        base.ContainsPoint(Vector2.Transform(point, Matrix.Invert(FinalMatrix)));

    /// <summary>
    /// 鼠标悬停声音
    /// </summary>
    public SoundStyle? MouseOverSound { get; set; }

    public void UseMenuTickSoundForMouseOver() => MouseOverSound = SoundID.MenuTick;

    public override void MouseOver(UIMouseEvent evt)
    {
        if (MouseOverSound != null)
            SoundEngine.PlaySound(MouseOverSound);
        base.MouseOver(evt);
    }

    public virtual View AppendFromView(View child)
    {
        child.Remove();
        child.Parent = this;
        Elements.Add(child);
        Recalculate();
        return this;
    }
}