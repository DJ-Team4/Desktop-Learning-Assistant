﻿using System;
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
using UI.Tomato;
using TTomato = DesktopLearningAssistant.TaskTomato.Model.Tomato;

namespace UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 屏幕时间统计模块

        private MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();
        private int updateSlice = 5;       // 更新屏幕时间统计数据的时间间隔（秒）
        private DispatcherTimer timeDataUpdateTimer = new DispatcherTimer();

        private void TimeDataUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(mainWindowViewModel.Update));
        }

        #endregion

        #region 任务/番茄钟模块

        private TaskTomatoService tts = TaskTomatoService.GetTimeStatisticService();

        private TimeCount timeCount;
        private double m_Percent = 0;
        private bool m_IsStart = false;

        private DispatcherTimer tomatoTimer;

        private void UpdateRecentFilesListView()
        {
            
        }

        #endregion

        #region 文件管理模块

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

        #endregion

        private DispatcherTimer m_Timer1 = new DispatcherTimer();

        System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();

        public MainWindow()
        {
            InitializeComponent();
            
            this.Loaded += new RoutedEventHandler(TomatoClock_OnLoaded); //***加载倒计时
            //25分钟走完一个番茄钟
            //  m_Timer1.Interval = new TimeSpan(0, 0, 0, 15, 0);
            m_Timer1.Interval = new TimeSpan(0, 0, 0, 1, 0);

            m_Timer1.Tick += M_Timer1_Tick;

            //_timer.Interval = 300;
            //_timer.Tick += TimerDealy;
            //_timer.Start();

            this.DataContext = mainWindowViewModel;

            // 定时更新ViewModel数据
            timeDataUpdateTimer.Interval = new TimeSpan(0, 0, 0, updateSlice);
            timeDataUpdateTimer.Tick += TimeDataUpdateTimer_Tick;
            timeDataUpdateTimer.Start();
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
            tts.GetTaskEfficiencies(DateTime.Now, 5);

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


        //进度条处理
        private void M_Timer1_Tick(object sender, EventArgs e)
        {
            m_Percent += 0.01;
            if (m_Percent > 1)
            {
                m_Percent = 1;
                m_Timer1.Stop();
                m_IsStart = false;
                StartChange(m_IsStart);
            }

            circleProgressBar.CurrentValue1 = m_Percent;
        }
   
        private void StartChange(bool bState)
        {
            if (bState)
                btn.Content = "停止";
            else
                btn.Content = "开始";
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            //倒计时

            tomatoTimer.Start();

            //进度条
            if (m_IsStart)
            {
                m_Timer1.Stop();
                m_IsStart = false;
            }
            else
            {
                // m_Percent = 0;
                m_Timer1.Start();
                m_IsStart = true;
                Thread thread2 = new Thread(new ThreadStart(() =>
                {
                    for (int i = 1; i <= 2500; i++)
                    {
                        Thread.Sleep(10000);
                    }
                }));
                thread2.Start();

                //增加番茄
                TTomato tomato = new TTomato()
                {
                    TaskID = mainWindowViewModel.CurrentTaskId,
                    BeginTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(25)
                };

                if(m_Percent==100)
                tts.FinishedOneTomato(tomato);//25分钟后结束番茄
            }
            StartChange(m_IsStart);
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

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();

        }

        private void TimeCountPause_Click(object sender, MouseButtonEventArgs e)
        {
            tomatoTimer.Stop();
            ImageSource start = new BitmapImage(new Uri("Icon/Start.jpeg", UriKind.Relative));
        }


        /// <summary>
        /// 点击“文件管理”按钮
        /// </summary>
        private void OpenFileWinButton_Click(object sender, RoutedEventArgs e)
        {
            //打开文件管理窗口
            new FileWindow.FileWindow().Show();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OpenAllTasksWindow(object sender, RoutedEventArgs e)
        {
            AllTasksWindow allTasksWindow = new AllTasksWindow();
            allTasksWindow.Show();
        }
    }
}


