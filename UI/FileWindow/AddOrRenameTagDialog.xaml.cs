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
    /// AddTagDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddOrRenameTagDialog : WindowX
    {
        /// <summary>
        /// 结果：输入的标签名
        /// </summary>
        public string TagName { get; private set; }

        /// <summary>
        /// 创建一个添加标签的对话框
        /// </summary>
        public static AddOrRenameTagDialog MakeAddTagDialog()
        {
            var dialog = new AddOrRenameTagDialog
            {
                Title = "添加标签"
            };
            dialog.tagNameTxtbox.Focus();
            return dialog;
        }

        /// <summary>
        /// 创建一个重命名标签的对话框
        /// </summary>
        /// <param name="tagName">当前标签名</param>
        public static AddOrRenameTagDialog MakeRenameTagDialog(string tagName)
        {
            var dialog = new AddOrRenameTagDialog
            {
                Title = "重命名标签"
            };
            dialog.tagNameTxtbox.Text = tagName;
            dialog.tagNameTxtbox.Focus();
            dialog.tagNameTxtbox.SelectAll();
            return dialog;
        }

        private AddOrRenameTagDialog()
        {
            InitializeComponent();
        }

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
