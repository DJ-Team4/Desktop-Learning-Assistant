using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using DesktopLearningAssistant.TaskTomato;
using DesktopLearningAssistant.TaskTomato.Model;
using DesktopLearningAssistant.Configuration;
using UI.AllTaskWindow;
using UI.FileWindow;
using DesktopLearningAssistant.TagFile;
using System.IO;
using Panuon.UI.Silver;

namespace UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {
        #region 屏幕时间统计模块

        private int updateSlice = 15;       // 更新屏幕时间统计数据的时间间隔（秒）
        private DispatcherTimer timeDataUpdateTimer = new DispatcherTimer();

        private void TodayPieChart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }

        private void TimeDataUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(viewModel.Update));
        }

        #endregion

        #region 任务/番茄钟模块

        private TomatoClockManager clockManager;
        private ClockState nextState;       // 下一个预备的状态
        private TaskTomatoService tts;

        private TaskInfo CurrentTaskInfo    // 间接使用viewModel中的currentTasskInfo
        {
            get
            {
                return viewModel.currentTaskInfo;
            }
            set
            {
                viewModel.currentTaskInfo = value;
            }
        }

        /// <summary>
        /// 在构造函数中被调用，把番茄钟部分的构造代码放入此函数
        /// </summary>
        private void InitTaskClockModule()
        {
            clockManager = new TomatoClockManager(configService.TTConfig.WorkTimeSpan, configService.TTConfig.RelaxTimeSpan);
            clockManager.WorkClockFinishedEvent += WorkClockFinishedHandler;
            clockManager.RelaxClockFinishedEvent += RelaxClockFinishedHandler;
            clockManager.ClockTickEvent += ClockTickEventHandler;
            nextState = ClockState.Working;
            tts = TaskTomatoService.GetTaskTomatoService();

            UpdateCurrentTaskInfo();
            viewModel.UpdateRelativeFiles();
            RelativeFilesListView.Items.Refresh();
        }

        private void ClockBtn_Click(object sender, RoutedEventArgs e)
        {
            // 在番茄钟未完成时结束
            if (nextState == ClockState.Stop)
            {
                if (clockManager.clockState == ClockState.Working)
                {
                    nextState = ClockState.Working;
                    MinuteTextBlock.Text = configService.TTConfig.WorkTimeSpan.Minutes.ToString("D2");
                    SecondTextClock.Text = configService.TTConfig.WorkTimeSpan.Seconds.ToString("D2");
                }
                
                else if (clockManager.clockState == ClockState.Relaxing)
                {
                    nextState = ClockState.Working;
                    MinuteTextBlock.Text = configService.TTConfig.RelaxTimeSpan.Minutes.ToString("D2");
                    SecondTextClock.Text = configService.TTConfig.RelaxTimeSpan.Seconds.ToString("D2");
                }

                ProgressBar.CurrentValue = 1;
                ClockBtnImage.Source = new BitmapImage(new Uri("../Image/Start.png", UriKind.Relative));

                clockManager.AbortClock();
                return;
            }

            if (nextState == ClockState.Working)
            {
                clockManager.StartWorkClock();
            }
            else if (nextState == ClockState.Relaxing)
            {
                clockManager.StartRelaxClock();
            }

            nextState = ClockState.Stop;
            ClockBtnImage.Source = new BitmapImage(new Uri("../Image/Pause.png", UriKind.Relative));
        }

        /// <summary>
        /// 打开所有任务界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenAllTasksWindow(object sender, RoutedEventArgs e)
        {
            AllTasksWindow allTasksWindow = new AllTasksWindow(CurrentTaskInfo);
            allTasksWindow.ShowDialog();
            if (allTasksWindow.StartTaskInfo == null)
            {
                UpdateCurrentTaskInfo();
            }
            else
            {
                UpdateCurrentTaskInfo(allTasksWindow.StartTaskInfo);
            }
        }

        /// <summary>
        /// 双击打开相关文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RelativeFilesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int selectedIndex = RelativeFilesListView.SelectedIndex;
            if (selectedIndex == -1) return;
            string filePath = viewModel.RelativeFileItems[selectedIndex].FilePath;

            if (!File.Exists(filePath)) return;

            FileUtils.OpenFile(filePath);   // 用默认程序打开文件
        }

        /// <summary>
        /// clockTimer完成一个工作钟时回调
        /// </summary>
        private void WorkClockFinishedHandler(object sender)
        {
            if (CurrentTaskInfo == null) return;

            this.Dispatcher.Invoke(new Action(() =>
            {
                string whiteListKey = WhiteListKeyComboBox.Text;
                configService = ConfigService.GetConfigService();

                if (whiteListKey == "" || whiteListKey == null)
                {
                    whiteListKey = configService.TTConfig.WhiteLists.Keys.ToList()[0];
                }

                Tomato tomato = new Tomato()
                {
                    TaskID = CurrentTaskInfo.TaskID,
                    BeginTime = DateTime.Now - configService.TTConfig.WorkTimeSpan,
                    EndTime = DateTime.Now,
                    FocusApps = FocusApp.TransFrom(configService.TTConfig.WhiteLists[whiteListKey])
                };

                tts.FinishedOneTomato(tomato);
                nextState = ClockState.Relaxing;

                // UI恢复
                ClockBtnImage.Source = new BitmapImage(new Uri("../Image/Start.png", UriKind.Relative));
                viewModel.UpdateRelativeFiles();
                RelativeFilesListView.Items.Refresh();  // 强制刷新相关文件视图

                // 检查任务是否完成，若完成则换下一个任务
                CurrentTaskInfo = tts.GetTaskWithID(CurrentTaskInfo.TaskID);
                if (CurrentTaskInfo.FinishedTomatoCount >= CurrentTaskInfo.TotalTomatoCount)
                {
                    tts.SetTaskFinished(CurrentTaskInfo);
                    UpdateCurrentTaskInfo();
                }
            }));
        }

        /// <summary>
        /// clockTimer完成一个休息钟时回调
        /// </summary>
        private void RelaxClockFinishedHandler(object sender)
        {
            if (CurrentTaskInfo == null) return;
            nextState = ClockState.Working;
            // UI恢复

            this.Dispatcher.Invoke(new Action(() =>
            {
                ClockBtnImage.Source = new BitmapImage(new Uri("../Image/Start.png", UriKind.Relative));
            }));
        }
        
        /// <summary>
        /// 每间隔一秒clockManager就会调用此方法一次，并返回累计时间间隔
        /// </summary>
        /// <param name="timeSpan"></param>
        private void ClockTickEventHandler(object sender, TimeSpan timeSpan)
        {
            this.Dispatcher.Invoke(new Action(() => {
                this.MinuteTextBlock.Text = timeSpan.Minutes.ToString("D2");
                this.SecondTextClock.Text = timeSpan.Seconds.ToString("D2");
                this.ProgressBar.CurrentValue = clockManager.Percentage;
            }));
        }

        private void UpdateCurrentTaskInfo(TaskInfo taskInfo = null)
        {
            if (taskInfo == null) CurrentTaskInfo = tts.GetCurrentTaskInfo();     // 获取一个新的未完成任务
            else CurrentTaskInfo = taskInfo;

            if (CurrentTaskInfo != null)
            {
                CurrentTaskInfoLabel.Text = CurrentTaskInfo.Name;
                viewModel.UpdateRelativeFiles();
                RelativeFilesListView.Items.Refresh();
            }
            else
            {
                CurrentTaskInfoLabel.Text = "没有未完成的任务(*￣▽￣*)";
                viewModel.RelativeFileItems.Clear();
                RelativeFilesListView.Items.Refresh();
            }
        }

        private void TomatoTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO：效率统计表加载
        }

        #endregion

        #region 文件管理模块

        /// <summary>
        /// 点击“文件管理”按钮
        /// </summary>
        private void OpenFileWinButton_Click(object sender, RoutedEventArgs e)
        {
            //打开文件管理窗口
            new FileWindow.FileWindow().Show();
        }

        /// <summary>
        /// 文件拖入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void File_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            // TODO：做成边框线变动效果
        }

        /// <summary>
        /// 文件拖入且松开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void File_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (files.Length < 1) return;
                AddFileDialog addFileDialog = new AddFileDialog(files[0]);
                addFileDialog.ShowDialog();
            }
        }

        #endregion

        #region 设置、隐藏与关闭

        private NotifyIcon notifyIcon = null;

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();

            // 番茄钟内的白名单列表需要更新
            viewModel.UpdateWhiteKeys();
            WhiteListKeyComboBox.ItemsSource = viewModel.WhiteListKeys;
            WhiteListKeyComboBox.Items.Refresh();
        }

        private void HideMenuItem_Click(object sender, RoutedEventArgs e)
        {
            notify();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        #endregion

        #region 磁吸贴边

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
            if (Left < 10)
                Left = 0;
            else if (Left > SystemParameters.WorkArea.Width)
                Left = SystemParameters.WorkArea.Width;
            if (Top < 10)
                Top = 0;
        }

        #endregion

        private ConfigService configService;
        private MainWindowViewModel viewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();

            configService = ConfigService.GetConfigService();
            DataContext = viewModel;

            // 定时更新ViewModel数据
            timeDataUpdateTimer.Interval = new TimeSpan(0, 0, 0, updateSlice);
            timeDataUpdateTimer.Tick += TimeDataUpdateTimer_Tick;
            timeDataUpdateTimer.Start();

            // 番茄钟部分初始化
            InitTaskClockModule();
        }
    }
}
