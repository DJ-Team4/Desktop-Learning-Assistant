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
        TaskItem taskItem = new TaskItem();


        public Image TomatoFinishedImage;


        public AllTasksWindow()
        {
            InitializeComponent();
            List<TaskItem> items = new List<TaskItem>();
            /*            AllTasksListView.Items.Add(new TaskItem()
                        {
                            ID = 1, Name = "软件架构实习三", State = true, StartTime = "2020/10/9 12:13:00",
                            DeadLine = "2020/10/15 22:00:00"
                        });*/



            items.Add(new TaskItem()
            {
                ID =taskInfo.TaskID, Name = taskInfo.Name,
                State = taskInfo.Finished,
                StartTime = taskInfo.StartTime.ToString(),
                DeadLine = taskInfo.EndTime.ToString(),
                finishedTomato = taskInfo.FinishedTomatoCount,
                totalTomato = taskInfo.TotalTomatoCount,
           //     TomatoFinishedImagesList=taskItem.TomatoFinishedImagesList.Add(Func<int,List<Image>> fun1(tomato) =>{}),
                TomatoUnfinishedImagesList = {}
            });

            AllTasksListView.ItemsSource = items;
            CollectionView view = (CollectionView) CollectionViewSource.GetDefaultView(AllTasksListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("State");
            view.GroupDescriptions.Add(groupDescription);
        }

        public  List<Image> TomatoFinishedImages(int tomatofinished)
        {
            TaskItem taskItem = new TaskItem();

            for (int i = tomatofinished; i >= 0; i--)
            {
                taskItem.TomatoFinishedImagesList.Add(TomatoFinishedImage);
            }

            return taskItem.TomatoFinishedImagesList;
        }
        public List<Image> TomatoUnFinishedImages(int tomatounfinished)
        {
            TaskItem taskItem = new TaskItem();
            for (int i = tomatounfinished; i >= 0; i--)
            {
                taskItem.TomatoUnfinishedImagesList.Add(TomatoFinishedImage);
            }

            return taskItem.TomatoUnfinishedImagesList;
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
        public List<Image> TomatoFinishedImagesList { get; set; }
        public List<Image> TomatoUnfinishedImagesList { get; set; }
    }

   
}
