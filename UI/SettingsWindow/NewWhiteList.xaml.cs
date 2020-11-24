using Panuon.UI.Silver;
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

namespace UI.SettingsWindow
{
    /// <summary>
    /// NewWhiteList.xaml 的交互逻辑
    /// </summary>
    public partial class NewWhiteList : WindowX
    {
        public string WhiteListName { get; set; }

        public NewWhiteList()
        {
            InitializeComponent();
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WhiteListNameTextBox.Text == null || WhiteListNameTextBox.Text == "")
            {
                MessageBox.Show("未输入白名单名称");
                DialogResult = false;
                Close();
            }
            else
            {
                WhiteListName = WhiteListNameTextBox.Text;
                DialogResult = true;
                Close();
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
