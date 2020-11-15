using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace UI.Tomato
{
    /// <summary>
    /// AllTasksWindow.xaml 的交互逻辑
    /// </summary>
    ///

    public partial class AllTasksWindow : Window
    {
        TaskTomatoService tts = TaskTomatoService.GetTimeStatisticService();
        TaskInfo taskInfo = new TaskInfo();
        
        

        private Image Unfinishedimg;
        private Image Finishedimg;
    
        public AllTasksWindow()
        {
            InitializeComponent();
            List<TaskItem> items = new List<TaskItem>();

            Unfinishedimg.Source = new BitmapImage(new Uri(@"../Icon/tomatounfinished.png", UriKind.Relative));
            Finishedimg.Source = new BitmapImage(new Uri(@"../Icon/tomatoufinished.png", UriKind.Relative));



            items.Add(new TaskItem()
            {
                ID =taskInfo.TaskID, Name = taskInfo.Name,
                State = taskInfo.Finished,
                StartTime = taskInfo.StartTime.ToString(),
                DeadLine = taskInfo.EndTime.ToString(),

                finishedTomato = taskInfo.FinishedTomatoCount,
                totalTomato = taskInfo.TotalTomatoCount,
                TomatImages = {Unfinishedimg,Finishedimg}
                //问题 怎么根据已完成和未完成番茄数给list<image>赋值

            });

            AllTasksListView.ItemsSource = items;
            CollectionView view = (CollectionView) CollectionViewSource.GetDefaultView(AllTasksListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("State");
            view.GroupDescriptions.Add(groupDescription);
        }
        /// <summary>
        /// 重写List构造函数
        /// </summary>
        /// <returns></returns>
        public List<Image> TomatoImages()
        {
            int unfinished = taskInfo.TotalTomatoCount - taskInfo.FinishedTomatoCount;
            return null;//TODO no return

        }

        private void AddNewTask_OnClick(object sender, RoutedEventArgs e)
        {
            NewTaskWindow newTaskWindow = new NewTaskWindow();
            newTaskWindow.Show();

            taskInfo.TaskID = AllTasksListView.Items.Count + 1;
            taskInfo.Name = newTaskWindow.TxtBoxTaskName.Text;
            taskInfo.StartTime = DateTime.Parse(newTaskWindow.StartTimeSelect.Value.ToString());
            taskInfo.EndTime = DateTime.Parse(newTaskWindow.EndTimeSelect.Value.ToString());
            taskInfo.TotalTomatoCount = newTaskWindow.ListViewTomato.Items.Count;
            taskInfo.Notes = newTaskWindow.TextBoxNotes.Text;

        }

        private void DeleteTask(object sender, RoutedEventArgs e)
        {
            taskInfo.TaskID = AllTasksListView.SelectedIndex;
            MessageBox.Show("确认删除任务:"+ AllTasksListView.SelectedIndex.ToString()+taskInfo.Name, "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            tts.DeleteTask(taskInfo.TaskID);
        }

        private void ModifyTasks(object sender, RoutedEventArgs e)
        {
            NewTaskWindow newTaskWindow = new NewTaskWindow();
            newTaskWindow.Show();

            taskInfo.TaskID = AllTasksListView.SelectedIndex;
            taskInfo.Name = newTaskWindow.TxtBoxTaskName.Text;
            taskInfo.StartTime=DateTime.Parse(newTaskWindow.StartTimeSelect.Value.ToString());
            taskInfo.EndTime = DateTime.Parse(newTaskWindow.EndTimeSelect.Value.ToString());
            taskInfo.TotalTomatoCount = newTaskWindow.ListViewTomato.Items.Count;
            taskInfo.Notes = newTaskWindow.TextBoxNotes.Text;

            tts.ModifyTask(taskInfo);
        }

      
    }

 

    public class TaskItem
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public String StartTime { get; set; }
        public String DeadLine { get; set; }
        public int finishedTomato { get; set; }
        public int totalTomato { get; set; }
        public bool State { get; set; }

        public List<Image> TomatImages { get; set; }
    }

   
}
