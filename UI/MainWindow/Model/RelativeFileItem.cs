using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    /// <summary>
    /// 最近打开文件的视图层模型
    /// </summary>
    public class RelativeFileItem
    {
        public Image IconImage { get; set; }
        public string FileName { get; set; }
    }
}
