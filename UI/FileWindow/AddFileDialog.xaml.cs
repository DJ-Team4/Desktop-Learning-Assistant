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
    /// AddFileDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddFileDialog : Window
    {
        public AddFileDialog(IEnumerable<string> allTagNames)
        {
            InitializeComponent();
            DataContext = this;
            foreach (string tagName in allTagNames)
            {
                TagListForComboBox.Add(new SelectableFileTag
                {
                    TagName = tagName,
                    IsSelected = false
                });
            }
        }

        /// <summary>
        /// 结果：文件路径
        /// </summary>
        public string Filepath { get; private set; }

        /// <summary>
        /// 结果：是否使用快捷方式
        /// </summary>
        public bool AsShortcut { get; private set; }

        /// <summary>
        /// 结果：要添加的标签
        /// </summary>
        public ICollection<string> TagNames { get; private set; }

        /// <summary>
        /// 给下拉框绑定数据用的列表
        /// </summary>
        public ObservableCollection<SelectableFileTag> TagListForComboBox { get; }
            = new ObservableCollection<SelectableFileTag>();

        /// <summary>
        /// 确定
        /// </summary>
        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filepathTxtbox.Text;
            if (filepath == null || filepath.Trim().Length == 0)
            {
                DialogResult = false;
            }
            else
            {
                DialogResult = true;
                //Set the result.
                Filepath = System.IO.Path.GetFullPath(filepath);
                AsShortcut = asShortcutRadio.IsChecked.GetValueOrDefault(true);
                TagNames = new List<string>();
                foreach (var stag in TagListForComboBox)
                    if (stag.IsSelected)
                        TagNames.Add(stag.TagName);
            }
            Close();
        }

        /// <summary>
        /// 取消
        /// </summary>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Multiselect = false;
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string filepath = dialog.FileName;
                    filepathTxtbox.Text = filepath;
                }
            }
        }

        private void TagComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            bool isfirst = true;
            foreach (var stag in TagListForComboBox)
            {
                if (stag.IsSelected)
                {
                    if (isfirst)
                        isfirst = false;
                    else
                        sb.Append(", ");
                    sb.Append(stag.TagName);
                }
            }
            tagTxt.Text = sb.ToString();
        }

        private void EditTagBtn_Click(object sender, RoutedEventArgs e)
        {
            tagComboBox.IsDropDownOpen = !tagComboBox.IsDropDownOpen;
        }
    }
}
