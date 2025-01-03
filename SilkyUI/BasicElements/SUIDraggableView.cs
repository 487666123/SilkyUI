namespace SilkyUI.BasicElements;

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
    public Vector2 DragIncrement { get; set; } = Vector2.Zero;
    public Vector2 DragOffset { get; set; } = Vector2.Zero;

    public SUIDraggableView(bool draggable = true)
    {
        Draggable = draggable;

        Border = 2;
        BorderColor = new Color(18, 18, 38) * 0.75f;
        BgColor = new Color(63, 65, 151) * 0.75f;
        CornerRadius = new Vector4(12);

        ShadowColor = new Color(18, 18, 38) * 0.1f;
        Shaded = true;
        ShadowThickness = 50f;

        SetPadding(10f);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        if (!Draggable ||
            (evt.Target != this && evt.Target is not View { DragIgnore: true } &&
             !evt.Target.GetType().IsAssignableFrom(typeof(UIElement)))) return;

        DragOffset = new Vector2(Main.mouseX, Main.mouseY) - Offset;
        Dragging = true;
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        base.LeftMouseUp(evt);
        Dragging = false;
    }

    public override void Update(GameTime gameTime)
    {
        if (IsMouseHovering)
        {
            // 锁定滚动条
            PlayerInput.LockVanillaMouseScroll("SilkyUserInterfaceFramework");
            // 锁定鼠标操作
            Main.LocalPlayer.mouseInterface = true;
        }

        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Dragging)
        {
            var x = Main.mouseX - DragOffset.X;
            var y = Main.mouseY - DragOffset.Y;
            if (DragIncrement.X != 0) x -= x % DragIncrement.X;
            if (DragIncrement.Y != 0) y -= y % DragIncrement.Y;
            Offset = new Vector2(x, y);
        }

        base.Draw(spriteBatch);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        var position = GetDimensions().Position();
        var size = GetDimensions().Size();

        var shadowThickness = new Vector2(ShadowThickness);
        var shadowPosition = position - shadowThickness;
        var shadowSize = size + shadowThickness * 2;

        if (Shaded)
        {
            SDFRectangle.Shadow(shadowPosition, shadowSize, CornerRadius,
                ShadowColor, ShadowThickness, FinalMatrix);
        }

        base.DrawSelf(spriteBatch);
    }
}