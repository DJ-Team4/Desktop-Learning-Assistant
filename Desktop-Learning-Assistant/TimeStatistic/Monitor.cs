using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DesktopLearningAssistant.TimeStatistic.Model;
using DesktopLearningAssistant.Configuration;
using DesktopLearningAssistant.Configuration.Config;

namespace DesktopLearningAssistant.TimeStatistic
{
    public class Monitor
    {
        private Monitor uniqueMonitor;
        private bool monitorStarted = false;     // 保证只开启一个线程进行监控
        private int timeSlice;                   // Monitor的检查周期，单位：毫秒

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public Monitor()
        {
            var configService = ConfigService.GetConfigService();
            timeSlice = configService.TSConfig.TimeSlice;
        }

        public Monitor GetMonitor()
        {
            if (uniqueMonitor == null)
            {
                lock(uniqueMonitor)
                {
                    uniqueMonitor = new Monitor();
                }
            }
            return uniqueMonitor;
        }

        public static void Start()
        {
            if (!monitorStarted)
            {
                monitorStarted = true;
                timeSlice = 200;                  // 暂时直接写死，应该由配置文件设置
                Thread thread = new Thread(new ThreadStart(Monitor.Work));
                thread.Start();
            }
        }

        private static void Work()
        {
            while (true)
            {
                try
                {
                    IntPtr hWnd = GetForegroundWindow();
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

                    lock (taskPieces)
                    {
                        if (taskPieces.Count == 0)
                        {
                            taskPieces.Add(currentTP);
                            Thread.Sleep(timeSlice);
                            continue;
                        }

                        UserActivityPiece lastTP = taskPieces[taskPieces.Count - 1];    // 上一个任务片片
                        lastTP.CloseTime = DateTime.Now;        // 先更新上一个任务片的结束时间
                        if (!lastTP.Equals(currentTP))
                        {
                            taskPieces.Add(currentTP);

                            if (Process.GetProcesses().Count(p => p.ProcessName == lastTP.Name) == 0)     // 当前窗口发送了变化时，检测上一个窗口的进程是否被关闭（可能也只是隐藏）。
                            {
                                killedTasks.Add(new Model.UserActivity(lastTP));  // 记录被杀死的进程
                            }
                        }
                    }

                    Thread.Sleep(timeSlice);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
