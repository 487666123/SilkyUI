using SilkyUI.BasicComponents;

namespace SilkyUI.BasicElements;

public partial class View
{
    /// <summary>
    /// 绘制自己
    /// </summary>
    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        RoundedRectangle.Draw(GetDimensions().Position(), GetDimensions().Size(), FinallyDrawBorder, FinalMatrix);
        base.DrawSelf(spriteBatch);
    }

    protected void UpdateMatrix()
    {
        try
        {
            var original =
                this.RecentParentView() is { } parent ? parent.FinalMatrix : Main.UIScaleMatrix;
            FinalMatrix = TransformMatrix * original;
            foreach (var view in Elements.OfType<View>()) view.UpdateMatrix();
        }
        finally
        {
            TransformMatrixHasChanges = false;
        }
    }

    /// <summary>
    /// 绘制
    /// </summary>
    public override void Draw(SpriteBatch spriteBatch)
    {
        UpdateAnimationTimer();
        if (TransformMatrixHasChanges)
            UpdateMatrix();

        if (FinalMatrix == Matrix.Identity)
        {
            OriginalDraw(spriteBatch);
            return;
        }

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred,
            null, null, null, OverflowHiddenRasterizerState, null, FinalMatrix);

        OriginalDraw(spriteBatch);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred,
            null, null, null, OverflowHiddenRasterizerState, null, FinalMatrix);
    }

    public Rectangle GetClippingRectangleFromView(SpriteBatch spriteBatch)
    {
        var topLeft = Vector2.Transform(_innerDimensions.Position(), FinalMatrix);
        var rightBottom = Vector2.Transform(_innerDimensions.RightBottom(), FinalMatrix);
        var rectangle =
            new Rectangle(
                (int)Math.Floor(topLeft.X), (int)Math.Floor(topLeft.Y),
                (int)Math.Ceiling(rightBottom.X - topLeft.X),
                (int)Math.Ceiling(rightBottom.Y - topLeft.Y));
        var scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
        // rectangle = Rectangle.Intersect(rectangle, scissorRectangle);
        // var max1 = (int)Math.Ceiling(Main.screenWidth * Main.UIScale);
        // var max2 = (int)Math.Ceiling(Main.screenHeight * Main.UIScale);
        // rectangle.X = Utils.Clamp(rectangle.X, 0, max1);
        // rectangle.Y = Utils.Clamp(rectangle.Y, 0, max2);
        // rectangle.Width = Utils.Clamp(rectangle.Width, 0, max1 - rectangle.X);
        // rectangle.Height = Utils.Clamp(rectangle.Height, 0, max2 - rectangle.Y);
        return Rectangle.Intersect(rectangle, scissorRectangle);
    }

    /// <summary>
    /// 原版绘制
    /// </summary>
    protected virtual void OriginalDraw(SpriteBatch spriteBatch)
    {
        var overflowHidden = OverflowHidden;
        var useImmediateMode = UseImmediateMode;

        var anisotropicClamp = SamplerState.AnisotropicClamp;
        if (useImmediateMode || OverrideSamplerState != null)
        {
            spriteBatch.End();
            spriteBatch.Begin(useImmediateMode ? SpriteSortMode.Immediate : SpriteSortMode.Deferred,
                BlendState.AlphaBlend, OverrideSamplerState ?? anisotropicClamp, DepthStencilState.None,
                OverflowHiddenRasterizerState, null, FinalMatrix);
            DrawSelf(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None,
                OverflowHiddenRasterizerState, null, FinalMatrix);
        }
        else
        {
            DrawSelf(spriteBatch);
        }

        var scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
        if (overflowHidden)
        {
            spriteBatch.End();
            var scissorRectangle2 = Rectangle.Intersect(GetClippingRectangleFromView(spriteBatch),
                spriteBatch.GraphicsDevice.ScissorRectangle);
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
            spriteBatch.GraphicsDevice.RasterizerState = OverflowHiddenRasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None,
                OverflowHiddenRasterizerState, null, FinalMatrix);
        }

        DrawChildren(spriteBatch);

        if (overflowHidden)
        {
            var rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None,
                rasterizerState, null, FinalMatrix);
        }

        if (!FinallyDrawBorder || Border <= 0f || BorderColor == Color.Transparent) return;
        var position = GetDimensions().Position();
        var size = GetDimensions().Size();

        RoundedRectangle.DrawBorder(position, size, FinalMatrix);
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
}