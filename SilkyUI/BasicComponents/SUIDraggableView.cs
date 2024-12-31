namespace SilkyUI.BasicComponents;

/// <summary>
/// 可拖动视图
/// </summary>
public class SUIDraggableView : View
{
    /// <summary>
    /// 显示窗口阴影
    /// </summary>
    public bool Shaded;

    public float ShadowThickness;
    public Color ShadowColor;
    public bool Draggable { get; set; }
    public bool Dragging { get; protected set; }
    public Vector2 Offset;
    public Vector2 DragIncrement = new(5f);

    public SUIDraggableView(Color backgroundColor, Color borderColor, float rounded = 12, bool draggable = false)
    {
        SetPadding(10f);
        Draggable = draggable;

        ShadowColor = borderColor * 0.35f;

        BorderColor = borderColor;
        BgColor = backgroundColor;
        CornerRadius = new Vector4(rounded);
    }

    public SUIDraggableView(Color backgroundColor, Color borderColor, Vector4 cornerRadius, bool draggable = false)
    {
        SetPadding(10f);
        ShadowColor = borderColor * 0.35f;
        Draggable = draggable;

        BorderColor = borderColor;
        BgColor = backgroundColor;
        CornerRadius = cornerRadius;
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        // 当点击的是子元素不进行移动
        if (!Draggable ||
            (evt.Target != this && evt.Target is not View { DragIgnore: true } &&
             !evt.Target.GetType().IsAssignableFrom(typeof(UIElement)))) return;
        Offset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
        Dragging = true;
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        base.LeftMouseUp(evt);
        Dragging = false;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Dragging)
        {
            var (x, y) = (Main.mouseX - Offset.X, Main.mouseY - Offset.Y);
            if (DragIncrement.X != 0)
                x -= x % DragIncrement.X;
            if (DragIncrement.Y != 0)
                y -= y % DragIncrement.Y;
            this.SetPositionPixels(x, y).Recalculate();
        }

        base.Draw(spriteBatch);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        Vector2 position = GetDimensions().Position();
        Vector2 size = GetDimensions().Size();

        Vector2 shadowThickness = new(ShadowThickness);
        Vector2 shadowPosition = position - shadowThickness;
        Vector2 shadowSize = size + shadowThickness * 2;

        if (Shaded)
        {
            SDFRectangle.Shadow(shadowPosition, shadowSize, CornerRadius,
                ShadowColor, ShadowThickness, FinalMatrix);
        }

        base.DrawSelf(spriteBatch);
    }
}