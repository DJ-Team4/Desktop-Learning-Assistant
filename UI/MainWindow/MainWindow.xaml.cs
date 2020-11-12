using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Panuon.UI.Silver;
using LiveCharts;
using LiveCharts.Wpf;
using UI.Process;
using System.ComponentModel;
using System.Threading;
using DesktopLearningAssistant.TimeStatistic;
using DesktopLearningAssistant.TaskTomato;
using DesktopLearningAssistant.TaskTomato.Model;

using TTomato = DesktopLearningAssistant.TaskTomato.Model.Tomato;

namespace UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection SeriesCollection { get; set; }

        private MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

        // 关于番茄时钟倒计时
        private TimeCount timeCount;

        private DispatcherTimer tomatoTimer;

        public MainWindow()
        {
            InitializeComponent();
            
            this.Loaded += new RoutedEventHandler(TomatoClock_OnLoaded); //***加载倒计时

            this.DataContext = mainWindowViewModel;

            // 当数据发生变化时，更新ViewModel数据
            ActivityMonitor am = ActivityMonitor.GetMonitor();
            am.DataUpdateEvent += Am_DataUpdateEvent;

            // 测试TaskTomato服务
            testTmp();
        }

        private void testTmp()
        {
            TaskInfo taskInfo1 = new TaskInfo()
            {
                Name = "重写美偲的接口",
                Notes = "……",
                TotalTomatoCount = 5,
                StartTime = DateTime.Today,
                EndTime = DateTime.Today.AddDays(1),
            };

            TaskInfo taskInfo2 = new TaskInfo()
            {
                Name = "解决频闪问题",
                Notes = "……",
                TotalTomatoCount = 5,
                StartTime = DateTime.Today,
                EndTime = DateTime.Today.AddDays(1),
            };

            TaskTomatoService tts = TaskTomatoService.GetTimeStatisticService();
            tts.AddTask(taskInfo1);
            tts.AddTask(taskInfo2);

            TaskInfo taskInfo3 = tts.GetTaskWithID(1);
            TaskInfo taskInfo4 = tts.GetTaskWithName("解决频闪问题");

            List<TaskInfo> taskInfos = tts.GetAllUnfinishedTaskInfos();

            tts.DeleteTask(taskInfo3.TaskID);
            tts.DeleteTask(taskInfo4.TaskID);

            tts.AddTask(taskInfo1);
            TTomato tomato = new TTomato()
            {
                TaskID = 1,
                BeginTime = DateTime.Today,
                EndTime = DateTime.Now
            };
            tts.FinishedOneTomato(tomato);
            taskInfos = tts.GetAllUnfinishedTaskInfos();
        }

        private void Am_DataUpdateEvent(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(mainWindowViewModel.Update));
        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart) chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries) chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }


        private void TomatoClock_OnLoaded(object sender, RoutedEventArgs e)
        {
            //设置定时器

            tomatoTimer = new DispatcherTimer();
            tomatoTimer.Interval = new TimeSpan(10000000); //时间间隔为一秒
            tomatoTimer.Tick += new EventHandler(Timer_Tick);

            //转换成秒数
            Int32 hour = Convert.ToInt32(HourArea.Text);
            Int32 minute = Convert.ToInt32(MinuteArea.Text);
            Int32 second = Convert.ToInt32(SecondArea.Text);

            //处理倒计时的类
            timeCount = new TimeCount(hour * 3600 + minute * 60 + second);
            CountDown += new CountDownHandler(timeCount.TimeCountDown);
           //  timer.Start();
        }

        /// <summary>
        /// 处理倒计时的委托
        /// </summary>

        public delegate bool CountDownHandler();

        /// <summary>
        /// 处理事件
        /// </summary>
        public event CountDownHandler CountDown;

        public bool OnCountDown()
        {
            if (CountDown != null)
                return CountDown();

            return false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (OnCountDown())
            {
                HourArea.Text = timeCount.GetHour();
                MinuteArea.Text = timeCount.GetMinute();
                SecondArea.Text = timeCount.GetSecond();
            }
            else
                tomatoTimer.Stop();
        }

        private void File_DragEnter(object sender, DragEventArgs e)
        {
            //MessageBox.Show("File Drop Enter");
            Debug.WriteLine("drag in");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else
                e.Effects = DragDropEffects.None;
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            MessageBox.Show("File Drop");
            Debug.WriteLine("drop");
            var tagWindow = new FileWindow.FileWindow();
            tagWindow.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();

        }

        private void TimeCountStart_OnClick(object sender, RoutedEventArgs e)
        {
          tomatoTimer.Start();
          Thread thread = new Thread(new ThreadStart(() =>
          {
              for (int i = 1; i <= 2500; i++)
              {
                  this.TomatoProgressBar.Dispatcher.Invoke(() => this.TomatoProgressBar.Value = i);
                  Thread.Sleep(10000);
              }
          }));
          thread.Start();

            ImageSource pause = new BitmapImage(new Uri("Icon/Pause.jpg", UriKind.Relative));
          this.ButtonImage.Source = pause;
        }

        private void TimeCountPause_Click(object sender, MouseButtonEventArgs e)
        {
            tomatoTimer.Stop();
            ImageSource start = new BitmapImage(new Uri("Icon/Start.jpeg", UriKind.Relative));
            this.ButtonImage.Source = start;

        }

        private void OpenTomatoWindow(object sender, MouseButtonEventArgs e)
        {
            TomatoWindow tomatoWindow =new TomatoWindow();
            tomatoWindow.Show();
        }

        /// <summary>
        /// 点击“文件管理”按钮
        /// </summary>
        private void OpenFileWinButton_Click(object sender, RoutedEventArgs e)
        {
            //打开文件管理窗口
            new FileWindow.FileWindow().Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TimelineWindow timelineWindow = new TimelineWindow();
            timelineWindow.Show();
        }
    }
}


