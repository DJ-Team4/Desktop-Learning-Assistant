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
using Panuon.UI.Silver;
using DesktopLearningAssistant.TagFile;
using System.Diagnostics;

namespace UI.FileWindow
{
    /// <summary>
    /// TagWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FileWindow : WindowX
    {
        public FileWindow()
        {
            InitializeComponent();
            winVM = new FileWinVM(this);
            DataContext = winVM;
            SetAlignment();
        }

        private readonly FileWinVM winVM;

        /// <summary>
        /// 设置弹窗菜单对齐方式
        /// </summary>
        /// <remarks>
        /// 解决 Popup 位置在触摸屏电脑上左右颠倒的憨批行为。
        /// </remarks>
        private static void SetAlignment()
        {
            var ifLeft = SystemParameters.MenuDropAlignment;
            if (ifLeft)
            {
                var t = typeof(SystemParameters);
                var field = t.GetField("_menuDropAlignment",
                    System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Static);
                field?.SetValue(null, false);
            }
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        private async void AddTagBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = AddOrRenameTagDialog.MakeAddTagDialog();
            dialog.Owner = this;
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
            var dialog = new ConfirmDialog(
                $"你确定要删除标签 {winVM.CurrentTagName} 吗？", "删除标签");
            dialog.Owner = this;
            if (dialog.ShowDialog().GetValueOrDefault(false))
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
            dialog.Owner = this;
            if (dialog.ShowDialog().GetValueOrDefault(false))
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
            var dialog = new AddFileDialog(allTagNames)
            {
                Owner = this
            };
            if (dialog.ShowDialog().GetValueOrDefault(false))
                await winVM.RefreshFilesAsync();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            winVM.OpenSelectedFile();
        }

        /// <summary>
        /// 打开文件位置
        /// </summary>
        private void ShowFileInExplorerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            winVM.ShowSelectedFileInExplorer();
        }

        /// <summary>
        /// 编辑文件信息
        /// </summary>
        private async void EditFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var fileInfo = winVM.GetSelectedFileInfo();
            if (fileInfo == null)
                return;
            var dialog = new EditFileDialog(fileInfo, await winVM.AllTagNamesAsync());
            dialog.Owner = this;
            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                var newInfo = dialog.FileInfo;//same ref here
                await winVM.UpdateSelectedFileAsync(newInfo);
            }
        }

        /// <summary>
        /// 删除文件到回收站
        /// </summary>
        private async void DeleteFileToRecycleBinMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ConfirmDialog(
                $"你确定要移动此文件到回收站吗？", "删除文件");
            dialog.Owner = this;
            if (dialog.ShowDialog().GetValueOrDefault(false))
                await winVM.DeleteSelectedFileToRecycleBinAsync();
        }

        /// <summary>
        /// 彻底删除文件
        /// </summary>
        private async void DeleteFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ConfirmDialog(
                $"你确定要彻底删除此文件吗？", "删除文件");
            dialog.Owner = this;
            if (dialog.ShowDialog().GetValueOrDefault(false))
                await winVM.DeleteSelectedFileAsync();
        }

        /// <summary>
        /// 刷新文件集合页面
        /// </summary>
        private async void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            await winVM.RefreshFilesAsync();
        }

        /// <summary>
        /// 双击打开文件
        /// </summary>
        private void FileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            winVM.OpenSelectedFile();
        }

        /// <summary>
        /// 文件名搜索框按键事件
        /// </summary>
        private void FilenameSearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                winVM.TagSearchText = tagSearchBox.Text;
                winVM.FilenameSearchText = filenameSearchBox.Text;
                winVM.GoToSearchResult();
            }
        }

        /// <summary>
        /// 提示是否可用
        /// </summary>
        private bool IsPopupUsable()
        {
            return intelliPopup.IsOpen
                   && winVM.IntelliItems.Count > 0;
        }

        /// <summary>
        /// 把选中的提示放到输入框中，并关闭提示
        /// </summary>
        private void UseIntelliThenClose()
        {
            if (intelliLst.SelectedItem != null)
            {
                string tagName = IntelliUtils.EscapeTagName(
                    (intelliLst.SelectedItem as IntelliItem).Name);
                int prefixLen = IntelliUtils.ExtractPrefix(tagSearchBox.Text).Length;
                string textWithoutPrefix = tagSearchBox.Text.Substring(
                    0, tagSearchBox.Text.Length - prefixLen);
                tagSearchBox.Text = textWithoutPrefix + tagName + "\" ";
                tagSearchBox.CaretIndex = tagSearchBox.Text.Length;
            }
            intelliPopup.IsOpen = false;
        }

        /// <summary>
        /// 处理标签搜索框的 Enter, Tag, Esc, Up, Down, Back, Delete 按键事件
        /// </summary>
        private void TagSearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (IsPopupUsable()) //插入提示
                    UseIntelliThenClose();
                else //搜索
                {
                    winVM.TagSearchText = tagSearchBox.Text;
                    winVM.FilenameSearchText = filenameSearchBox.Text;
                    winVM.GoToSearchResult();
                }
            }
            else if (e.Key == Key.Tab)
            {
                if (IsPopupUsable())
                {
                    e.Handled = true; //屏蔽默认的 Tab 跳转行为
                    UseIntelliThenClose();
                }
            }
            else if (e.Key == Key.Escape)
            {
                intelliPopup.IsOpen = false;
            }
            else if (e.Key == Key.Up)
            {
                if (IsPopupUsable())
                {
                    int cnt = winVM.IntelliItems.Count;
                    intelliLst.SelectedIndex = (intelliLst.SelectedIndex - 1 + cnt) % cnt;
                }
            }
            else if (e.Key == Key.Down)
            {
                if (IsPopupUsable())
                {
                    intelliLst.SelectedIndex = (intelliLst.SelectedIndex + 1)
                                                % winVM.IntelliItems.Count;
                }
            }
            else if (e.Key == Key.Back || e.Key == Key.Delete)
                intelliPopup.IsOpen = false;
        }

        /// <summary>
        /// 是否发生了 PreviewTextInput 事件
        /// </summary>
        private bool previewTextInputHappend = false;

        /// <summary>
        /// 设置标志位，并处理自动补全右括号
        /// </summary>
        private void TagSearchBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            previewTextInputHappend = true;
            if (e.Text == "(") //处理自动补全右括号
            {
                int oldIndex = tagSearchBox.CaretIndex;
                string left = tagSearchBox.Text.Substring(0, oldIndex);
                string right = tagSearchBox.Text.Length > 0
                                    ? tagSearchBox.Text.Substring(oldIndex) : "";
                tagSearchBox.Text = left + ")" + right;
                tagSearchBox.CaretIndex = oldIndex;
            }
        }

        /// <summary>
        /// 处理补全提示的打开、更新、关闭
        /// </summary>
        /// <remarks>
        /// 不能在 PreviewTextInput 中处理，因为中英文输入行为不一致。
        /// </remarks>
        private void TagSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //需判断标志位，以免删除时触发
            if (!previewTextInputHappend)
                return;
            if (IntelliUtils.IsQuoteOdd(tagSearchBox.Text)
                && tagSearchBox.CaretIndex == tagSearchBox.Text.Length)
            {
                string prefix = IntelliUtils.ExtractPrefix(tagSearchBox.Text);
                winVM.RefreshIntelliItems(prefix);
                if (winVM.IntelliItems.Count > 0)
                {
                    intelliPopup.IsOpen = true;
                    intelliLst.SelectedIndex = 0;
                    if (prefix.Length == 0)
                        intelliPopup.PlacementRectangle =
                            tagSearchBox.GetRectFromCharacterIndex(
                                tagSearchBox.Text.Length);
                }
                else
                    intelliPopup.IsOpen = false;
            }
            else
                intelliPopup.IsOpen = false;
            previewTextInputHappend = false;
        }

        /// <summary>
        /// 提示列表按键事件
        /// </summary>
        private void IntelliLst_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                intelliPopup.IsOpen = false;
                tagSearchBox.Focus();
            }
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (IsPopupUsable())
                    UseIntelliThenClose();
                intelliPopup.IsOpen = false;
                tagSearchBox.Focus();
            }
        }

        /// <summary>
        /// 提示列表双击
        /// </summary>
        private void IntelliLst_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsPopupUsable())
                UseIntelliThenClose();
            intelliPopup.IsOpen = false;
            tagSearchBox.Focus();
        }
    }
}
