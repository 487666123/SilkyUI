namespace SilkyUI.BasicElements;

public abstract class BasicBody : View
{
    protected BasicBody()
    {
        Border = 0;
        Width.Percent = Height.Percent = 1f;
    }

    /// <summary>
    /// 启用
    /// </summary>
    public abstract bool IsEnabled { get; set; }

    /// <summary>
    /// 是否不可选中
    /// </summary>
    public virtual bool IsNotSelectable => false;

    /// <summary>
    /// 鼠标在当前 UI 某一个元素上时调用此方法，返回 <see langword="true"/> 此元素会占用光标，防止下层 UI 触发鼠标事件
    /// </summary>
    public abstract bool CanSetFocusTarget(UIElement target);

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        CheckScreenSizeChanges();
    }

    public event Action<Vector2, Vector2> ScreenSizeChanges;

    protected static Vector2 GetCurrentScreenSize() => PlayerInput.OriginalScreenSize / Main.UIScale;
    protected Vector2 LastScreenSize { get; set; } = GetCurrentScreenSize();

    /// <summary>
    /// 总是执行
    /// </summary>
    protected virtual void CheckScreenSizeChanges()
    {
        var currentScreenSize = GetCurrentScreenSize();
        if (currentScreenSize == LastScreenSize) return;
        OnScreenSizeChanges(currentScreenSize, LastScreenSize);
        LastScreenSize = currentScreenSize;
    }

    protected virtual void OnScreenSizeChanges(Vector2 newVector2, Vector2 oldVector2)
    {
        ScreenSizeChanges?.Invoke(newVector2, oldVector2);
        Recalculate();
    }

    public virtual bool UseRenderTarget { get; set; } = true;

    public virtual float Opacity
    {
        get => 0.75f;
        set { }
    }

    protected float LastUIScale { get; set; } = Main.UIScale;

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Math.Abs(LastUIScale - Main.UIScale) > float.Epsilon)
        {
            UpdateMatrix();
            LastUIScale = Main.UIScale;
        }

        // base.Draw(spriteBatch);
        // return;

        var rt2dPool = RenderTargetPool.Instance;
        var device = Main.graphics.GraphicsDevice;

        if (UseRenderTarget)
        {
            var originalRT2Ds = device.GetRenderTargets();

            if (originalRT2Ds != null)
            {
                foreach (var item in originalRT2Ds)
                {
                    if (item.renderTarget is RenderTarget2D rt)
                    {
                        rt.RenderTargetUsage = RenderTargetUsage.PreserveContents;
                    }
                }
            }

            // 刺客获取的屏幕大小不是正常的
            var rt2d = rt2dPool.Borrow((int)Math.Round(Main.screenWidth * Main.UIScale),
                (int)Math.Round(Main.screenHeight * Main.UIScale));

            var lastRenderTargetUsage = device.PresentationParameters.RenderTargetUsage;
            device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

            device.SetRenderTarget(rt2d);
            device.Clear(Color.Transparent);

            base.Draw(spriteBatch);

            spriteBatch.End();
            device.SetRenderTargets(originalRT2Ds);

            spriteBatch.Begin();
            spriteBatch.Draw(rt2d, Vector2.Zero, null,
                Color.White * Opacity, 0f, Vector2.Zero, Vector2.One, 0, 0);

            device.PresentationParameters.RenderTargetUsage = lastRenderTargetUsage;

            rt2dPool.Return(rt2d);
        }
        else
        {
            base.Draw(spriteBatch);
        }
    }
}