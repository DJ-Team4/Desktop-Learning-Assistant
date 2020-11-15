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

        public async Task<List<string>> AllTagNamesAsync()
        {
            var tags = await service.TagListAsync();
            var tagNames = new List<string>();
            foreach (Tag tag in tags)
                tagNames.Add(tag.TagName);
            return tagNames;
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
        /// 要显示的文件
        /// </summary>
        public ObservableCollection<FileVM> Files
        {
            get
            {
                var fileItems = FilesFromNavAsync().Result;//TODO async
                return FileItemsToFileVMs(fileItems);
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

        private ObservableCollection<TagNavItem> TagsToTagNavItems(List<Tag> tags)
        {
            tags.Sort((t1, t2) => t1.TagName.CompareTo(t2.TagName));
            var tagNavs = new ObservableCollection<TagNavItem>();
            foreach (Tag tag in tags)
                tagNavs.Add(new TagNavItem(tag));
            return tagNavs;
        }

        /// <summary>
        /// 刷新文件区的文件集合
        /// </summary>
        private void RefreshFiles()
        {
            OnFilesChanged();
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
                return new List<FileItem>();//TODO return search result
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
        /// 导航栏被选中条目改变
        /// </summary>
        private void OnSelectedNavItemChanged()
        {
            OnPropertyChanged("SelectedNavItem");
            OnFilesChanged();
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
        public FileVM(FileItem fileItem) => this.fileItem = fileItem;

        public string Name { get => fileItem.DisplayName; }

        private readonly FileItem fileItem;
    }
}
