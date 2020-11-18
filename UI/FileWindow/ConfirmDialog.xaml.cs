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
using Panuon.UI.Silver;

namespace UI.FileWindow
{
    /// <summary>
    /// ConfirmDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmDialog : WindowX
    {
        public ConfirmDialog(string message, string caption)
        {
            InitializeComponent();
            Title = caption;
            msgTxt.Text = message;
            confirmBtn.Focus();
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
