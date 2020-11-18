using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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
        private NotifyIcon notifyIcon = null;

        #region 任务/番茄钟模块

        private TaskTomatoService tts = TaskTomatoService.GetTaskTomatoService();

        private TimeCount timeCount;
        private double m_Percent = 0;
        private bool m_IsStart = false;

        private DispatcherTimer tomatoTimer;

        

        private void UpdateRecentFilesListView()
        {
            
        }

        #endregion

        #region 文件管理模块

        private void File_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            //MessageBox.Show("File Drop Enter");
            Debug.WriteLine("drag in");
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                e.Effects = System.Windows.DragDropEffects.Link;
            else
                e.Effects = System.Windows.DragDropEffects.None;
        }

        private void File_Drop(object sender, System.Windows.DragEventArgs e)
        {
            System.Windows.MessageBox.Show("File Drop");
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
            m_Timer1.Interval = new TimeSpan(0, 0, 0, 15, 0);

            m_Timer1.Tick += M_Timer1_Tick;

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

            TaskTomatoService tts = TaskTomatoService.GetTaskTomatoService();
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


        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart) chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries) chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }
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
        /// <summary>
        /// UI变化
        /// </summary>
        /// <param name="bState"></param>
        private void StartChange(bool bState)
        {
            if (bState)
                btn.Content = "停止";
            else
                btn.Content = "开始";
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
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
                tomatoTimer.Start();
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    for (int i = 1; i <= 2500; i++)
                    {
                        //      this.TomatoProgressBar.Dispatcher.Invoke(() => this.TomatoProgressBar.Value = i);
                        Thread.Sleep(10000);
                    }
                }));
                thread.Start();
            }
            circleProgressBar.CurrentValue1 = 0;
            StartChange(m_IsStart);
        }

        private void TomatoClock_OnLoaded(object sender, RoutedEventArgs e)
        {
            //设置定时器
            tomatoTimer = new DispatcherTimer();
            tomatoTimer.Interval = new TimeSpan(10000000); //时间间隔为一秒
            tomatoTimer.Tick += new EventHandler(Timer_Tick);

            //转换成秒数
            Int32 minute = Convert.ToInt32(MinuteArea.Text);
            Int32 second = Convert.ToInt32(SecondArea.Text);

            //处理倒计时的类
            timeCount = new TimeCount(minute * 60 + second);
            CountDown += new CountDownHandler(timeCount.TimeCountDown);
           // timer.Start();
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
            m_Timer1.Stop();
            ImageSource start = new BitmapImage(new Uri("Image/Start.jpeg", UriKind.Relative));
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

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(e.LeftButton==MouseButtonState.Pressed)
            {
                this.DragMove();
               
            }
            if (this.Left < 10)
                this.Left = 0;
            else if (this.Left > SystemParameters.WorkArea.Width)
                this.Left = SystemParameters.WorkArea.Width;
            if (this.Top < 10)
                this.Top = 0;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }


        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }


        private void OpenAllTasks_OnClick(object sender, RoutedEventArgs e)
        {
            AllTasksWindow allTasksWindow=new AllTasksWindow();
            allTasksWindow.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            notify();
        }
        private void notify()
        {
            //隐藏主窗体
            this.Visibility = Visibility.Hidden;

            //设置托盘的各个属性
            notifyIcon = new NotifyIcon();
            notifyIcon.Text = "桌面学习助手";
            notifyIcon.Icon = new System.Drawing.Icon("Image/Icon.ico");
            notifyIcon.Visible = true;
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
        }

        /// <summary>
        /// 鼠标单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //如果鼠标左键单击
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    this.Activate();
                }
            }
        }

        private void OpenAllTasksWindow(object sender, RoutedEventArgs e)
        {
            AllTasksWindow allTasksWindow = new AllTasksWindow();
            allTasksWindow.Show();
        }

   
    }
}


