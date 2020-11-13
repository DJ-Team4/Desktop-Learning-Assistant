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
using DesktopLearningAssistant.TaskTomato.Model;
using DesktopLearningAssistant.TaskTomato;
using UI.Tomato;

namespace UI
{
    /// <summary>
    /// TomatoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TomatoWindow : Window
    {
        public TaskInfo taskinfo;
        public TaskTomatoService tasksercive;

        public TomatoWindow()
        {
            InitializeComponent();
        }

        private void NewTaskWindow_Click(object sender, MouseButtonEventArgs e)
        {
            NewTaskWindow newTaskWindow = new NewTaskWindow();
            newTaskWindow.Show();
        }

        private void AllTasks_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AllTasksWindow allTasksWindow=new AllTasksWindow();
            allTasksWindow.Show();
        }

        private void SearchTask_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            taskinfo.TaskID = int.Parse(SearchBox.Text);
            tasksercive.GetTaskWithID(taskinfo.TaskID);
        }
    }
}
