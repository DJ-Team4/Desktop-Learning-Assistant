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

namespace UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection SeriesCollection { get; set; }
        MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

        // 关于番茄时钟倒计时
        private TimeCount timeCount;

        private DispatcherTimer timer;
        //

        public MainWindow()
        {
            InitializeComponent();
            

            this.Loaded += new RoutedEventHandler(TomatoClock_OnLoaded); //***加载倒计时

            this.DataContext = mainWindowViewModel;
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

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10000000); //时间间隔为一秒
            timer.Tick += new EventHandler(timer_Tick);
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

        private void timer_Tick(object sender, EventArgs e)
        {
            if (OnCountDown())
            {
                HourArea.Text = timeCount.GetHour();
                MinuteArea.Text = timeCount.GetMinute();
                SecondArea.Text = timeCount.GetSecond();
            }
            else
                timer.Stop();
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
            TagWindow tagWindow=new TagWindow();
            tagWindow.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();

        }

        private void TimeCountStart_OnClick(object sender, RoutedEventArgs e)
        {
           timer.Start();
           ImageSource pause = new BitmapImage(new Uri("Icon/Pause.jpg", UriKind.Relative));
          this.ButtonImage.Source = pause;
        }

        private void TimeCountPause_Click(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            ImageSource start = new BitmapImage(new Uri("Icon/Start.jpeg", UriKind.Relative));
            this.ButtonImage.Source = start;

        }

        private void OpenTomatoWindow(object sender, MouseButtonEventArgs e)
        {
            TomatoWindow tomatoWindow =new TomatoWindow();
            tomatoWindow.Show();
        }
    }
}


