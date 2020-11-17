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

    public class TaskItem
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public String StartTime { get; set; }
        public String DeadLine { get; set; }
        public bool finishedTomato { get; set; }
        public int totalTomato { get; set; }
        public List<Image> TomatoImageList { get; set; }
    }

    public partial class AllTasksWindow : Window
    {
        TaskTomatoService tts = TaskTomatoService.GetTaskTomatoService();
        TaskInfo taskInfo = new TaskInfo();
        TaskItem taskItem = new TaskItem();

        public Image TomatoFinishedImage;

        public List<TaskItem> TaskItems { get; set; }

        public AllTasksWindow()
        {
            InitializeComponent();

            TaskItems = new List<TaskItem>();

            this.AllTasksListView.ItemsSource = TaskItems;

            UpdateViewModel();
        }

        private void UpdateViewModel()
        {
            TaskItems.Clear();
            TaskTomatoService tts = TaskTomatoService.GetTaskTomatoService();
            List<TaskInfo> allTaskInfos = tts.GetAllFinishedTaskInfo();
            allTaskInfos.AddRange(tts.GetAllUnfinishedTaskInfos());
            TaskItems.AddRange(TransferTaskItemsFromTaskInfo(allTaskInfos));
        }

        private List<TaskItem> TransferTaskItemsFromTaskInfo(List<TaskInfo> taskInfos)
        {
            List<TaskItem> taskItems = new List<TaskItem>();
            foreach (TaskInfo taskInfo in taskInfos)
            {
                TaskItem taskItem = new TaskItem()
                {
                    ID = taskInfo.TaskID,
                    Name = taskInfo.Name,
                    StartTime = taskInfo.StartTime.ToString(),
                    DeadLine = taskInfo.EndTime.ToString(),
                    finishedTomato = taskInfo.Finished,
                    totalTomato = taskInfo.TotalTomatoCount,
                    TomatoImageList = GetTomatoImages(taskInfo.FinishedTomatoCount, taskInfo.TotalTomatoCount)
                };
                taskItems.Add(taskItem);
            }
            return taskItems;
        }

        private TaskInfo TransferTaskInfoFromTaskItem(TaskItem taskItem)
        {
            return TaskTomatoService.GetTaskTomatoService().GetTaskWithID(taskItem.ID);
        }

        private List<Image> GetTomatoImages(int tomatoFinishedCount, int tomatoTotalCount)
        {
            List<Image> images = new List<Image>();

            for (int i = 0; i < tomatoTotalCount; i++)
            {
                if (i < tomatoFinishedCount)
                {
                    images.Add(new Image() { Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Image\Tomato-Finished.png", UriKind.Absolute)) });
                }
                else
                {
                    images.Add(new Image() { Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Image\Tomato-Unfinished.png", UriKind.Absolute)) });
                }
            }

            return images;
        }

        private void AddNewTask_OnClick(object sender, RoutedEventArgs e)
        {
            NewTaskWindow newTaskWindow = new NewTaskWindow();
            newTaskWindow.Show();
            UpdateViewModel();
        }

        private void DeleteTask(object sender, RoutedEventArgs e)
        {
            TaskItem taskItem = AllTasksListView.SelectedItem as TaskItem;
            if (taskItem == null)
            {
                MessageBox.Show("未选中任务");
                return;
            }
            tts.DeleteTask(taskItem.ID);
            UpdateViewModel();
        }

        private void ModifyTasks(object sender, RoutedEventArgs e)
        {
            TaskItem selectedTaskItem = AllTasksListView.SelectedItem as TaskItem;
            if (selectedTaskItem == null)
            {
                MessageBox.Show("没有选中任务");
                return;
            }

            NewTaskWindow newTaskWindow = new NewTaskWindow();
            newTaskWindow.taskInfo = TransferTaskInfoFromTaskItem(selectedTaskItem);
            newTaskWindow.isModify = true;
            newTaskWindow.FillData();
            newTaskWindow.Show();
            UpdateViewModel();
        }

        private void SearchBtn_OnClick(object sender, RoutedEventArgs e)
        {
                taskInfo=tts.GetTaskWithName(SearchTextBox.Text);
                AllTasksListView.Items.Clear();
                AllTasksListView.Items.Add(taskInfo);
        }

        private void SearchTextBox_OnTextChanged(object sender, TextChangedEventArgs e) //搜索之后回退
        {
            if (SearchTextBox.Text == "")
            {
                UpdateViewModel();
            }
            else
                return;
        }
    }
    
}
