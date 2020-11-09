using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.FileWindow
{
    class FileWinVM
    {
        public FileWinVM()
        {
            Tags.Add(new TagVM { Name = "n1" });
            Tags.Add(new TagVM { Name = "n2" });
            Tags.Add(new TagVM { Name = "n3" });
        }

        public ObservableCollection<TreeItemVM> Tags { get; } = new ObservableCollection<TreeItemVM>();
        //public ObservableCollection<string> Tags { get; } = new ObservableCollection<string>();
    }

    interface TreeItemVM
    {
        string Name { get; set; }
        ObservableCollection<FileVM> Files { get; }
    }

    class TagVM : TreeItemVM
    {
        public string Name { get; set; }

        public ObservableCollection<FileVM> Files => throw new NotImplementedException();
    }

    class FileVM
    {
        public string Filename { get; }
    }
}
