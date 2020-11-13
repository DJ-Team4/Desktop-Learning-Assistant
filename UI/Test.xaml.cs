using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Tomato
{
    /// <summary>
    /// Test.xaml 的交互逻辑
    /// </summary>
    public partial class Test : Window
    {

        private System.Windows.Threading.DispatcherTimer m_Timer1 = new System.Windows.Threading.DispatcherTimer();

        double m_Percent = 0;
        bool m_IsStart = false;

        public Test()
        {
            InitializeComponent();
            //25分钟走完一个番茄钟
          //  m_Timer1.Interval = new TimeSpan(0, 0, 0, 15, 0);
            m_Timer1.Interval = new TimeSpan(0, 0, 0, 1, 0);

            m_Timer1.Tick += M_Timer1_Tick;
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
                btnStart.Content = "停止";
            else
                btnStart.Content = "开始";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_IsStart)
            {
                m_Timer1.Stop();
                m_IsStart = false;

            }
            else
            {
  //              m_Percent = 0;
                m_Timer1.Start();
                m_IsStart = true;

            }

            StartChange(m_IsStart);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_Timer1.Stop();
        }


        public string mLinePath = @"C:\Users\11145\Desktop\LearnAssistant\Tomato\Tomato\tomatounfinished.png";

        public string MLinePath
        {
            get { return mLinePath; }
            set { mLinePath = value; }
        }


        private void AddTomatoNum_OnClick(object sender, RoutedEventArgs e)
        {
            Image mLine = new Image() {Source = new BitmapImage(new Uri(MLinePath, UriKind.Absolute))};
            mLine.Width = 35;
            mLine.Height = 35;
            ListViewTomato.Items.Add(mLine);
        }
  }
}

