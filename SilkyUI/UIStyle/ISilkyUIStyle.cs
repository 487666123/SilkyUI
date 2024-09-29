using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkyUI.UIStyle;

public interface ISilkyUIStyle
{
    /// <summary>
    /// 初始化时调用
    /// </summary>
    void OnInitialize();

    /// <summary>
    /// 更改主题时调用
    /// </summary>
    void OnChangeStyle(Mod mod, String styleName);
}