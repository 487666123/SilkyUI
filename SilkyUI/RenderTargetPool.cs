namespace SilkyUI;

public class RenderTargetPool
{
    private static readonly Lazy<RenderTargetPool> LazyInstance = new(() => new RenderTargetPool());

    public static RenderTargetPool Instance => LazyInstance.Value;

    private RenderTargetPool() { }

    /// <summary>
    /// 缓存
    /// </summary>
    private readonly Dictionary<(int, int), Stack<RenderTarget2D>> _cached = [];

    /// <summary>
    /// 被占用
    /// </summary>
    private readonly Dictionary<(int, int), Stack<RenderTarget2D>> _occupied = [];

    /// <summary>
    /// 借出 RenderTarget2D (需归还)
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <returns>(需归还)</returns>
    public RenderTarget2D Borrow(int width, int height)
    {
        // 必须要有大小
        if (width <= 0 || height <= 0)
        {
            throw new Exception("width 和 height 都必须大于 0");
        }

        var size = (width, height);

        #region 检查有没有对应池子，没有则创建
        if (!_cached.ContainsKey(size))
        {
            _cached.Add(size, new Stack<RenderTarget2D>());
        }

        if (!_occupied.ContainsKey(size))
        {
            _occupied.Add(size, new Stack<RenderTarget2D>());
        }
        #endregion

        if (_cached[size].Count > 0)
        {
            _cached[size].TryPop(out var renderTarget2D);
            _occupied[size].Push(renderTarget2D);
            return renderTarget2D;
        }
        else
        {
            var renderTarget2D = Create(width, height);
            _occupied[size].Push(renderTarget2D);
            return renderTarget2D;
        }
    }

    /// <summary>
    /// 归还 RenderTarget2D
    /// </summary>
    /// <param name="rt2d">要归还的 RenderTarget2D</param>
    public void Return(RenderTarget2D rt2d)
    {
        (int width, int height) size = (rt2d.Width, rt2d.Height);

        if (!_cached.ContainsKey(size) || !_occupied.ContainsKey(size) || !_occupied[size].Contains(rt2d))
        {
            throw new Exception("不属于此池");
        }

        _occupied[size].TryPop(out var _);
        _cached[size].Push(rt2d);
    }

    public void Dispose()
    {
        foreach ((_, Stack<RenderTarget2D> value) in _occupied)
        {
            if (value is null)
                continue;
            foreach (var renderTarget2D in value)
            {
                renderTarget2D?.Dispose();
            }
        }

        foreach ((_, Stack<RenderTarget2D> value) in _cached)
        {
            if (value is null)
                continue;
            foreach (var renderTarget2D in value)
            {
                renderTarget2D?.Dispose();
            }
        }
    }

    /// <summary>
    /// 创建 RenderTarget2D
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <returns>创建的对象</returns>
    public static RenderTarget2D Create(int width, int height)
    {
        // Usage 设置为 PreserveContents 在每次替换的时候可以不清除内容
        return new RenderTarget2D(Main.graphics.GraphicsDevice, width, height,
                false, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
    }
}
