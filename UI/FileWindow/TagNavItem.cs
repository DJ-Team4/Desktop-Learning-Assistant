using DesktopLearningAssistant.TagFile.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.FileWindow
{
    /// <summary>
    /// 标签条目
    /// </summary>
    public class TagNavItem : INavItem, INotifyPropertyChanged
    {
        public TagNavItem(Tag tag) => Tag = tag;

        public string Header { get => Tag.TagName; }

        public Tag Tag { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyHeaderChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Header"));
        }
    }
}
