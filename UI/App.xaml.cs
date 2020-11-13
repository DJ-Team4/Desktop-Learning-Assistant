using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DesktopLearningAssistant.TimeStatistic;
using DesktopLearningAssistant.Configuration;

namespace UI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private MainWindow mainWindow;

        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 启动主窗口
            mainWindow = new MainWindow();
            mainWindow.Show();

            // 启动屏幕监控
            ActivityMonitor am = ActivityMonitor.GetMonitor();
            am.Start();

            // 读入配置
            ConfigService.LoadFromJson();
        }

        /// <summary>
        /// 程序出口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // 关闭屏幕监控
            ActivityMonitor am = ActivityMonitor.GetMonitor();
            am.Stop();

            // 将屏幕时间统计数据写入DB
            TimeDataManager timeDataManager = TimeDataManager.GetTimeDataManager();
            timeDataManager.SaveDataToDb();

            // 写入配置
            ConfigService.SaveAsJson();
        }
    }
}
