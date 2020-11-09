using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace UI
{
    /// <summary>
    /// NewTaskWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewTaskWindow : Window
    {
        [DllImport("user32.dll")] public static extern int MessageBoxTimeoutA(IntPtr hWnd, string msg, string Caps, int type, int Id, int time); //引用DLL

        public NewTaskWindow()
        {
            InitializeComponent();
        }

        private void NewTask_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxTimeoutA((IntPtr)0, "创建任务成功", "提示", 0, 0, 1000); // 直接调用 1秒

        }
    }
}
