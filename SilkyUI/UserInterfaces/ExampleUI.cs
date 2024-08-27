using SilkyUI.BasicComponents;

namespace SilkyUI.UserInterfaces;

[AutoloadUserInterface("Vanilla: Radial Hotbars", "SilkyUI: ExampleUI")]
public class ExampleUI : BasicBody
{
    public override bool Enabled { get; set; } = true;
    public override bool CanSetFocusTarget(UIElement target) => target != this;

    public SUIDraggableView MainPanel { get; private set; }

    public List<View> Views { get; private set; } = [];

    public override void OnInitialize()
    {
        var style = SilkyUserInterfaceSystem.Instance.UserInterfaceStyleManager.Style;

        var backgroundColor = style.GetColor("BackgroundColor");
        var borderColor = style.GetColor("BorderColor");
        var smallRounded = style.GetFloat("SmallRounded");

        MainPanel = new SUIDraggableView(
            backgroundColor * 0.75f, borderColor * 0.75f, draggable: true)
        {
            Shaded = true,
            ShadowThickness = 50f,
            ShadowColor = borderColor * 0.2f,
            FlowDirection = FlowDirection.Row,
        };
        MainPanel.SetPadding(10f);
        MainPanel.Join(this);

        var view1 = new View
        {
            SpecifyWidth = true,
            SpecifyHeight = true,
            Width = { Pixels = 100 },
            Height = { Pixels = 100 },
            BgColor = Color.White * 0.5f
        };
        Views.Add(view1);
        view1.Join(MainPanel);

        var view2 = new View
        {
            SpecifyWidth = true,
            SpecifyHeight = true,
            Width = { Pixels = 150 },
            Height = { Pixels = 150 },
            BgColor = Color.White * 0.5f
        };
        Views.Add(view2);
        view2.Join(MainPanel);

        var view3 = new View
        {
            SpecifyWidth = true,
            SpecifyHeight = true,
            Width = { Pixels = 200 },
            Height = { Pixels = 200 },
            BgColor = Color.White * 0.5f
        };
        Views.Add(view3);
        view3.Join(MainPanel);

        var view4 = new View
        {
            SpecifyWidth = false,
            SpecifyHeight = false,
            Width = { Pixels = 250 },
            Height = { Pixels = 250 },
            BgColor = Color.White * 0.5f,
        };
        view4.SetPadding(10);
        view4.OnUpdateTransformMatrix += (view) =>
        {
            SetMatrixScaleAnimation(view, 0.1f);
        };
        Views.Add(view4);
        view4.Join(MainPanel);

        var view5 = new View
        {
            SpecifyWidth = true,
            SpecifyHeight = true,
            Width = { Pixels = 50 },
            Height = { Pixels = 50 },
            BgColor = Color.White * 0.5f
        };
        Views.Add(view5);
        view5.Join(view4);

        var view6 = new View
        {
            SpecifyWidth = false,
            SpecifyHeight = false,
            Width = { Pixels = 100 },
            Height = { Pixels = 100 },
            BgColor = Color.White * 0.5f
        };
        view6.SetPadding(10);
        view6.OnUpdateTransformMatrix += (view) =>
        {
            SetMatrixScaleAnimation(view, 0.1f);
        };
        Views.Add(view6);
        view6.Join(view4);

        var view7 = new View
        {
            SpecifyWidth = true,
            SpecifyHeight = true,
            Width = { Pixels = 100 },
            Height = { Pixels = 500 },
            BgColor = Color.White * 0.5f
        };
        Views.Add(view7);
        view7.Join(view6);

        var view8 = new View
        {
            SpecifyWidth = true,
            SpecifyHeight = true,
            Width = { Pixels = 100 },
            Height = { Pixels = 100 },
            BgColor = Color.White * 0.5f
        };
        Views.Add(view8);
        view8.Join(view6);

        var view9 = new View
        {
            SpecifyWidth = true,
            SpecifyHeight = true,
            Width = { Pixels = 250 },
            Height = { Pixels = 0, Percent = 0.5f },
            BgColor = Color.Red * 0.5f,
            Positioning = Positioning.Absolute
        };
        Views.Add(view9);
        view9.Join(MainPanel);

        var view10 = new View
        {
            SpecifyWidth = false,
            SpecifyHeight = false,
            Width = { Pixels = 150 },
            Height = { Pixels = 150 },
            BgColor = Color.Red * 0.5f,
            Positioning = Positioning.Absolute,
            HAlign = 0.5f,
            VAlign = 0.5f,
        };
        view10.SetPadding(10);
        view10.OnUpdateTransformMatrix += (view) =>
        {
            SetMatrixScaleAnimation(view, 0.1f);
        };
        Views.Add(view10);
        view10.Join(MainPanel);

        var view11 = new View
        {
            SpecifyWidth = true,
            SpecifyHeight = true,
            Width = { Pixels = 150 },
            Height = { Pixels = 150 },
            BgColor = Color.White * 0.5f,
            Positioning = Positioning.Relative,
            HAlign = 0.5f,
            VAlign = 0.5f,
        };
        view11.SetPadding(10);
        Views.Add(view11);
        view11.Join(view10);

        var view12 = new View
        {
            SpecifyWidth = true,
            SpecifyHeight = true,
            Width = { Pixels = 150 },
            Height = { Pixels = 150 },
            BgColor = Color.White * 0.5f,
            Positioning = Positioning.Relative,
            HAlign = 0.5f,
            VAlign = 0.5f
        };
        Views.Add(view12);
        view12.Join(view10);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
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