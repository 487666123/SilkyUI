namespace SilkyUI.Helper;

public static class DrawHelper
{
    /// <summary>
    /// 绘制物品图标
    /// </summary>
    public static void DrawItemIcon(Item item, Color lightColor, Vector2 center,
        float maxSize = 32f, float itemScale = 1f)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;

        Main.instance.LoadItem(item.type);
        Texture2D texture2D = TextureAssets.Item[item.type].Value;
        Rectangle frame = Main.itemAnimations[item.type] is null
            ? texture2D.Frame()
            : Main.itemAnimations[item.type].GetFrame(texture2D);
        itemScale *= frame.Width > maxSize || frame.Height > maxSize
            ? frame.Width > frame.Height ? maxSize / frame.Width : maxSize / frame.Height
            : 1f;
        Vector2 origin = frame.Size() / 2f;
        if (ItemLoader.PreDrawInInventory(item, spriteBatch, center, frame, item.GetAlpha(lightColor),
                item.GetColor(lightColor), origin, itemScale))
        {
            spriteBatch.Draw(texture2D, center, frame, item.GetAlpha(lightColor), 0f, origin, itemScale,
                SpriteEffects.None, 0f);
            if (item.color != Color.Transparent)
                spriteBatch.Draw(texture2D, center, frame, item.GetColor(lightColor), 0f, origin, itemScale,
                    SpriteEffects.None, 0f);
        }

        ItemLoader.PostDrawInInventory(item, spriteBatch, center, frame, item.GetAlpha(lightColor),
            item.GetColor(lightColor), origin, itemScale);

        if (ItemID.Sets.TrapSigned[item.type])
            Main.spriteBatch.Draw(TextureAssets.Wire.Value, center + new Vector2(14f) * itemScale,
                new Rectangle(4, 58, 8, 8), lightColor, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);

        if (ItemID.Sets.DrawUnsafeIndicator[item.type])
        {
            Vector2 vector2 = new Vector2(-4f, -4f) * itemScale;
            Texture2D value7 = TextureAssets.Extra[258].Value;
            Rectangle rectangle2 = value7.Frame();
            Main.spriteBatch.Draw(value7, center + vector2 + new Vector2(14f) * itemScale, rectangle2, lightColor, 0f,
                rectangle2.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        if (item.type is ItemID.RubblemakerSmall or ItemID.RubblemakerMedium or ItemID.RubblemakerLarge)
        {
            Vector2 vector3 = new Vector2(2f, -6f) * itemScale;
            switch (item.type)
            {
                case 5324:
                    {
                        Texture2D value10 = TextureAssets.Extra[257].Value;
                        Rectangle rectangle5 = value10.Frame(3, 1, 2);
                        Main.spriteBatch.Draw(value10, center + vector3 + new Vector2(16f) * itemScale, rectangle5,
                            lightColor, 0f, rectangle5.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        break;
                    }
                case 5329:
                    {
                        Texture2D value9 = TextureAssets.Extra[257].Value;
                        Rectangle rectangle4 = value9.Frame(3, 1, 1);
                        Main.spriteBatch.Draw(value9, center + vector3 + new Vector2(16f) * itemScale, rectangle4,
                            lightColor, 0f, rectangle4.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        break;
                    }
                case 5330:
                    {
                        Texture2D value8 = TextureAssets.Extra[257].Value;
                        Rectangle rectangle3 = value8.Frame(3);
                        Main.spriteBatch.Draw(value8, center + vector3 + new Vector2(16f) * itemScale, rectangle3,
                            lightColor, 0f, rectangle3.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        break;
                    }
            }
        }
    }

    public static void DrawString(Vector2 position, string text, Color textColor, Color borderColor, float scale = 1f,
        bool large = false, float spread = 2f)
    {
        DrawString(position, text, textColor, borderColor, Vector2.Zero, scale, large, spread);
    }

    public static void DrawString(Vector2 pos, string text, Color textColor, Color borderColor, Vector2 origin,
        float textScale, bool large, float spread = 2f)
    {
        DynamicSpriteFont spriteFont = (large ? FontAssets.DeathText : FontAssets.MouseText).Value;

        float x = pos.X;
        float y = pos.Y;
        Color color = borderColor;
        float border = spread * textScale;

        if (borderColor == Color.Transparent)
        {
            Main.spriteBatch.DrawString(spriteFont, text, pos, textColor, 0f, origin, textScale, 0, 0f);
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    pos.X = x - border;
                    pos.Y = y;
                    break;
                case 1:
                    pos.X = x + border;
                    pos.Y = y;
                    break;
                case 2:
                    pos.X = x;
                    pos.Y = y - border;
                    break;
                case 3:
                    pos.X = x;
                    pos.Y = y + border;
                    break;
                default:
                    pos.X = x;
                    pos.Y = y;
                    color = textColor;
                    break;
            }

            Main.spriteBatch.DrawString(spriteFont, text, pos, color, 0f, origin, textScale, 0, 0f);
        }
    }
}