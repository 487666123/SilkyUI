namespace SilkyUI.BasicElements;

public partial class View
{
    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        RoundedRectangle.Draw(GetDimensions().Position(), GetDimensions().Size(), FinallyDrawBorder, FinalMatrix);
        base.DrawSelf(spriteBatch);
    }

    protected void UpdateMatrix()
    {
        try
        {
            var original = this.RecentParentView() is { } parent ? parent.FinalMatrix : Main.UIScaleMatrix;
            FinalMatrix = TransformMatrix * original;
            foreach (var view in Elements.OfType<View>()) view.UpdateMatrix();
        }
        finally
        {
            TransformMatrixHasChanges = false;
        }
    }

    public event Action<SpriteBatch> OnDraw;

    /// <summary>
    /// 绘制
    /// </summary>
    public override void Draw(SpriteBatch spriteBatch)
    {
        UpdateAnimationTimer();
        if (TransformMatrixHasChanges) UpdateMatrix();
        OnDraw?.Invoke(spriteBatch);

        if (Parent is not View view || FinalMatrix != view.FinalMatrix)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred,
                null, null, null, OverflowHiddenRasterizerState, null, FinalMatrix);

            OriginalDraw(spriteBatch);

            var matrix = Parent is View parent ? parent.FinalMatrix : Main.UIScaleMatrix;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred,
                null, null, null, OverflowHiddenRasterizerState, null, matrix);
        }
        else OriginalDraw(spriteBatch);
    }

    /// <summary>
    /// 原版绘制
    /// </summary>
    protected virtual void OriginalDraw(SpriteBatch spriteBatch)
    {
        var useImmediateMode = UseImmediateMode;

        // 立即绘制
        if (useImmediateMode || OverrideSamplerState != null)
        {
            spriteBatch.End();
            var samplerState = OverrideSamplerState ?? SamplerState.AnisotropicClamp;
            var sortMode = useImmediateMode ? SpriteSortMode.Immediate : SpriteSortMode.Deferred;
            spriteBatch.Begin(sortMode, BlendState.AlphaBlend, samplerState,
                DepthStencilState.None, OverflowHiddenRasterizerState, null, FinalMatrix);
            DrawSelf(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, OverflowHiddenRasterizerState, null, FinalMatrix);
        }
        else DrawSelf(spriteBatch);

        // 裁切绘制
        var originalScissor = spriteBatch.GraphicsDevice.ScissorRectangle;
        if (OverflowHidden)
        {
            spriteBatch.End();
            var rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            var scissor = Rectangle.Intersect(GetClippingRectangleFromView(spriteBatch), originalScissor);
            spriteBatch.GraphicsDevice.ScissorRectangle = scissor;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, OverflowHiddenRasterizerState, null, FinalMatrix);

            DrawChildren(spriteBatch);

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = originalScissor;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, rasterizerState, null, FinalMatrix);
        }
        else DrawChildren(spriteBatch);

        if (!FinallyDrawBorder || Border <= 0f || BorderColor == Color.Transparent) return;
        var position = GetDimensions().Position();
        var size = GetDimensions().Size();

        RoundedRectangle.DrawOnlyTheBorder(position, size, FinalMatrix);
    }

    /// <summary>
    /// 绘制子元素
    /// </summary>
    /// <param name="spriteBatch"></param>
    public override void DrawChildren(SpriteBatch spriteBatch)
    {
        var children = GetChildrenByZIndex();

        // 不绘制完全溢出的子元素
        if (OverflowHidden || HideFullyOverflowedElements)
        {
            foreach (var uie in from uie in children
                     where _dimensions.Intersects(uie._dimensions)
                     select uie)
            {
                uie.Draw(spriteBatch);
            }
        }
        else
        {
            foreach (var element in children)
            {
                element.Draw(spriteBatch);
            }
        }
    }

    protected virtual Rectangle GetClippingRectangleFromView(SpriteBatch spriteBatch)
    {
        var topLeft = Vector2.Transform(_dimensions.Position(), FinalMatrix);
        var rightBottom = Vector2.Transform(_dimensions.RightBottom(), FinalMatrix);
        var rectangle =
            new Rectangle(
                (int)Math.Floor(topLeft.X), (int)Math.Floor(topLeft.Y),
                (int)Math.Ceiling(rightBottom.X - topLeft.X),
                (int)Math.Ceiling(rightBottom.Y - topLeft.Y));
        var scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
        return Rectangle.Intersect(rectangle, scissorRectangle);
    }

    /// <summary>
    /// 更新动画计时器
    /// </summary>
    protected virtual void UpdateAnimationTimer()
    {
        if (IsMouseHovering)
        {
            if (!HoverTimer.IsForward)
                HoverTimer.StartForwardUpdate();
        }
        else if (!HoverTimer.IsReverse)
        {
            HoverTimer.StartReverseUpdate();
        }

        HoverTimer.Update();
    }
}