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
        public string FilePath { get; set; }
        public string FileName
        {
            get
            {
                List<string> tmp = new List<string>(FilePath.Split('\\'));
                if (tmp.Count < 1) return "";
                return tmp[tmp.Count - 1];
            }
        }
        
    }
}
