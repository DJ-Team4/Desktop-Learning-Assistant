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
using Panuon.UI.Silver;

namespace UI.FileWindow
{
    /// <summary>
    /// AddFileDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddFileDialog : WindowX
    {
        public AddFileDialog(IEnumerable<string> allTagNames)
        {
            InitializeComponent();
            DataContext = this;
            foreach (string tagName in allTagNames)
                FileTags.Add(new SelectableFileTag(tagName));
        }

        public AddFileDialog(string filepath)
        {
            InitializeComponent();
            DataContext = this;
            filepathTxtbox.Text = filepath;
            //TODO get recommend
        }

        /// <summary>
        /// 给标签列表绑定数据用的集合
        /// </summary>
        public ObservableCollection<SelectableFileTag> FileTags { get; }
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
            foreach (var stag in FileTags)
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
                if (!System.IO.File.Exists(filepath))
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

        private readonly ITagFileService service = TagFileService.GetService();
    }
}
