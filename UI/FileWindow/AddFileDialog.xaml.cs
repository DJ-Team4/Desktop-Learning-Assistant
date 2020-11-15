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

namespace UI.FileWindow
{
    /// <summary>
    /// AddFileDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddFileDialog : Window
    {
        public AddFileDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 结果：文件路径
        /// </summary>
        public string Filepath { get; private set; }

        /// <summary>
        /// 结果：是否使用快捷方式
        /// </summary>
        public bool AsShortcut { get; set; }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            string filepath = filepathTxtbox.Text;
            if (filepath == null || filepath.Trim().Length == 0)
            {
                DialogResult = false;
            }
            else
            {
                DialogResult = true;
                Filepath = System.IO.Path.GetFullPath(filepath);
                AsShortcut = asShortcutRadio.IsChecked.GetValueOrDefault(true);
            }
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Multiselect = false;
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string filepath = dialog.FileName;
                    filepathTxtbox.Text = filepath;
                }
            }
        }
    }
}
