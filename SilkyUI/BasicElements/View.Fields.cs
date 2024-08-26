using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkyUI.BasicElements;

public partial class View : UIElement
{
    public Vector4 Rounded = new(0f);
    public float Border = 2f;
    public Color BgColor = Color.Transparent;
    public Color BorderColor = Color.Transparent;

    public Matrix TransformMatrix = Matrix.Identity;
}
