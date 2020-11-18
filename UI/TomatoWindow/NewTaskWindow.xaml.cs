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
using DesktopLearningAssistant.TaskTomato;
using DesktopLearningAssistant.TaskTomato.Model;

namespace UI
{
    /// <summary>
    /// NewTaskWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewTaskWindow : Window
    {
        [DllImport("user32.dll")] public static extern int MessageBoxTimeoutA(IntPtr hWnd, string msg, string Caps, int type, int Id, int time); //引用DLL

        public TaskInfo NewTaskInfo { get; set; }
        public bool IsModify { get; set; }

        public string tomatoFinishedImagePath = @"Image\Tomato-Finished.png";

        public string MLinePath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + tomatoFinishedImagePath; }
            set { tomatoFinishedImagePath = value; }
        }

        public NewTaskWindow()
        {
            InitializeComponent();

            NewTaskInfo = new TaskInfo();
            IsModify = false;

            StartTimeSelect.Value = DateTime.Now;
            EndTimeSelect.Value = DateTime.Now + TimeSpan.FromDays(1);
        }

        /// <summary>
        /// 当传入一个TaskInfo进行构造时，表示是修改此TaskInfo
        /// </summary>
        /// <param name="taskInfo"></param>
        public NewTaskWindow(TaskInfo taskInfo)
        {
            InitializeComponent();

            NewTaskInfo = taskInfo;
            IsModify = true;

            TxtBoxTaskName.Text = taskInfo.Name;
            StartTimeSelect.Value = taskInfo.StartTime;
            EndTimeSelect.Value = taskInfo.EndTime;
            for (int i = 0; i < taskInfo.TotalTomatoCount; i++)
            {
                AddTomatoNum_OnClick(this, null);
            }
            TxtBoxNotes.Text = taskInfo.Notes;
        }

        private void Affirm_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxTimeoutA((IntPtr)0, "保存成功", "提示", 0, 0, 1000); // 直接调用 1秒
            TaskTomatoService tts = TaskTomatoService.GetTaskTomatoService();

            NewTaskInfo.Name = TxtBoxTaskName.Text;
            NewTaskInfo.StartTime = DateTime.Parse(StartTimeSelect.Value.ToString());
            NewTaskInfo.EndTime = DateTime.Parse(EndTimeSelect.Value.ToString());
            NewTaskInfo.TotalTomatoCount = TomatoListStackPanel.Children.Count;
            NewTaskInfo.Notes = TxtBoxNotes.Text;

            if (!IsModify)
            {
                tts.AddTask(NewTaskInfo);
            }
            else
            {
                tts.ModifyTask(NewTaskInfo);
            }
            
            this.Close();
        }

        private void AddTomatoNum_OnClick(object sender, RoutedEventArgs e)
        {
            Image image = new Image() { Source = new BitmapImage(new Uri(MLinePath, UriKind.Absolute)) };
            image.Width = 35;
            image.Height = 35;
            TomatoListStackPanel.Children.Add(image);
        }

        private void DeleteTomatoNum_OnClick(object sender, RoutedEventArgs e)
        {
            if (TomatoListStackPanel.Children.Count == 0) return;
            TomatoListStackPanel.Children.RemoveAt(TomatoListStackPanel.Children.Count - 1);
        }
    
    }
}
