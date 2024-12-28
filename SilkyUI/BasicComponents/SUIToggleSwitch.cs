using SilkyUI.Animation;

namespace SilkyUI.BasicComponents;

/// <summary>
/// 拨动开关
/// </summary>
public class SUIToggleSwitch : View
{
    /// <summary>
    /// 状态
    /// </summary>
    public event Func<bool> Status;

    /// <summary>
    /// 切换
    /// </summary>
    public event Action Switch;

    /// <summary>
    /// 拨动圆的颜色
    /// </summary>
    public Color ToggleCircleColor;

    public readonly AnimationTimer SwitchTimer = new();

    public SUIToggleSwitch()
    {
        SetPadding(4f);

        ToggleCircleColor = new Color(18, 18, 38);
        Border = 2f;
        BorderColor = new Color(18, 18, 38);
    }

    public void Toggle()
    {
        Switch?.Invoke();
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        if (evt.Target == this)
            Toggle();

        base.LeftMouseDown(evt);
    }

    public override void Update(GameTime gameTime)
    {
        if (Status?.Invoke() ?? false)
        {
            if (!SwitchTimer.IsForward)
            {
                SwitchTimer.StartForwardUpdate();
            }
        }
        else
        {
            if (!SwitchTimer.IsReverse)
            {
                SwitchTimer.StartReverseUpdate();
            }
        }

        SwitchTimer.Update();
        base.Update(gameTime);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 innerPos = GetInnerDimensions().Position();
        Vector2 innerSize = GetInnerDimensions().Size();

        float circleSize = Math.Min(innerSize.X, innerSize.Y);

        SDFGraphics.NoBorderRound(innerPos + (innerSize - new Vector2(circleSize)) * SwitchTimer,
            circleSize, ToggleCircleColor, FinalMatrix);
    }
}