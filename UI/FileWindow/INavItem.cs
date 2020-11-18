using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.FileWindow
{
    /// <summary>
    /// 导航栏条目
    /// </summary>
    public interface INavItem
    {
        string Header { get; }
    }
}
