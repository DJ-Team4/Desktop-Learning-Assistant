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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace UI.FileWindow
{
    /// <summary>
    /// AddFileDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddFileDialog : WindowX, INotifyPropertyChanged
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="allTagNames">所有标签</param>
        public AddFileDialog(IEnumerable<string> allTagNames)
        {
            InitializeComponent();
            DataContext = this;
            foreach (string tagName in allTagNames)
                FileTags.Add(new SelectableFileTag(tagName));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filepath">文件路径</param>
        public AddFileDialog(string filepath)
        {
            InitializeComponent();
            DataContext = this;
            Filepath = filepath;
            Task.Run(async () =>
            {
                await FillTagsFromServiceAsync();;
                await UpdateRecommendationAsync(Filepath);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// 给标签列表绑定数据用的集合
        /// </summary>
        public ObservableCollection<SelectableFileTag> FileTags { get; }
            = new ObservableCollection<SelectableFileTag>();

        private Visibility recommendVisibility = Visibility.Collapsed;
        public Visibility RecommendVisibility
        {
            get => recommendVisibility;
            private set
            {
                recommendVisibility = value;
                OnPropertyChanged();
            }
        }

        private string mfilepath;
        public string Filepath
        {
            get => mfilepath;
            set
            {
                mfilepath = value;
                OnPropertyChanged();
            }
        }

        private string tagsStr;
        public string TagsStr
        {
            get => tagsStr;
            set
            {
                tagsStr = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 执行添加文件
        /// </summary>
        public async Task AddFileAsync()
        {
            //Get the result
            string filepath = System.IO.Path.GetFullPath(Filepath);
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

        private async Task UpdateRecommendationAsync(string filepath)
        {
            List<Tag> tags;
            try
            {
                tags = await service.RecommendTagAsync(filepath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "读取文件时出错");
                return;
            }
            var showSet = new HashSet<string>();
            foreach (Tag tag in tags.Take(MAX_RECOMMEND))
                showSet.Add(tag.TagName);
            foreach (var stag in FileTags)
                stag.IsSelected = showSet.Contains(stag.TagName);

            if (tags.Count == 0)
                RecommendVisibility = Visibility.Collapsed;
            else
            {
                RecommendVisibility = Visibility.Visible;
                var sb = new StringBuilder();
                bool isfirst = true;
                foreach (Tag tag in tags.Take(MAX_RECOMMEND))
                {
                    if (isfirst)
                        isfirst = false;
                    else
                        sb.Append(", ");
                    sb.Append(tag.TagName);
                }
                TagsStr = sb.ToString();
            }
        }

        /// <summary>
        /// 从标签名集合填充所有标签列表，在 UI 线程执行
        /// </summary>
        private void FillTagsFromNamesInUiThread(IEnumerable<string> allTagNames)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (string tagName in allTagNames)
                    FileTags.Add(new SelectableFileTag(tagName));
            });
        }

        /// <summary>
        /// 查询并填充所有标签列表
        /// </summary>
        private async Task FillTagsFromServiceAsync()
        {
            var tagNames = new List<string>();
            (await service.TagListAsync()).ForEach(tag => tagNames.Add(tag.TagName));
            tagNames.Sort();
            FillTagsFromNamesInUiThread(tagNames);
        }

        /// <summary>
        /// 确定
        /// </summary>
        private async void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Filepath == null || Filepath.Trim().Length == 0)
            {
                DialogResult = false;
                Close();
            }
            else
            {
                if (!System.IO.File.Exists(Filepath))
                {
                    MessageBox.Show($"文件 {Filepath} 不存在", "文件不存在");
                    return;
                }
                try
                {
                    await AddFileAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "添加文件时出错");
                    return;
                }
                DialogResult = true;
                Close();
            }
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
        private async void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Multiselect = false;
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Filepath = dialog.FileName;
                    await UpdateRecommendationAsync(Filepath);
                }
            }
        }

        private readonly ITagFileService service = TagFileService.GetService();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// 最大推荐数
        /// </summary>
        private const int MAX_RECOMMEND = 5;
    }
}
