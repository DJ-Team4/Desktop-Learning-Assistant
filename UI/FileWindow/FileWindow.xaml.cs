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
using DesktopLearningAssistant.TagFile.Model;

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
            winVM = new FileWinVM();
            DataContext = winVM;
        }

        private FileWinVM winVM;

        private void AddTagBtn_Click(object sender, RoutedEventArgs e)
        {
            //TODO add tag click
        }

        private void RemoveTagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            winVM.RemoveSelectedTag();
        }
    }
}
