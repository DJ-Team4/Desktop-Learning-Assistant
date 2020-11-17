using DesktopLearningAssistant.TagFile;
using DesktopLearningAssistant.TagFile.Model;
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

        public AddFileDialog(string filepath)
        {
            InitializeComponent();
            DataContext = this;
            filepathTxtbox.Text = filepath;
            //TODO get recommend
        }

        /// <summary>
        /// 给下拉框绑定数据用的列表
        /// </summary>
        public ObservableCollection<SelectableFileTag> TagListForComboBox { get; }
            = new ObservableCollection<SelectableFileTag>();

        /// <summary>
        /// 执行添加文件
        /// </summary>
        public async Task AddFileAsync()
        {
            //Get the result
            string filepath = System.IO.Path.GetFullPath(filepathTxtbox.Text);
            bool asShortcut = asShortcutRadio.IsChecked.GetValueOrDefault(true);
            var tagNames = new List<string>();
            foreach (var stag in TagListForComboBox)
                if (stag.IsSelected)
                    tagNames.Add(stag.TagName);

            FileItem fileItem = asShortcut ? await service.AddShortcutToRepoAsync(filepath)
                                           : await service.MoveFileToRepoAsync(filepath);
            foreach (string tagName in tagNames)
            {
                Tag tag = await service.GetTagByNameAsync(tagName);
                if (tag != null)
                    await service.AddRelationAsync(tag, fileItem);
            }
        }

        private void UpdateTagTxt()
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

        /// <summary>
        /// 确定
        /// </summary>
        private async void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filepathTxtbox.Text;
            if (filepath == null || filepath.Trim().Length == 0)
            {
                DialogResult = false;
            }
            else
            {
                if(!System.IO.File.Exists(filepath))
                {
                    MessageBox.Show($"文件 {filepath} 不存在", "文件不存在");
                    return;
                }
                await AddFileAsync();
                DialogResult = true;
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
                //TODO update recommend
            }
        }

        private void TagComboBox_DropDownClosed(object sender, EventArgs e)
        {
            editTagBtn.Content = "编辑标签";
            UpdateTagTxt();
        }

        private void EditTagBtn_Click(object sender, RoutedEventArgs e)
        {
            tagComboBox.IsDropDownOpen = !tagComboBox.IsDropDownOpen;
        }

        private void TagComboBox_DropDownOpened(object sender, EventArgs e)
        {
            editTagBtn.Content = "收起";
        }

        private readonly ITagFileService service = TagFileService.GetService();
    }
}
