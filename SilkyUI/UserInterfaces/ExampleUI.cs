﻿using OneOf;
using SilkyUI.Animation;
using SilkyUI.BasicComponents;

namespace SilkyUI.UserInterfaces;

[Attributes.Autoload("Vanilla: Radial Hotbars", "SilkyUI: ExampleUI")]
public class ExampleUI : BasicBody
{
    public SUIDraggableView MainPanel { get; private set; }

    private static readonly int[] RoshanBadges =
        [1, 4, 6, 7, 10, 489, 490, 491, 2998, 1, 4, 6, 7, 10, 489, 490, 491, 2998];

    public override void OnInitialize()
    {
        var backgroundColor = new Color(63, 65, 151);
        var borderColor = new Color(18, 18, 38);

        // 面板
        MainPanel = new SUIDraggableView(backgroundColor * 0.75f, borderColor * 0.75f, draggable: true)
        {
            Display = Display.Flexbox,
            FlexDirection = FlexDirection.Column,
            MainAxisAlignment = MainAxisAlignment.Start,
            Shaded = true,
            ShadowThickness = 50f,
            ShadowColor = borderColor * 0.2f,
            Border = 2f,
            Gap = new Vector2(12f),
            FinallyDrawBorder = true,
            CornerRadius = new Vector4(12f, 0f, 12f, 12f),
        }.Join(this);
        MainPanel.HAlign = MainPanel.VAlign = 0.5f;
        MainPanel.SetPadding(12f);

        var container1 = new View
        {
            BgColor = Color.Black * 0.25f, // 背景颜色
            Display = Display.Flexbox, // Flex 布局
            FlexDirection = FlexDirection.Row, // 主轴方向为: 行 
            MainAxisAlignment = MainAxisAlignment.Center, // 主轴 自结尾向前排列
            CrossAxisAlignment = CrossAxisAlignment.Center, // 交叉轴 居中
            Gap = new Vector2(12f), // 子元素间距
            Border = 2, // 边框宽度
            BorderColor = Color.Black * 0.75f, // 边框颜色
            CornerRadius = new Vector4(12f, 4f, 4f, 12f), // 圆角
            OverflowHidden = true,
            FlexWrap = true,
        }.Join(MainPanel);
        container1.SetWidth(500f);
        container1.SetPadding(12f); // 内边距

        // RoshanBadges = [489, 490, 491, 2998]
        foreach (var type in RoshanBadges)
        {
            // 创建并添加
            var img = new SUIImage(TextureAssets.Item[type].LoadItem().Value)
            {
                Border = 2f,
                BorderColor = borderColor * 0.25f,
                BgColor = backgroundColor * 0.25f,
                CornerRadius = new Vector4(8f),
                ImagePercent = new Vector2(0.5f),
                ImageOrigin = new Vector2(0.5f),
            };
            img.SetSize(42f, 42f);
            img.SetPadding(12f);
            img.OnDraw += _ =>
                ScalingAnimationUsingMatrix(img, img.HoverTimer, 0.1f);
            if (type == 1) img.OnLeftMouseDown += (_, _) => IsEnabled = false;

            container1.AppendFromView(img);
        }

        var container2 = new View
        {
            Display = Display.Flexbox,
            FlexDirection = FlexDirection.Row,
            MainAxisAlignment = MainAxisAlignment.SpaceEvenly,
            BgColor = Color.Black * 0.25f,
        }.Join(MainPanel);
        container2.SetWidth(500f);

        var box5 = new View
            { BgColor = Color.White * 0.5f }.Join(container2);
        box5.SetSize(100f, 40f);

        var box6 = new View
            { BgColor = Color.White * 0.5f }.Join(container2);
        box6.SetSize(100f, 100f);

        var box7 = new View
            { BgColor = Color.White * 0.5f }.Join(container2);
        box7.SetSize(100f, 80f);

        var box8 = new View
            { BgColor = Color.White * 0.5f }.Join(container2);
        box8.SetSize(100f, 100f);

        var container3 = new View
        {
            Display = Display.Flexbox,
            FlexDirection = FlexDirection.Row,
            MainAxisAlignment = MainAxisAlignment.SpaceBetween,
            CrossAxisAlignment = CrossAxisAlignment.End,
            BgColor = Color.Black * 0.25f,
        }.Join(MainPanel);
        container3.SetWidth(500f);

        var box9 = new View
            { BgColor = Color.White * 0.5f }.Join(container3);
        box9.SetSize(100f, 100f);

        var box10 = new View
            { BgColor = Color.White * 0.5f }.Join(container3);
        box10.SetSize(100f, 80f);

        var box11 = new View
            { BgColor = Color.White * 0.5f }.Join(container3);
        box11.SetSize(100f, 100f);

        var box12 = new View
            { BgColor = Color.White * 0.5f }.Join(container3);
        box12.SetSize(100f, 20f);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        UseRenderTarget = true;
        HoverTimer.Speed = 10f;
        Opacity = MainPanel.HoverTimer.Lerp(0.75f, 1f);
        base.Draw(spriteBatch);
    }

    /// <summary>
    /// 使用矩阵缩放动画
    /// </summary>
    public static void ScalingAnimationUsingMatrix(View view, AnimationTimer timer, float target)
    {
        if (view == null) return;
        var scale = 1f + (target = timer.Lerp(0f, target));
        var center = view.GetDimensions().Center();
        var offset = center * target;
        view.TransformMatrix =
            Matrix.CreateScale(scale, scale, 1f) *
            Matrix.CreateTranslation(-offset.X, -offset.Y, 0);
    }
}