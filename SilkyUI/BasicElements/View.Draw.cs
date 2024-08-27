namespace SilkyUI.BasicElements;

public partial class View
{
    public event Action<View> OnUpdateTransformMatrix;

    /// <summary>
    /// 绘制自己
    /// </summary>
    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        DrawSDFRectangle();
        base.DrawSelf(spriteBatch);
    }

    /// <summary>
    /// 更新变换矩阵
    /// </summary>
    protected virtual void UpdateTransformMatrix()
    {
        if (this.RecentParentView() is View parent)
            TransformMatrix = parent.TransformMatrix;
        else
            TransformMatrix = Main.UIScaleMatrix;

        OnUpdateTransformMatrix?.Invoke(this);
    }

    /// <summary>
    /// 绘制
    /// </summary>
    public override void Draw(SpriteBatch spriteBatch)
    {
        UpdateTransformMatrix();
        UpdateAnimationTimer();

        if (TransformMatrix == Matrix.Identity)
        {
            OriginalDraw(spriteBatch);
            return;
        }

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred,
            null, null, null, null, null, TransformMatrix);

        OriginalDraw(spriteBatch);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred,
            null, null, null, null, null, TransformMatrix);
    }

    /// <summary>
    /// 原版绘制
    /// </summary>
    protected virtual void OriginalDraw(SpriteBatch spriteBatch)
    {
        var overflowHidden = OverflowHidden;
        var useImmediateMode = UseImmediateMode;
        var scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
        var anisotropicClamp = SamplerState.AnisotropicClamp;
        if (useImmediateMode || OverrideSamplerState != null)
        {
            spriteBatch.End();
            spriteBatch.Begin(useImmediateMode ? SpriteSortMode.Immediate : SpriteSortMode.Deferred,
                BlendState.AlphaBlend, OverrideSamplerState ?? anisotropicClamp, DepthStencilState.None,
                OverflowHiddenRasterizerState, null, TransformMatrix);
            DrawSelf(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None,
                OverflowHiddenRasterizerState, null, TransformMatrix);
        }
        else
        {
            DrawSelf(spriteBatch);
        }

        if (overflowHidden)
        {
            spriteBatch.End();
            var scissorRectangle2 = Rectangle.Intersect(GetClippingRectangle(spriteBatch),
                spriteBatch.GraphicsDevice.ScissorRectangle);
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
            spriteBatch.GraphicsDevice.RasterizerState = OverflowHiddenRasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None,
                OverflowHiddenRasterizerState, null, TransformMatrix);
        }

        DrawChildren(spriteBatch);
        if (overflowHidden)
        {
            var rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None,
                rasterizerState, null, TransformMatrix);
        }

        if (!FinallyDrawBorder || !(Border > 0f) || BorderColor == Color.Transparent) return;
        var position = GetDimensions().Position();
        var size = GetDimensions().Size();

        SDFRectangle.HasBorder(position, size, Rounded,
            Color.Transparent, Border, BorderColor, TransformMatrix);
    }

    /// <summary>
    /// 绘制子元素
    /// </summary>
    /// <param name="spriteBatch"></param>
    public override void DrawChildren(SpriteBatch spriteBatch)
    {
        // 不绘制完全溢出的子元素
        if (OverflowHidden && HideFullyOverflowedElements)
        {
            var size = Parent.GetDimensions().Size();
            var position = Parent.GetDimensions().Position();

            foreach (var uie in from uie in Elements
                                let childPosition = uie.GetDimensions().Position()
                                let childSize = uie.GetDimensions().Size()
                                where Collision.CheckAABBvAABBCollision(position, size, childPosition, childSize)
                                select uie)
            {
                uie.Draw(spriteBatch);
            }
        }
        else
        {
            foreach (var element in Elements)
            {
                element.Draw(spriteBatch);
            }
        }
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

    /// <summary>
    /// 绘制 SDF 圆角矩形
    /// </summary>
    protected void DrawSDFRectangle()
    {
        var position = GetDimensions().Position();
        var size = GetDimensions().Size();

        if (Border > 0)
        {
            if (BorderColor == Color.Transparent || FinallyDrawBorder)
            {
                if (BgColor != Color.Transparent)
                    SDFRectangle.NoBorder(position + new Vector2(Border), size - new Vector2(Border * 2f),
                        Rounded - new Vector4(Border), BgColor, TransformMatrix);
            }
            else
            {
                SDFRectangle.HasBorder(position, size, Rounded, BgColor, Border, BorderColor, TransformMatrix);
            }
        }
        else if (BgColor != Color.Transparent)
        {
            SDFRectangle.NoBorder(position, size, Rounded, BgColor, TransformMatrix);
        }
    }
}
