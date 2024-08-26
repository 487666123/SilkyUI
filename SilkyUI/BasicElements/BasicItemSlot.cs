﻿namespace SilkyUI.BasicElements;

public class BasicItemSlot : View
{
    protected Item AirItem = new();

    public virtual Item Item
    {
        get => AirItem;
        set => AirItem = value;
    }

    /// <summary>
    /// 允许显示物品信息
    /// </summary>
    public bool DisplayItemInfo;

    /// <summary>
    /// 允许显示物品堆叠数量
    /// </summary>
    public bool DisplayItemStack;

    /// <summary>
    /// 总是显示物品堆叠数量，即使堆叠数量为 1
    /// </summary>
    public bool AlwaysDisplayItemStack;

    public float ItemIconMaxWidthAndHeight = 32f;
    public float ItemIconScale = 1f;

    public Color ItemColor = Color.White;
    public Vector2 ItemIconOffset;
    public Vector2 ItemIconResize;

    public BasicItemSlot(UserInterfaceStyle style)
    {
        this.SetPositionPixels(52f, 52f);
        BgColor = style.GetColor("BackgroundColor");
        BorderColor = style.GetColor("BackgroundColor");
        Rounded = new Vector4(style.GetFloat("SmallRounded"));
    }

    /// <summary>
    /// 设置 BaseItemSlot 基本属性
    /// </summary>
    /// <param name="displayItemInfo"></param>
    /// <param name="displayItemStack"></param>
    public void SetBasicProperties(bool displayItemInfo, bool displayItemStack)
    {
        DisplayItemInfo = displayItemInfo;
        DisplayItemStack = displayItemStack;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (DisplayItemInfo && IsMouseHovering)
        {
            Main.hoverItemName = Item.Name;
            Main.HoverItem = Item.Clone();
        }

        var innerDimensions = GetInnerDimensions();
        innerDimensions.X += ItemIconOffset.X;
        innerDimensions.Y += ItemIconOffset.Y;
        innerDimensions.Width += ItemIconResize.X;
        innerDimensions.Height += ItemIconResize.Y;

        DrawHelper.DrawItemIcon(Item, ItemColor, innerDimensions.Center(), ItemIconMaxWidthAndHeight, ItemIconScale);

        if (AlwaysDisplayItemStack || DisplayItemStack && Item.stack > 1)
        {
            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensions().Size();

            Vector2 textSize = FontAssets.ItemStack.Value.MeasureString(Item.stack.ToString()) * 0.75f * ItemIconScale;
            Vector2 textPos = pos + new Vector2(size.X * 0.18f, (size.Y - textSize.Y) * 0.9f);
            DrawHelper.DrawString(textPos, Item.stack.ToString(), Color.White, Color.Black, 0.75f * ItemIconScale);
        }
    }
}