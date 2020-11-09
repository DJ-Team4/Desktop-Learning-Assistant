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
        }

        public string NewTag { get; set; }

        private async Task AddTag()
        {            

            Tag tag = await TagFileService.GetService().AddTagAsync("newtagName");
       
        }

        private void AddFile_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
