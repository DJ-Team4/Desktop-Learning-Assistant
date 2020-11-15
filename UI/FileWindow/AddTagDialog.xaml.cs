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
    /// AddTagDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddTagDialog : Window
    {
        public AddTagDialog()
        {
            InitializeComponent();
        }

        public string TagName { get; private set; }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            string tagName = tagNameTxtbox.Text;
            if (tagName == null || tagName.Trim().Length == 0)
            {
                DialogResult = false;
            }
            else
            {
                DialogResult = true;
                TagName = tagName.Trim();
            }
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
