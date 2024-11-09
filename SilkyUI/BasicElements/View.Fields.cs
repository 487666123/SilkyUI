using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkyUI.BasicElements;

public partial class View
{
    public RoundedRectangle RoundedRectangle = new();

    public Matrix TransformMatrix = Matrix.Identity;

    public Vector2 Gap;
}