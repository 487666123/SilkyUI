namespace SilkyUI.BasicElements;

/// <summary>
/// 简单说明: 2024.8.26 <br/>
/// 只有 View 能用, 别的都不建议用. <br/>
/// 使用固定宽高必须设置 <see cref="SpecifyWidth"/> <see cref="SpecifyHeight"/> 为 true <br/>
/// 所有 UI 元素都必须基于 <see cref="View"/> <br/>
/// </summary>
public partial class View
{
    public View()
    {
        MaxWidth = MaxHeight = new StyleDimension(114514f, 0f);

        OnLeftMouseDown += (_, _) => LeftMousePressed = true;
        OnRightMouseDown += (_, _) => RightMousePressed = true;
        OnMiddleMouseDown += (_, _) => MiddleMousePressed = true;

        OnLeftMouseUp += (_, _) => LeftMousePressed = false;
        OnRightMouseUp += (_, _) => RightMousePressed = false;
        OnMiddleMouseUp += (_, _) => MiddleMousePressed = false;
    }

    /// <summary>
    /// 获取父元素 <see cref="CalculatedStyle"/><br/>
    /// 绝对定位下与原版相同<br/>
    /// 相对定位下, 父元素不是 <see cref="View"/> 子类与原版相同<br/>
    /// 父元素是 <see cref="View"/> 且父元素 <see cref="SpecifyWidth"/> <see cref="SpecifyHeight"/>
    /// 为 false, 则不使用父元素大小转而设为 0<br/>
    /// 且会根据父元素 <see cref="FlexDirection"/> 决定 <see cref="CalculatedStyle"/> 的 XY,
    /// 排列自身位置<br/>
    /// 位置与 <see cref="UIElementExtensions.PreviousRelativeElement(UIElement)"/> 相关
    /// </summary>
    /// <returns></returns>
    public virtual CalculatedStyle GetParentDimensions()
    {
        // 没有父元素
        if (Parent is null)
            return SilkyUIHelper.GetBasicBodyDimensions();

        // 父元素不是 View 类则直接返回父元素 innerDimensions
        if (Parent is not View parent)
            return Parent.GetInnerDimensions();

        var container = parent.GetInnerDimensions();

        // 可能是负数, 所以要设为 0
        if (!parent.SpecifyWidth) container.Width = 0;
        if (!parent.SpecifyHeight) container.Height = 0;

        return container;
    }

    public override void Recalculate()
    {
        var parentDimensions = GetParentDimensions();

        _outerDimensions = CalculateOuterDimensionsByParentDimensions(parentDimensions);
        _dimensions = CalculateDimensions(_outerDimensions);
        _innerDimensions = CalculateInnerDimensions(_dimensions);

        RecalculateChildren();
    }

    /// <summary>
    /// 计算属性，基于父元素尺寸<br/>
    /// 不使用 <see cref="UIElement.GetDimensionsBasedOnParentDimensions(CalculatedStyle)"/>
    /// </summary>
    public virtual CalculatedStyle CalculateOuterDimensionsByParentDimensions(CalculatedStyle parentDimensions)
    {
        var left = Left.GetValue(parentDimensions.Width) + parentDimensions.X;
        var top = Top.GetValue(parentDimensions.Height) + parentDimensions.Y;

        var width = SpecifyWidth
            ? MathHelper.Clamp(Width.GetValue(parentDimensions.Width),
                MinWidth.GetValue(parentDimensions.Width),
                MaxWidth.GetValue(parentDimensions.Width))
            : 0;
        var height = SpecifyHeight
            ? MathHelper.Clamp(Height.GetValue(parentDimensions.Height),
                MinHeight.GetValue(parentDimensions.Height),
                MaxHeight.GetValue(parentDimensions.Height))
            : 0;

        var result = BoxSizing switch
        {
            BoxSizing.BorderBox => new CalculatedStyle(
                left, top,
                width + this.HMargin(),
                height + this.VMargin()),
            BoxSizing.ContentBox => new CalculatedStyle(
                left, top,
                width + this.HPadding() + this.HMargin() + Border * 2f,
                height + this.VPadding() + this.VMargin() + Border * 2f),
            _ => new CalculatedStyle()
        };

        if (IsAbsolute)
        {
            result.X += (parentDimensions.Width - result.Width) * HAlign;
            result.Y += (parentDimensions.Height - result.Height) * VAlign;
        }
        else if (IsRelative && Parent is View view)
        {
            if (view.SpecifyWidth)
                result.X += (parentDimensions.Width - result.Width) * HAlign;
            if (view.SpecifyHeight)
                result.Y += (parentDimensions.Height - result.Height) * VAlign;
        }

        // if (IsRelativePosition && Parent is View { Display: Display.InlineBlock } parent)
        // {
        //     var preElement = this.PreviousRelativeElement();
        //     var preElementRightBottom = preElement._outerDimensions.RightBottom();
        //
        //     if (parent.SpecifyWidth)
        //     {
        //         if (preElementRightBottom.X + result.Width > parentDimensions.Right())
        //         {
        //             result.X = parentDimensions.X;
        //             result.Y = preElementRightBottom.Y;
        //         }
        //         else
        //         {
        //             result.X = (result.X - parentDimensions.X) + preElementRightBottom.X;
        //         }
        //     }
        //     else
        //     {
        //         result.X = preElementRightBottom.X +
        //     }
        // }

        return result;
    }

    #region CalculateDimensions

    public CalculatedStyle CalculateDimensions(CalculatedStyle outerDimensions)
    {
        var dimensions = outerDimensions;
        dimensions.X += MarginLeft;
        dimensions.Y += MarginTop;
        dimensions.Width -= MarginLeft + MarginRight;
        dimensions.Height -= MarginTop + MarginBottom;
        return dimensions;
    }

    public CalculatedStyle CalculateInnerDimensions(CalculatedStyle dimensions)
    {
        var innerDimensions = dimensions;
        innerDimensions.X += PaddingLeft + Border;
        innerDimensions.Y += PaddingTop + Border;
        innerDimensions.Width -= this.HPadding() + Border;
        innerDimensions.Height -= this.VPadding() + Border;
        return innerDimensions;
    }

    #endregion

    /// <summary>
    /// 重新计算子元素
    /// </summary>
    public override void RecalculateChildren()
    {
        // 计算子元素前先分类
        ClassifyElements();

        FlowElements.ForEach(element => element.Recalculate());

        if (Display is Display.InlineFlex)
            CalculateFlexLayout();

        if (Parent is View parent)
        {
            var start = _innerDimensions.Position();
            var end = FlowElements.Aggregate(start,
                (current, element) => Vector2.Max(current, element.GetOuterDimensions().RightBottom()));

            var content = end - start;

            if (content is { X: > 0 } or { Y: > 0 })
            {
                SecondRecalculate(parent, content);
            }
        }

        // 绝对定位
        NonFlowElements.ForEach(element => element.Recalculate());
    }

    /// <summary>
    /// 计算位置与偏移
    /// </summary>
    public virtual void SecondRecalculate(View parent, Vector2 content)
    {
        var offset = new Vector2();

        if (!SpecifyWidth)
        {
            _outerDimensions.Width =
                content.X + this.HMargin() + this.HPadding() + Border * 2;

            if (Position is Position.Absolute ||
                (Position is Position.Relative && parent.SpecifyWidth))
            {
                offset.X = -_outerDimensions.Width * HAlign;
            }
        }

        if (!SpecifyHeight)
        {
            _outerDimensions.Height =
                content.Y + this.VMargin() + this.VPadding() + Border * 2;

            if (Position is Position.Absolute ||
                (Position is Position.Relative && parent.SpecifyHeight))
            {
                offset.Y = -_outerDimensions.Height * VAlign;
            }
        }

        if (offset.X != 0)
            if (offset.Y != 0) this.DimensionsOffset(offset);
            else this.DimensionsOffsetX(offset.X);
        else if (offset.Y != 0) this.DimensionsOffsetY(offset.Y);

        _dimensions = CalculateDimensions(_outerDimensions);
        _innerDimensions = CalculateInnerDimensions(_dimensions);
    }

    /// <summary>
    /// 判断点是否在元素内, 会计算<see cref="TransformMatrix"/>
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public override bool ContainsPoint(Vector2 point) =>
        base.ContainsPoint(Vector2.Transform(point, Matrix.Invert(TransformMatrix)));
}

public struct ElementFlow()
{
    public readonly List<UIElement> FlowElements = [];
    public float Start { get; set; }
    public float End { get; set; }
    public float Width { get; set; }
}

#region 垃圾

public abstract class BasicUnit
{
    public abstract float GetValue(float parentDimension);

    public override bool Equals(object obj)
    {
        return obj is BasicUnit other && other.GetType() == GetType();
    }

    public override int GetHashCode()
    {
        return this.GetType().GetHashCode();
    }
}

public class PixelsUnit : BasicUnit
{
    public float Value;

    public PixelsUnit(float value)
    {
        Value = value;
    }

    public override float GetValue(float parentDimension)
    {
        return Value;
    }
}

public class PercentUnit : BasicUnit
{
    public float Value;

    public PercentUnit(float value)
    {
        Value = value;
    }

    public override float GetValue(float parentDimension)
    {
        return parentDimension * Value;
    }
}

public class MiddleUnit
{
    private HashSet<BasicUnit> _basicUnits = [];

    public void Set(BasicUnit basicUnit)
    {
        if (_basicUnits == null || !_basicUnits.Contains(basicUnit)) return;
        _basicUnits.Remove(basicUnit);
        _basicUnits.Add(basicUnit);
    }

    public float GetValue(float dimensions)
    {
        return _basicUnits.Sum(basicUnit => basicUnit.GetValue(dimensions));
    }
}

#endregion