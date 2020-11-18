using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UI.FileWindow
{
    public class SelectableFileTag : INotifyPropertyChanged
    {
        public SelectableFileTag(string tagName, bool isSelected = false)
        {
            TagName = tagName;
            IsSelected = isSelected;
        }

        public string TagName { get; private set; }

        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
