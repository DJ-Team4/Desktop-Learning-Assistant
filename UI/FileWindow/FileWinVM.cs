using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DesktopLearningAssistant.TagFile.Model;
using DesktopLearningAssistant.TagFile;
using System.Diagnostics;

namespace UI.FileWindow
{
    public class FileWinVM : INotifyPropertyChanged
    {
        public FileWinVM(System.Windows.Window window)
        {
            this.window = window;

            UpNavItems.Add(new AllFilesNavItem());
            UpNavItems.Add(new NoTagNavItem());
            UpNavItems.Add(new SearchResultNavItem());
            SelectedNavItem = UpNavItems[0];
            //TODO obs colle
            Task.Run(async () =>
            {
                List<Tag> tags = await service.TagListAsync();
                this.window.Dispatcher.Invoke(() =>
                {
                    foreach (TagNavItem tagNav in TagsToTagNavItems(tags))
                        DownNavItems.Add(tagNav);
                });
            }).ConfigureAwait(false);
        }

        private readonly ITagFileService service = TagFileService.GetService();
        private readonly System.Windows.Window window;

        /// <summary>
        /// 导航栏上半部分
        /// </summary>
        public ObservableCollection<INavItem> UpNavItems { get; }
            = new ObservableCollection<INavItem>();

        /// <summary>
        /// 导航栏下半部分
        /// </summary>
        public ObservableCollection<INavItem> DownNavItems { get; }
            = new ObservableCollection<INavItem>();

        /// <summary>
        /// 导航栏选中的条目
        /// </summary>
        private INavItem selectedNavItem;
        public INavItem SelectedNavItem
        {
            get => selectedNavItem;
            set
            {
                selectedNavItem = value;
                OnSelectedNavItemChanged();
            }
        }

        /// <summary>
        /// 文件区选中的文件
        /// </summary>
        public FileVM SelectedFile { get; set; }

        /// <summary>
        /// 要显示的文件
        /// </summary>
        private ObservableCollection<FileVM> files = new ObservableCollection<FileVM>();
        public ObservableCollection<FileVM> Files
        {
            get => files;
            private set
            {
                files = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 当前选中的标签的名字。若选中的不是标签则返回 null。
        /// </summary>
        public string CurrentTagName
        {
            get
            {
                var tagNav = SelectedNavItem as TagNavItem;
                return tagNav?.Tag.TagName;
            }
        }

        /// <summary>
        /// 标签查询表达式
        /// </summary>
        private string tagSearchText = "";
        public string TagSearchText
        {
            get => tagSearchText;
            set
            {
                tagSearchText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 文件名搜索
        /// </summary>
        private string filenameSearchText = "";
        public string FilenameSearchText
        {
            get => filenameSearchText;
            set
            {
                filenameSearchText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 供智能提示使用的标签集合
        /// </summary>
        public ObservableCollection<IntelliItem> IntelliItems { get; }
            = new ObservableCollection<IntelliItem>();

        /// <summary>
        /// 添加标签。若标签已存在则什么都不做。
        /// </summary>
        public async Task AddTagAsync(string tagName)
        {
            if (await service.IsTagExistAsync(tagName))
                return;
            Tag tag = await service.AddTagAsync(tagName);
            window.Dispatcher.Invoke(() => DownNavItems.Add(new TagNavItem(tag)));
        }

        /// <summary>
        /// 重命名选中的标签，若重命名成功则刷新界面。
        /// </summary>
        public async Task RenameSelectedTagAsync(string newTagName)
        {
            if (!(SelectedNavItem is TagNavItem tagNav))
                return;
            bool needRefresh = await service.RenameTagAsync(tagNav.Tag, newTagName);
            if (needRefresh)
            {
                tagNav.NotifyHeaderChanged();
                await RefreshFilesAsync();
            }
        }

        /// <summary>
        /// 移除选中的标签。
        /// 若选中的条目不是标签条目则什么都不做。
        /// </summary>
        public async Task RemoveSelectedTagAsync()
        {
            if (!(SelectedNavItem is TagNavItem tagNav))
                return;
            await service.RemoveTagAsync(tagNav.Tag);
            window.Dispatcher.Invoke(() => DownNavItems.Remove(tagNav));
        }

        /// <summary>
        /// 所有标签的名称列表
        /// </summary>
        public async Task<List<string>> AllTagNamesAsync()
        {
            var tags = await service.TagListAsync();
            var tagNames = new List<string>();
            foreach (Tag tag in tags)
                tagNames.Add(tag.TagName);
            tagNames.Sort();
            return tagNames;
        }

        /// <summary>
        /// 打开选中的文件
        /// </summary>
        public void OpenSelectedFile()
        {
            SelectedFile?.FileItem.Open();
        }

        /// <summary>
        /// 在资源管理器中展示选中的文件
        /// </summary>
        public void ShowSelectedFileInExplorer()
        {
            SelectedFile?.FileItem.ShowInExplorer();
        }

        /// <summary>
        /// 将选中文件移动到回收站
        /// </summary>
        public async Task DeleteSelectedFileToRecycleBinAsync()
        {
            await SelectedFile?.FileItem.DeleteToRecycleBinAsync();
            window.Dispatcher.Invoke(() => Files.Remove(SelectedFile));
        }

        /// <summary>
        /// 将选中文件彻底删除
        /// </summary>
        public async Task DeleteSelectedFileAsync()
        {
            await SelectedFile?.FileItem.DeleteAsync();
            window.Dispatcher.Invoke(() => Files.Remove(SelectedFile));
        }

        /// <summary>
        /// 获取选中文件的信息
        /// </summary>
        /// <returns>若未选中则返回 null</returns>
        public FileInfoForEdit GetSelectedFileInfo()
        {
            FileVM file = SelectedFile;
            if (file == null)
                return null;
            var fileInfo = new FileInfoForEdit
            {
                Filename = file.FileItem.RealName,
                CreateAt = file.CreateAt,
                AccessAt = file.AccessAt
            };
            foreach (string tagName in file.TagNames)
                fileInfo.TagNames.Add(tagName);
            return fileInfo;
        }

        /// <summary>
        /// 更新文件信息：文件名、标签
        /// </summary>
        public async Task UpdateSelectedFileAsync(FileInfoForEdit fileInfo)
        {
            if (SelectedFile == null)
                return;
            //update tag
            var tags = new List<Tag>();
            foreach (string tagName in fileInfo.TagNames)
            {
                Tag tag = await service.GetTagByNameAsync(tagName);
                if (tag != null)
                    tags.Add(tag);
            }
            FileItem selectedFile = SelectedFile.FileItem;
            await service.UpdateFileRelationAsync(selectedFile, tags);
            //update filename
            await service.RenameFileItemAsync(selectedFile, fileInfo.Filename);
            //update UI
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].FileItem.FileItemId == selectedFile.FileItemId)
                {
                    Files[i] = new FileVM(selectedFile);
                    break;
                }
            }
        }

        /// <summary>
        /// 刷新文件区文件集合，完成后会自动通知界面刷新
        /// </summary>
        public void RefreshFilesThenNotify()
        {
            RefreshFilesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 刷新文件区的文件集合
        /// </summary>
        public async Task RefreshFilesAsync()
        {
            ObservableCollection<FileVM> observableFiles;
            try
            {
                observableFiles = FileItemsToFileVMs(await FilesFromNavAsync());
            }
            catch (InvalidExpressionException e)
            {
                //表达式查询可能出现非法表达式异常，这里暂时把异常吞掉，返回一个空列表
                Debug.WriteLine($"Exception in FileWinVM.RefreshFilesAsync(): {e.Message}");
                observableFiles = new ObservableCollection<FileVM>();
            }
            window.Dispatcher.Invoke(() => Files = observableFiles);
        }

        /// <summary>
        /// 转到搜索结果页面
        /// </summary>
        public void GoToSearchResult()
        {
            SelectedNavItem = UpNavItems[2];
        }

        /// <summary>
        /// 刷新智能提示列表
        /// </summary>
        /// <param name="prefix">标签前缀</param>
        public void RefreshIntelliItems(string prefix)
        {
            IntelliItems.Clear();
            foreach (var tagNav in DownNavItems)
            {
                string tagName = tagNav.Header;
                if (tagName.StartsWith(prefix))
                    IntelliItems.Add(new IntelliItem(tagName));
            }
        }

        /// <summary>
        /// 按照选中的条目获取 FileItem 列表
        /// </summary>
        private async Task<List<FileItem>> FilesFromNavAsync()
        {
            if (SelectedNavItem == null)
                return new List<FileItem>();
            if (SelectedNavItem is AllFilesNavItem)
                return await service.FileItemListAsync();
            else if (SelectedNavItem is NoTagNavItem)
                return await service.FilesWithoutTagAsync();
            else if (SelectedNavItem is SearchResultNavItem)
            {
                List<FileItem> fileItems =
                    (TagSearchText == null || TagSearchText.Trim().Length == 0)
                        ? await service.FileItemListAsync()
                        : await service.QueryAsync(TagSearchText);
                var result = new List<FileItem>();
                if (FilenameSearchText == null || FilenameSearchText.Length == 0)
                    fileItems.ForEach(fileItem => result.Add(fileItem));
                else
                {
                    foreach (var fileItem in fileItems)
                        if (fileItem.DisplayName.Contains(FilenameSearchText))
                            result.Add(fileItem);
                }
                return result;
            }
            else
            {
                Debug.Assert(SelectedNavItem is TagNavItem);
                Tag tag = (SelectedNavItem as TagNavItem).Tag;
                var fileItems = new List<FileItem>();
                foreach (var relation in tag.Relations)
                    fileItems.Add(relation.FileItem);
                return fileItems;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// 导航栏被选中条目改变。
        /// <list type="number">
        /// <item>通知 SelectedNavItem 属性改变。</item>
        /// <item>刷新文件区。</item>
        /// <item>若选中了标签，则设置表达式栏。</item>
        /// </list>
        /// </summary>
        private void OnSelectedNavItemChanged()
        {
            OnPropertyChanged("SelectedNavItem");
            RefreshFilesThenNotify();
            if (SelectedNavItem is TagNavItem)
            {
                var sb = new StringBuilder();
                sb.Append('"');
                foreach (char ch in CurrentTagName)
                {
                    if (ch == '"')
                        sb.Append('\\');
                    sb.Append(ch);
                }
                sb.Append('"');
                TagSearchText = sb.ToString();
                FilenameSearchText = "";
            }
            else if (SelectedNavItem is AllFilesNavItem
                  || SelectedNavItem is NoTagNavItem)
            {
                TagSearchText = "";
                FilenameSearchText = "";
            }
        }

        /// <summary>
        /// 将 Tag 列表转换为 TagNavItem 集合，按照标签名字排序
        /// </summary>
        private static ObservableCollection<TagNavItem> TagsToTagNavItems(List<Tag> tags)
        {
            tags.Sort((t1, t2) => t1.TagName.CompareTo(t2.TagName));
            var tagNavs = new ObservableCollection<TagNavItem>();
            foreach (Tag tag in tags)
                tagNavs.Add(new TagNavItem(tag));
            return tagNavs;
        }

        /// <summary>
        /// 将 FileItem 列表转换为 FileVM 集合，按照显示的文件名排序
        /// </summary>
        private static ObservableCollection<FileVM> FileItemsToFileVMs(
            List<FileItem> fileItems)
        {
            //按显示的文件名排序
            fileItems.Sort(
                (f1, f2) => f1.DisplayName.CompareTo(f2.DisplayName));
            var fileVMs = new ObservableCollection<FileVM>();
            foreach (FileItem fileItem in fileItems)
                fileVMs.Add(new FileVM(fileItem));
            return fileVMs;
        }
    }
}
