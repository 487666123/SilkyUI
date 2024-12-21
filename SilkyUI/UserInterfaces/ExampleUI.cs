using OneOf;
using SilkyUI.BasicComponents;

namespace SilkyUI.UserInterfaces;

[AutoloadUserInterface("Vanilla: Radial Hotbars", "SilkyUI: ExampleUI")]
public class ExampleUI : BasicBody
{
    public override bool IsEnabled { get; set; } = true;
    public override bool CanSetFocusTarget(UIElement target) => target != this;

    public SUIDraggableView MainPanel { get; private set; }

    public override void OnInitialize()
    {
        var backgroundColor = new Color(63, 65, 151);
        var borderColor = new Color(18, 18, 38);

        MainPanel = new SUIDraggableView(backgroundColor * 0.75f, borderColor * 0.75f, draggable: true)
        {
            Display = Display.InlineFlex,
            FlexDirection = FlexDirection.Column,
            MainAxisAlignment = MainAxisAlignment.Start,
            Shaded = true,
            ShadowThickness = 50f,
            ShadowColor = borderColor * 0.2f,
            Border = 2f,
            Gap = new Vector2(12f)
        }.Join(this);
        MainPanel.SetPadding(12f);

        var container1 = new View
        {
            Display = Display.InlineFlex,
            FlexDirection = FlexDirection.Row,
            MainAxisAlignment = MainAxisAlignment.Center,
            CrossAxisAlignment = CrossAxisAlignment.Center,
            BgColor = Color.Black * 0.25f,
            Gap = new Vector2(12f)
        }.Join(MainPanel);
        container1.SetWidth(500f);

        var box1 = new SUIText
        {
            IsWrapped = true,
            TextOrKey = GetType().FullName,
            BgColor = Color.White * 0.5f
        }.Join(container1);
        box1.SetSize(100f, 100f);

        var box2 = new View
            { BgColor = Color.White * 0.5f }.Join(container1);
        box2.SetSize(100f, 50f);

        var box3 = new View
            { BgColor = Color.White * 0.5f }.Join(container1);
        box3.SetSize(100f, 20f);

        var box4 = new View
            { BgColor = Color.White * 0.5f }.Join(container1);
        box4.SetSize(100f, 100f);

        var container2 = new View
        {
            Display = Display.InlineFlex,
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
            Display = Display.InlineFlex,
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

    public static void SetMatrixScaleAnimation(View view, float target)
    {
        if (view == null)
            return;

        var center = view.GetDimensions().Center();

        var beforeTransformCenter = Vector2.Transform(center, view.TransformMatrix);

        var scale = view.HoverTimer.Lerp(1f, 1f + target);
        view.TransformMatrix *= Matrix.CreateScale(scale, scale, 1f);

        var afterTransformCenter = Vector2.Transform(center, view.TransformMatrix);

        var offset = beforeTransformCenter - afterTransformCenter;
        view.TransformMatrix *= Matrix.CreateTranslation(offset.X, offset.Y, 0f);
    }
}