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

namespace UI.AllTaskWindow
{
    /// <summary>
    /// AllTasksWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AllTasksWindow : Window
    {
        readonly TaskTomatoService tts = TaskTomatoService.GetTaskTomatoService();

        public List<TaskItem> TaskItems { get; set; }
        public TaskInfo StartTaskInfo { get; set; }     // 用户准备开始做的任务

        public AllTasksWindow(TaskInfo CurrentTaskInfo)
        {
            InitializeComponent();

            TaskItems = new List<TaskItem>();

            AllTasksListView.ItemsSource = TaskItems;
            StartTaskInfo = CurrentTaskInfo;

            UpdateView();
        }

        private void UpdateView(bool showAllTask=true)
        {
            if (showAllTask)
            {
                TaskItems.Clear();
                List<TaskInfo> allTaskInfos = tts.GetAllFinishedTaskInfo();
                allTaskInfos.AddRange(tts.GetAllUnfinishedTaskInfos());
                TaskItems.AddRange(TransferTaskItemsFromTaskInfo(allTaskInfos));
            }
            
            AllTasksListView.Items.Refresh();
        }

        /// <summary>
        /// 添加新任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewTask_OnClick(object sender, RoutedEventArgs e)
        {
            NewTaskWindow newTaskWindow = new NewTaskWindow();
            newTaskWindow.ShowDialog();
            UpdateView();
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteTask(object sender, RoutedEventArgs e)
        {
            TaskItem taskItem = AllTasksListView.SelectedItem as TaskItem;
            if (taskItem == null)
            {
                MessageBox.Show("未选中任务");
                return;
            }
            tts.DeleteTask(taskItem.ID);
            StartTaskInfo = null;
            UpdateView();
        }

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyTasks(object sender, RoutedEventArgs e)
        {
            TaskItem selectedTaskItem = AllTasksListView.SelectedItem as TaskItem;
            if (selectedTaskItem == null)
            {
                MessageBox.Show("没有选中任务");
                return;
            }

            NewTaskWindow newTaskWindow = new NewTaskWindow(TransferTaskInfoFromTaskItem(selectedTaskItem));
            newTaskWindow.ShowDialog();
            UpdateView();
        }

        /// <summary>
        /// 查询任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBtn_OnClick(object sender, RoutedEventArgs e)
        {
            TaskItems.Clear();
            List<TaskInfo> allTaskInfos = tts.GetAllFinishedTaskInfo();
            allTaskInfos.AddRange(tts.GetTaskWithName(SearchTextBox.Text));
            TaskItems.AddRange(TransferTaskItemsFromTaskInfo(allTaskInfos));

            UpdateView(false);
        }

        /// <summary>
        /// 从搜索状态恢复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text == "")
            {
                UpdateView();
            }
            else
                return;
        }

        /// <summary>
        /// 选中任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            TaskItem selectedTaskItem = AllTasksListView.SelectedItem as TaskItem;
            if (selectedTaskItem == null)
            {
                MessageBox.Show("没有选中任务");
                return;
            }

            StartTaskInfo = tts.GetTaskWithID(selectedTaskItem.ID);
            this.Close();
        }

        #region 工具函数

        /// <summary>
        /// TaskInfo->TaskItem
        /// </summary>
        /// <param name="taskInfos"></param>
        /// <returns></returns>
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
                    FinishedTomato = taskInfo.Finished,
                    TotalTomato = taskInfo.TotalTomatoCount,
                    ImageItems = GetImageItems(taskInfo.FinishedTomatoCount, taskInfo.TotalTomatoCount)
                };
                taskItems.Add(taskItem);
            }
            return taskItems;
        }

        /// <summary>
        /// TaskItem->TaskInfo
        /// </summary>
        /// <param name="taskItem"></param>
        /// <returns></returns>
        private TaskInfo TransferTaskInfoFromTaskItem(TaskItem taskItem)
        {
            return TaskTomatoService.GetTaskTomatoService().GetTaskWithID(taskItem.ID);
        }

        /// <summary>
        /// 构造番茄图标
        /// </summary>
        /// <param name="tomatoFinishedCount"></param>
        /// <param name="tomatoTotalCount"></param>
        /// <returns></returns>
        private List<ImageItem> GetImageItems(int tomatoFinishedCount, int tomatoTotalCount)
        {
            List<ImageItem> images = new List<ImageItem>();
            for (int i = 0; i < tomatoTotalCount; i++)
            {
                if (i < tomatoFinishedCount)
                {
                    images.Add(new ImageItem()
                    {
                        ImageSrc = new BitmapImage(new Uri("../Image/Tomato-Finished.png", UriKind.Relative))
                    });
                }
                else
                {
                    images.Add(new ImageItem()
                    {
                        ImageSrc = new BitmapImage(new Uri("../Image/Tomato-Unfinished.png", UriKind.Relative))
                    });
                }
            }
            return images;
        }

        #endregion
    }

}
