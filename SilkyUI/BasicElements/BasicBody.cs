﻿namespace SilkyUI.BasicElements;

public abstract class BasicBody : View
{
    protected BasicBody()
    {
        Border = 0;
        Width.Percent = Height.Percent = 1f;
        SpecifyWidth = SpecifyHeight = true;
    }

    public abstract bool IsEnabled { get; set; }

    /// <summary>
    /// 是否不可选中
    /// </summary>
    public virtual bool IsNotSelectable => false;

    /// <summary>
    /// 鼠标在当前 UI 某一个元素上时调用此方法，返回 true 此元素会占用光标，防止下层 UI 触发鼠标事件
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

    private float _opacity = 1f;

    /// <summary>
    /// 当前元素会创建一个独立的画布, 不建议频繁使用
    /// </summary>
    public virtual float Opacity
    {
        get => _opacity;
        set => _opacity = Math.Clamp(value, 0f, 1f);
    }

    protected float LastUIScale { get; set; } = Main.UIScale;
    protected bool UIScaleIsChanged => Math.Abs(LastUIScale - Main.UIScale) > float.Epsilon;

    protected virtual void OnUIScaleChanges()
    {
        try
        {
            UpdateMatrix();
        }
        finally
        {
            LastUIScale = Main.UIScale;
        }
    }

    protected Point GetCanvasSize()
    {
        var maxSize = new Vector2(Main.graphics.GraphicsDevice.Viewport.Width,
            Main.graphics.GraphicsDevice.Viewport.Height);
        var rtSize = Vector2.Min(maxSize,
            Vector2.Transform(new Vector2(Main.screenWidth, Main.screenHeight), FinalMatrix));
        return new Point((int)Math.Ceiling(rtSize.X), (int)Math.Ceiling(rtSize.Y));
    }

    /// <summary>
    /// 记录原来 RenderTargetUsage, 并全部设为 PreserveContents 防止设置新的 RenderTarget 时候消失
    /// </summary>
    protected static Dictionary<RenderTargetBinding, RenderTargetUsage> RecordWhileSetUpUsages
        (RenderTargetBinding[] bindings)
    {
        if (bindings is null || bindings.Length == 0) return null;
        Dictionary<RenderTargetBinding, RenderTargetUsage> usages = [];
        foreach (var item in bindings)
        {
            if (item.renderTarget is not RenderTarget2D rt) continue;
            usages[item] = rt.RenderTargetUsage;
            rt.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        return usages;
    }

    protected static void RecoverUsages(Dictionary<RenderTargetBinding, RenderTargetUsage> usages)
    {
        if (usages is null) return;
        foreach (var kvp in usages)
        {
            if (kvp.Key.renderTarget is not RenderTarget2D rt) continue;
            rt.RenderTargetUsage = kvp.Value;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (UIScaleIsChanged) OnUIScaleChanges();

        var pool = RenderTargetPool.Instance;
        var device = Main.graphics.GraphicsDevice;

        if (!UseRenderTarget)
        {
            base.Draw(spriteBatch);
            return;
        }

        var original = device.GetRenderTargets();
        var usages = RecordWhileSetUpUsages(original);
        var lastRenderTargetUsage = device.PresentationParameters.RenderTargetUsage;
        device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

        // 画布大小计算
        var canvasSize = GetCanvasSize();
        Console.WriteLine($"canvasSize: {canvasSize}");
        var rt2d = pool.Get(canvasSize.X, canvasSize.Y);
        try
        {
            device.SetRenderTarget(rt2d);
            device.Clear(Color.Transparent);
            base.Draw(spriteBatch);
            spriteBatch.End();

            // 恢复 RenderTargetUsage
            RecoverUsages(usages);
            device.SetRenderTargets(original);

            spriteBatch.Begin();
            spriteBatch.Draw(rt2d, Vector2.Zero, null,
                Color.White * Opacity, 0f, Vector2.Zero, Vector2.One, 0, 0);

            device.PresentationParameters.RenderTargetUsage = lastRenderTargetUsage;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            pool.Return(rt2d);
        }
    }
}