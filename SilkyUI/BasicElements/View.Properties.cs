using SilkyUI.Animation;

namespace SilkyUI.BasicElements;

public enum BoxSizing { BorderBox, ContentBox }

public enum Display { InlineFlex, InlineGrid }

public enum Positioning { Relative, Absolute }

public enum FlowDirection { Row, Column }

public partial class View : UIElement
{
    /// <summary>
    /// �������
    /// </summary>
    public List<float> GridTemplateRows { get; } = [];
    public List<float> GridTemplateColumns { get; } = [];

    /// <summary>
    /// ����Զ�λ
    /// </summary>
    public bool IsRelativePositioning => Positioning is Positioning.Relative;

    /// <summary>
    /// �Ǿ��Զ�λ
    /// </summary>
    public bool IsAbsolutePositioning => Positioning is Positioning.Absolute;

    public FlowDirection FlowDirection { get; set; } = FlowDirection.Row;

    public Display Display { get; set; } = Display.InlineFlex;

    public AnimationTimer HoverTimer { get; } = new();

    /// <summary>
    /// ����һ�л��ƶ�����֮���ٻ��Ʊ߿�
    /// </summary>
    public bool FinallyDrawBorder { get; set; } = false;

    /// <summary>
    /// ������ȫ���Ԫ��
    /// </summary>
    public bool HideFullyOverflowedElements { get; set; }

    /// <summary>
    /// ��������ģ�ͼ��㷽ʽ
    /// </summary>
    public BoxSizing BoxSizing { get; set; } = BoxSizing.BorderBox;

    /// <summary>
    /// Ԫ�ض�λ<br/>
    /// ���з� <see cref="View"/> ������Ԫ�ص� <see cref="UIElement"/> ��Ϊ <see cref="Positioning.Absolute"/><br/>
    /// ���Բ�������ʹ��ԭ����κ�Ԫ��<br/>
    /// �������Ԫ�ز�������������󣬿��������Ŀ issue ���ύ��� Pr
    /// </summary>
    public Positioning Positioning { get; set; } = Positioning.Relative;

    /// <summary>
    /// �϶����ԣ�Ĭ��Ϊ <see langword="false"/> ����Ӱ�쳤���п��϶�Ԫ���϶�
    /// </summary>
    public bool DragIgnore { get; set; }

    public bool LeftMouseButtonPressed { get; set; }
    public bool RightMouseButtonPressed { get; set; }

    /// <summary>
    /// ʹ��ָ���Ŀ��
    /// </summary>
    public bool SpecifyWidth { get; set; } = false;

    /// <summary>
    /// ʹ��ָ���ĸ߶�
    /// </summary>
    public bool SpecifyHeight { get; set; } = false;
}