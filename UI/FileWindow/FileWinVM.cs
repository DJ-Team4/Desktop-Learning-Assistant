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
using System.Windows.Media;
using System.Windows.Interop;
using System.Drawing;

namespace UI.FileWindow
{
    public class FileWinVM : INotifyPropertyChanged
    {
        public FileWinVM()
        {
            service = TagFileService.GetService();
            FillNavItems();

        }

        private readonly ITagFileService service;

        /// <summary>
        /// 填充导航栏的数据
        /// </summary>
        public void FillNavItems()
        {
            UpNavItems.Add(new AllFilesNavItem());
            UpNavItems.Add(new NoTagNavItem());
            UpNavItems.Add(new SearchResultNavItem());
            SelectedNavItem = UpNavItems[0];
            List<Tag> tags = service.TagListAsync().Result;//TODO async
            foreach (TagNavItem tagNav in TagsToTagNavItems(tags))
                DownNavItems.Add(tagNav);
        }

        /// <summary>
        /// 添加标签。若标签已存在则什么都不做。
        /// </summary>
        public async Task AddTagAsync(string tagName)
        {
            if (await service.IsTagExistAsync(tagName))
                return;
            Tag tag = await service.AddTagAsync(tagName);
            DownNavItems.Add(new TagNavItem(tag));
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
                RefreshFiles();
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
            DownNavItems.Remove(tagNav);
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="asShortcut">是否添加快捷方式</param>
        /// <param name="tagNames">要添加的标签</param>
        public async Task AddFileAsync(string filepath,
                                       bool asShortcut,
                                       IEnumerable<string> tagNames)
        {
            FileItem fileItem;
            if (asShortcut)
                fileItem = await service.AddShortcutToRepoAsync(filepath);
            else
                fileItem = await service.MoveFileToRepoAsync(filepath);
            foreach (string tagName in tagNames)
            {
                Tag tag = await service.GetTagByNameAsync(tagName);
                if (tag != null)
                    await service.AddRelationAsync(tag, fileItem);
            }
            RefreshFiles();
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
            Debug.WriteLine(SelectedFile.FileItem.RealPath());
        }

        /// <summary>
        /// 将选中文件移动到回收站
        /// </summary>
        public async Task DeleteSelectedFileToRecycleBin()
        {
            await SelectedFile?.FileItem.DeleteToRecycleBinAsync();
            Files.Remove(SelectedFile);
            OnFilesChanged();
        }

        /// <summary>
        /// 将选中文件彻底删除
        /// </summary>
        public async Task DeleteSelectedFile()
        {
            await SelectedFile?.FileItem.DeleteAsync();
            Files.Remove(SelectedFile);
            OnFilesChanged();
        }

        /// <summary>
        /// 获取选中文件的信息
        /// </summary>
        /// <returns>若未选中则返回 null</returns>
        public FileInfoForEdit GetSelectedFileInfo()
        {
            if (SelectedFile == null)
                return null;
            var fileInfo = new FileInfoForEdit
            {
                Filename = SelectedFile.FileItem.RealName,
                CreateAt = SelectedFile.CreateAt,
                AccessAt = SelectedFile.AccessAt
            };
            foreach (string tagName in SelectedFile.TagNames)
                fileInfo.TagNames.Add(tagName);
            return fileInfo;
        }

        /// <summary>
        /// 更新文件信息：文件名、标签
        /// </summary>
        public async Task UpdateSelectedFile(FileInfoForEdit fileInfo)
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
            await service.UpdateFileRelationAsync(SelectedFile.FileItem, tags);
            //update filename
            await service.RenameFileItemAsync(SelectedFile.FileItem, fileInfo.Filename);
            RefreshFiles();
        }

        /// <summary>
        /// 刷新文件区的文件集合
        /// </summary>
        public void RefreshFiles()
        {
            OnFilesChanged();
        }

        /// <summary>
        /// 转到搜索结果页面
        /// </summary>
        public void GoToSearchResult()
        {
            SelectedNavItem = UpNavItems[2];
        }

        /// <summary>
        /// 导航栏上半部分
        /// </summary>
        public ObservableCollection<INavItem> UpNavItems { get; private set; }
            = new ObservableCollection<INavItem>();

        /// <summary>
        /// 导航栏下半部分
        /// </summary>
        public ObservableCollection<INavItem> DownNavItems { get; private set; }
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
        public ObservableCollection<FileVM> Files
        {
            get
            {
                try
                {
                    var fileItems = FilesFromNavAsync().Result;//TODO async
                    return FileItemsToFileVMs(fileItems);
                }
                catch (Exception e)
                {
                    //TODO 表达式查询可能出现非法表达式异常，这里暂时把异常吞掉，返回一个空列表
                    Debug.WriteLine($"Exception in FileWinVM.Files: {e.Message}");
                    return FileItemsToFileVMs(new List<FileItem>());
                }
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

        private ObservableCollection<TagNavItem> TagsToTagNavItems(List<Tag> tags)
        {
            tags.Sort((t1, t2) => t1.TagName.CompareTo(t2.TagName));
            var tagNavs = new ObservableCollection<TagNavItem>();
            foreach (Tag tag in tags)
                tagNavs.Add(new TagNavItem(tag));
            return tagNavs;
        }

        private ObservableCollection<FileVM> FileItemsToFileVMs(
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

        #region 属性发生改变

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// 导航栏被选中条目改变。
        /// 1. 通知 SelectedNavItem 属性改变。
        /// 2. 通知 Files 属性改变。
        /// 3. 若选中了标签，则设置表达式栏。
        /// </summary>
        private void OnSelectedNavItemChanged()
        {
            OnPropertyChanged("SelectedNavItem");
            OnFilesChanged();
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
        /// 显示的文件集合改变
        /// </summary>
        private void OnFilesChanged()
        {
            OnPropertyChanged("Files");
        }

        #endregion

    }

    /// <summary>
    /// 导航栏条目
    /// </summary>
    public interface INavItem
    {
        string Header { get; }
    }

    /// <summary>
    /// “所有文件条目”
    /// </summary>
    public class AllFilesNavItem : INavItem
    {
        public string Header { get => "所有文件"; }
    }

    /// <summary>
    /// 无标签文件条目
    /// </summary>
    public class NoTagNavItem : INavItem
    {
        public string Header { get => "无标签文件"; }
    }

    /// <summary>
    /// 搜索结果条目
    /// </summary>
    public class SearchResultNavItem : INavItem
    {
        public string Header { get => "搜索结果"; }
    }

    /// <summary>
    /// 标签条目
    /// </summary>
    public class TagNavItem : INavItem
    {
        public TagNavItem(Tag tag) => Tag = tag;

        public string Header { get => Tag.TagName; }

        public Tag Tag { get; private set; }
    }

    /// <summary>
    /// 文件 View Model
    /// </summary>
    public class FileVM
    {
        public FileVM(FileItem fileItem)
        {
            FileItem = fileItem;
            realPath = fileItem.RealPath();
            foreach (var relation in fileItem.Relations)
                TagNames.Add(relation.Tag.TagName);
        }

        //TODO display name remove .lnk
        public string DisplayName { get => FileItem.DisplayName; }

        public string CreateAt
        {
            get
            {
                DateTime createAt = System.IO.File.GetCreationTime(realPath);
                return createAt.ToString();
            }
        }

        public string AccessAt
        {
            get
            {
                DateTime accessAt = System.IO.File.GetLastAccessTime(realPath);
                return accessAt.ToString();
            }
        }

        public ObservableCollection<string> TagNames { get; }
            = new ObservableCollection<string>();

        public ImageSource IconSrc
        {
            get
            {
                Icon ico = Icon.ExtractAssociatedIcon(realPath);
                return Imaging.CreateBitmapSourceFromHIcon(
                    ico.Handle,
                    System.Windows.Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
        }

        public FileItem FileItem { get; private set; }
        private readonly string realPath;
    }
}
