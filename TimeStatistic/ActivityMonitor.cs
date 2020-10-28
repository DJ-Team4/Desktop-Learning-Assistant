using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeStatisticTest.TimeStatistic;
using TimeStatisticTest.TimeStatistic.Model;
//using DesktopLearningAssistant.Configuration;
//using DesktopLearningAssistant.Configuration.Config;

namespace TimeStatisticTest.TimeStatistic
{
    public class ActivityMonitor
    {
        private static ActivityMonitor uniqueMonitor;
        private static readonly object locker = new object();   // 定义一个标识确保线程同步

        private TimeDataManager TDManager;
        private bool monitorStarted = false;     // 保证只开启一个线程进行监控
        private int timeSlice;                   // Monitor的检查周期，单位：毫秒

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public ActivityMonitor()
        {
            // var configService = ConfigService.GetConfigService();    // TODO: 从配置类中读出配置
            timeSlice = 200;   // configService.TSConfig.TimeSlice;
            TDManager = TimeDataManager.GetTimeDataManager();       // 注入DataManager
        }

        public static ActivityMonitor GetMonitor()
        {
            if (uniqueMonitor == null)
            {
                lock(locker)
                {
                    uniqueMonitor = new ActivityMonitor();
                }
            }
            return uniqueMonitor;
        }

        public void Start()
        {
            if (!monitorStarted)
            {
                monitorStarted = true;
                Thread thread = new Thread(new ThreadStart(Work));
                thread.Start();
            }
        }

        private void Work()
        {
            while (true)
            {
                Thread.Sleep(timeSlice);    // 定时轮询

                try
                {
                    IntPtr hWnd = GetForegroundWindow();        // 获取前台窗口句柄
                    uint pid = new uint();
                    GetWindowThreadProcessId(hWnd, out pid);
                    Process proc = Process.GetProcessById(Convert.ToInt32(pid));

                    UserActivityPiece currentTP = new UserActivityPiece
                    {
                        Name = proc.ProcessName,
                        Detail = proc.MainWindowTitle,
                        StartTime = DateTime.Now,
                        CloseTime = DateTime.Now,
                    };

                    lock (TDManager)
                    {
                        List<UserActivityPiece> userActivityPieces = TDManager.UserActivityPieces;  // 为了简便书写，作了两个引用
                        if (userActivityPieces.Count == 0)
                        {
                            userActivityPieces.Add(currentTP);
                            continue;
                        }

                        UserActivityPiece lastUAP = userActivityPieces[userActivityPieces.Count - 1];    // 上一个用户活动
                        lastUAP.CloseTime = DateTime.Now;        // 更新上一个用户活动的结束时间
                        if (!lastUAP.Equals(currentTP))
                        {
                            userActivityPieces.Add(currentTP);

                            if ((lastUAP.Name == "explorer" || lastUAP.Name == "Idle") && userActivityPieces.Count >= 3)    // Windows在杀进程前会先转入explorer或Idle，所以出现这种情况时需要再回溯一层UAP
                            {
                                lastUAP = userActivityPieces[userActivityPieces.Count - 3];
                            }
                            if (Process.GetProcesses().Count(p => p.ProcessName == lastUAP.Name) == 0)     // 当前窗口发送了变化时，检测上一个窗口的进程是否被关闭（可能也只是隐藏）。
                            {
                                TDManager.KilledActivity.Add(new UserActivity(lastUAP));  // 记录被杀死的进程
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
