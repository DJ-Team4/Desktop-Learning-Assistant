using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TagFile;
using DesktopLearningAssistant.TagFile.Model;

namespace UI.FileWindow
{
    /// <summary>
    /// 文件 View Model
    /// </summary>
    public class FileVM
    {
        public FileVM(FileItem fileItem)
        {
            FileItem = fileItem;
            RealPath = fileItem.RealPath();
            var tagLst = new List<string>();
            foreach (var relation in fileItem.Relations)
                tagLst.Add(relation.Tag.TagName);
            tagLst.Sort();
            foreach (string tagName in tagLst)
                TagNames.Add(tagName);
        }

        /// <summary>
        /// 显示在界面上的名字
        /// </summary>
        public string DisplayName
        {
            get => IsShortcut
                       ? System.IO.Path.GetFileNameWithoutExtension(FileItem.DisplayName)
                       : FileItem.DisplayName;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateAt
        {
            get
            {
                DateTime createAt = System.IO.File.GetCreationTime(RealPath);
                return createAt.ToString();
            }
        }

        /// <summary>
        /// 访问时间
        /// </summary>
        public string AccessAt
        {
            get
            {
                DateTime accessAt = System.IO.File.GetLastAccessTime(RealPath);
                return accessAt.ToString();
            }
        }

        /// <summary>
        /// 含有的标签的名字
        /// </summary>
        public ObservableCollection<string> TagNames { get; }
            = new ObservableCollection<string>();

        /// <summary>
        /// 文件图标
        /// </summary>
        public System.Windows.Media.ImageSource IconSrc
        {
            get
            {
                System.Drawing.Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(RealPath);
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    ico.Handle,
                    System.Windows.Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
        }

        /// <summary>
        /// 真实路径
        /// </summary>
        public string RealPath { get; private set; }

        /// <summary>
        /// 是否为快捷方式
        /// </summary>
        public bool IsShortcut
        {
            get => System.IO.Path.GetExtension(FileItem.DisplayName) == ".lnk";
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize
        {
            get
            {
                long byteSize = new System.IO.FileInfo(RealPath).Length;
                if (byteSize < (1 << 10))
                    return $"{byteSize} B";
                else if (byteSize < (1 << 20))
                {
                    double kb = byteSize / (double)(1 << 10);
                    return $"{kb:N2} KB";
                }
                else if (byteSize < (1 << 30))
                {
                    double mb = byteSize / (double)(1 << 20);
                    return $"{mb:N2} MB";
                }
                else
                {
                    double gb = byteSize / (double)(1 << 30);
                    return $"{gb:N2} GB";
                }
            }
        }

        public System.Windows.Visibility FileSizeVisibility
            => IsShortcut ? System.Windows.Visibility.Collapsed
                          : System.Windows.Visibility.Visible;

        public System.Windows.Visibility ShortcutVisibility
            => IsShortcut ? System.Windows.Visibility.Visible
                          : System.Windows.Visibility.Collapsed;

        /// <summary>
        /// 表示含有标签的字符串
        /// </summary>
        public string TagStr
        {
            get
            {
                var sb = new StringBuilder();
                bool isfirst = true;
                foreach(string tagName in TagNames)
                {
                    if (isfirst)
                        isfirst = false;
                    else
                        sb.Append(", ");
                    sb.Append(tagName);
                }
                return sb.ToString();
            }
        }

        public System.Windows.Visibility TagStrVisibility
            => TagNames.Count > 0 ? System.Windows.Visibility.Visible 
                                  : System.Windows.Visibility.Collapsed;

        /// <summary>
        /// 对应的 FileItem 实体类
        /// </summary>
        public FileItem FileItem { get; private set; }
    }
}
