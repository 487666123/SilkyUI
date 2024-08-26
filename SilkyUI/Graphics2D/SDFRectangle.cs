namespace SilkyUI.Graphics2D;

public static class SDFRectangle
{
    public static EffectPass SpriteEffectPass { get; } = Main.spriteBatch.spriteEffectPass;
    public static GraphicsDevice GraphicsDevice { get; } = Main.graphics.GraphicsDevice;

    private static Effect Effect => ModAsset.SDFRectangle.Value;

    private static void BasicDrawRectangle(Vector2 position, Vector2 size, Vector4 rounded)
    {
        size /= 2f;

        List<SDFGraphicsVertexType> vertices = [];

        var coordQ1 = new Vector2(rounded.X) - size;
        var coordQ2 = new Vector2(rounded.X);
        vertices.SDFVertexTypeRectangle(position, size, coordQ2, coordQ1, rounded.X);

        coordQ1 = new Vector2(rounded.Y) - size;
        coordQ2 = new Vector2(rounded.Y);
        vertices.SDFVertexTypeRectangle(position + new Vector2(size.X, 0f), size, new Vector2(coordQ1.X, coordQ2.Y),
            new Vector2(coordQ2.X, coordQ1.Y), rounded.Y);

        coordQ1 = new Vector2(rounded.Z) - size;
        coordQ2 = new Vector2(rounded.Z);
        vertices.SDFVertexTypeRectangle(position + new Vector2(0f, size.Y), size, new Vector2(coordQ2.X, coordQ1.Y),
            new Vector2(coordQ1.X, coordQ2.Y), rounded.Z);

        coordQ1 = new Vector2(rounded.W) - size;
        coordQ2 = new Vector2(rounded.W);
        vertices.SDFVertexTypeRectangle(position + size, size, coordQ1, coordQ2, rounded.W);

        GraphicsDevice.DrawUserPrimitives(0, vertices.ToArray(), 0, vertices.Count / 3);

        SpriteEffectPass.Apply();
    }

    public static void HasBorder(Vector2 position, Vector2 size, Vector4 rounded, Color backgroundColor, float border,
        Color borderColor, Matrix matrix)
    {
        MatrixHelper.Transform2SDFMatrix(ref matrix);

        const float innerShrinkage = 1;
        position -= new Vector2(innerShrinkage);
        size += new Vector2(innerShrinkage * 2);
        rounded += new Vector4(innerShrinkage);
        var parameters = Effect.Parameters;
        parameters["uTransform"].SetValue(matrix);
        parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
        parameters["uBorder"].SetValue(border);
        parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        parameters["uInnerShrinkage"].SetValue(innerShrinkage);
        Effect.CurrentTechnique.Passes["HasBorder"].Apply();
        BasicDrawRectangle(position, size, rounded);
    }

    public static void NoBorder(Vector2 position, Vector2 size, Vector4 rounded, Color backgroundColor, Matrix matrix)
    {
        MatrixHelper.Transform2SDFMatrix(ref matrix);

        const float innerShrinkage = 1;
        position -= new Vector2(innerShrinkage);
        size += new Vector2(innerShrinkage * 2);
        rounded += new Vector4(innerShrinkage);
        Effect.Parameters["uTransform"].SetValue(matrix);
        Effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
        Effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
        Effect.CurrentTechnique.Passes["NoBorder"].Apply();
        BasicDrawRectangle(position, size, rounded);
    }

    public static void Shadow(Vector2 position, Vector2 size, Vector4 rounded,
        Color backgroundColor, float shadow, Matrix matrix)
    {
        MatrixHelper.Transform2SDFMatrix(ref matrix);

        Effect.Parameters["uTransform"].SetValue(matrix);
        Effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
        Effect.Parameters["uShadowSize"].SetValue(shadow);
        Effect.CurrentTechnique.Passes["Shadow"].Apply();
        BasicDrawRectangle(position, size, rounded + new Vector4(shadow));
    }
}