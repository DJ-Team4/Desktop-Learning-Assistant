using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UI.FileWindow
{
    public class FileWinVM : INotifyPropertyChanged
    {
        public FileWinVM()
        {
            FillNavItems();

        }

        /// <summary>
        /// 填充导航栏的数据
        /// </summary>
        public void FillNavItems()
        {
            UpNavItems.Add(new NavItem { Header = "全部" });
            UpNavItems.Add(new NavItem { Header = "无标签" });
            UpNavItems.Add(new NavItem { Header = "搜索结果" });
            SelectedNavItem = UpNavItems[0];
            DownNavItems.Add(new NavItem { Header = "t1" });
            DownNavItems.Add(new NavItem { Header = "t2" });
            DownNavItems.Add(new NavItem { Header = "t3" });
            DownNavItems[0].Files.Add(new FileVM { Name = "a" });
            DownNavItems[1].Files.Add(new FileVM { Name = "b" });
            DownNavItems[2].Files.Add(new FileVM { Name = "c" });
            //TODO sort down nav items
        }

        /// <summary>
        /// 移除选中的标签
        /// </summary>
        public void RemoveSelectedTag()
        {
            //TODO remove tag
            System.Diagnostics.Debug.WriteLine("remove tag");
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        public void AddTag()
        {

        }

        /// <summary>
        /// 导航栏上半部分
        /// </summary>
        public ObservableCollection<NavItem> UpNavItems { get; private set; }
            = new ObservableCollection<NavItem>();

        /// <summary>
        /// 导航栏下半部分
        /// </summary>
        public ObservableCollection<NavItem> DownNavItems { get; private set; }
            = new ObservableCollection<NavItem>();

        /// <summary>
        /// 导航栏选中的条目
        /// </summary>
        private NavItem selectedNavItem;
        public NavItem SelectedNavItem
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
        public ObservableCollection<FileVM> Files => selectedNavItem.Files;

        #region 属性发生改变

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// 导航栏被选中条目改变
        /// </summary>
        private void OnSelectedNavItemChanged()
        {
            OnPropertyChanged("SelectedNavItem");
            OnPropertyChanged("Files");
        }

        #endregion

    }

    public class NavItem
    {
        public string Header { get; set; }
        public ObservableCollection<FileVM> Files { get; private set; }
            = new ObservableCollection<FileVM>();
        public NavItem()
        {
            Files.Add(new FileVM { Name = "f1" });
            Files.Add(new FileVM { Name = "f2" });
            Files.Add(new FileVM { Name = "f3" });
        }
    }

    public class FileVM
    {
        public string Name { get; set; }
    }
}
