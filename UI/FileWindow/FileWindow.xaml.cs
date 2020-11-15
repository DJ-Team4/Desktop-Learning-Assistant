using System;
using System.Collections.Generic;
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
using DesktopLearningAssistant.TagFile;

namespace UI.FileWindow
{
    /// <summary>
    /// TagWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FileWindow : Window
    {
        public FileWindow()
        {
            InitializeComponent();
            TagFileService.EnsureDbAndFolderCreated();//TODO move it
            winVM = new FileWinVM();
            DataContext = winVM;
        }

        private readonly FileWinVM winVM;

        /// <summary>
        /// 添加标签
        /// </summary>
        private async void AddTagBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = AddOrRenameTagDialog.MakeAddTagDialog();
            bool result = dialog.ShowDialog().GetValueOrDefault(false);
            if (result)
            {
                string tagName = dialog.TagName;
                await winVM.AddTagAsync(tagName);
            }
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        private async void RemoveTagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await winVM.RemoveSelectedTagAsync();
        }

        /// <summary>
        /// 重命名标签
        /// </summary>
        private async void RenameTagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string currentName = winVM.CurrentTagName;
            if (currentName == null)
                return;
            var dialog = AddOrRenameTagDialog.MakeRenameTagDialog(currentName);
            bool result = dialog.ShowDialog().GetValueOrDefault(false);
            if (result)
            {
                string newTagName = dialog.TagName;
                await winVM.RenameSelectedTagAsync(newTagName);
            }
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        private async void AddFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var allTagNames = await winVM.AllTagNamesAsync();
            var dialog = new AddFileDialog(allTagNames);
            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                string filepath = dialog.Filepath;
                bool asShortcut = dialog.AsShortcut;
                ICollection<string> tagNames = dialog.TagNames;
                try
                {
                    await winVM.AddFileAsync(filepath, asShortcut, tagNames);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "添加文件时出错");
                }
            }
        }

        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            winVM.OpenSelectedFile();
        }

        private void ShowFileInExplorerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            winVM.ShowSelectedFileInExplorer();
        }

        private async void EditFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var fileInfo = winVM.GetSelectedFileInfo();
            if (fileInfo == null)
                return;
            var dialog = new EditFileDialog(fileInfo, await winVM.AllTagNamesAsync());
            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                var newInfo = dialog.FileInfo;//same ref here
                await winVM.UpdateSelectedFile(newInfo);
            }
        }

        private async void DeleteFileToRecycleBinMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //TODO delete dialog
            await winVM.DeleteSelectedFileToRecycleBin();
        }

        private async void DeleteFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //TODO delete dialog
            await winVM.DeleteSelectedFile();
        }
    }
}
