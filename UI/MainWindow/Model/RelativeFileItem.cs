using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UI
{
    /// <summary>
    /// 最近打开文件的视图层模型
    /// </summary>
    public class RelativeFileItem
    {
        public ImageSource ImageSrc
        {
            get
            {
                if (!File.Exists(FilePath)) return new BitmapImage(new Uri("../Image/File.jpeg", UriKind.Relative));
                Icon ico = Icon.ExtractAssociatedIcon(FilePath);
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    ico.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
        }
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
