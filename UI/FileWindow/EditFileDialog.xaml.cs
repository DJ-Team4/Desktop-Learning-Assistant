using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UI.FileWindow
{
    /// <summary>
    /// EditFileDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditFileDialog : Window
    {
        public EditFileDialog(FileInfoForEdit fileInfo, IEnumerable<string> allTagName)
        {
            InitializeComponent();
            FileInfo = fileInfo;
            DataContext = this;
            //fill FileTag Collection
            foreach (string tagName in allTagName)
            {
                FileTags.Add(new SelectableFileTag
                {
                    IsSelected = fileInfo.TagNames.Contains(tagName),
                    TagName = tagName
                });
            }
        }

        /// <summary>
        /// 结果：文件信息
        /// </summary>
        public FileInfoForEdit FileInfo { get; private set; }

        /// <summary>
        /// 给 ListView 绑定用的集合
        /// </summary>
        public ObservableCollection<SelectableFileTag> FileTags { get; }
            = new ObservableCollection<SelectableFileTag>();

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            string filename = filenameTxtbox.Text;
            if (filename == null || filename.Trim().Length == 0
                || !IsFilenameValid(filename))
            {
                DialogResult = false;
            }
            else
            {
                DialogResult = true;
                FileInfo.Filename = filename.Trim();
                FileInfo.TagNames.Clear();
                foreach (var stag in FileTags)
                    if (stag.IsSelected)
                        FileInfo.TagNames.Add(stag.TagName);
            }
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static bool IsFilenameValid(string filename)
        {
            var invalidChars = new HashSet<char>();
            foreach (char ch in System.IO.Path.GetInvalidFileNameChars())
                invalidChars.Add(ch);
            foreach (char ch in filename)
                if (invalidChars.Contains(ch))
                    return false;
            return true;
        }
    }
}
