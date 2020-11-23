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
using System.Windows.Documents;

namespace DesktopLearningAssistant.TimeStatistic
{
    /// <summary>
    /// 活动监控类，监控活动过程和被关闭的软件
    /// </summary>
    public class ActivityMonitor
    {
        #region 静态变量

        /// <summary>
        /// 单例监视器
        /// </summary>
        private static ActivityMonitor uniqueMonitor;

        /// <summary>
        /// 确保线程同步的锁标识
        /// </summary>
        private static readonly object locker = new object();
        #endregion

        #region 私有变量

        /// <summary>
        /// 活动数据管理对象，通过TDManager增删改查活动片
        /// </summary>
        private TimeDataManager TDManager;
        private ConfigService configService;

        /// <summary>
        /// 监控线程是否开启的标识，确保只开启一个线程进行监控
        /// </summary>
        private bool monitorStarted = false;

        /// <summary>
        /// 是否要求关闭监控线程
        /// </summary>
        private bool closeFlag = false;

        /// <summary>
        /// 监控线程任务
        /// </summary>
        private Task monitorTask;

        /// <summary>
        /// Monitor的检查周期，单位：毫秒
        /// </summary>
        private int timeSlice;

        #endregion

        #region 公有变量

        /// <summary>
        /// 数据更新委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void DataUpdateEventHandler(object sender, EventArgs e);

        /// <summary>
        /// 数据更新事件
        /// </summary>
        public event DataUpdateEventHandler DataUpdateEvent;

        #endregion

        #region DLL

        /// <summary>
        /// 获取前台窗口句柄的方法
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 获取窗口句柄对应进程ID的方法
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        #endregion

        #region 公有方法

        /// <summary>
        /// 构造方法
        /// </summary>
        public ActivityMonitor()
        {
            configService = ConfigService.GetConfigService();
            timeSlice = configService.TSConfig.TimeSlice;
            TDManager = TimeDataManager.GetTimeDataManager();       // 注入DataManager
        }
        
        /// <summary>
        /// 获取单例对象的方法
        /// </summary>
        /// <returns></returns>
        public static ActivityMonitor GetMonitor()
        {
            if (uniqueMonitor != null) return uniqueMonitor;

            lock (locker)
            {
                uniqueMonitor = new ActivityMonitor();
            }
            return uniqueMonitor;
        }

        /// <summary>
        /// 释放单例对象
        /// </summary>
        public static void Dispose()
        {
            uniqueMonitor = null;
        }

        /// <summary>
        /// 开启监控的方法
        /// </summary>
        public void Start()
        {
            if (monitorStarted) return;

            monitorStarted = true;
            monitorTask = new Task(Work);
            monitorTask.Start();
        }

        public void Stop()
        {
            closeFlag = true;   // 要求停止监控
            monitorTask.Wait(); // 等待线程关闭
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 监控函数
        /// </summary>
        private void Work()
        {
            while (!closeFlag)
            {
                Thread.Sleep(timeSlice);    // 定时轮询

                try
                {
                    IntPtr hWnd = GetForegroundWindow();        // 获取前台窗口句柄
                    uint pid = new uint();
                    GetWindowThreadProcessId(hWnd, out pid);
                    Process proc = Process.GetProcessById(Convert.ToInt32(pid));

                    // 如果是新发现的进程，则加入类型字典中
                    if (!configService.TSConfig.TypeDict.ContainsKey(proc.ProcessName))
                    {
                        configService.TSConfig.TypeDict.Add(proc.ProcessName, "其他");
                    }

                    UserActivityPiece currentUAP = new UserActivityPiece
                    {
                        Name = proc.ProcessName,
                        Detail = proc.MainWindowTitle,
                        StartTime = DateTime.Now,
                        CloseTime = DateTime.Now,
                        Finished = false
                    };

                    lock (TDManager)
                    {
                        List<UserActivityPiece> userActivityPieces = TDManager.UserActivityPieces;  // 为了简便书写，作了引用
                        if (userActivityPieces.Count == 0)
                        {
                            userActivityPieces.Add(currentUAP);
                            continue;
                        }

                        UserActivityPiece lastUAP = userActivityPieces[userActivityPieces.Count - 1];    // 上一个用户活动
                        lastUAP.CloseTime = DateTime.Now;        // 更新上一个用户活动的结束时间

                        if (!lastUAP.Equals(currentUAP) || lastUAP.Finished)
                        {
                            // 更新UAP数组
                            lastUAP.Finished = true;
                            Console.WriteLine("————————————————————");
                            foreach (var uap in userActivityPieces)
                            {
                                Console.WriteLine(uap.ToString());
                            }
                            Console.WriteLine(currentUAP.ToString());
                            userActivityPieces.Add(currentUAP);

                            // 更新KA数组
                            /*
                            if ((lastUAP.Name == "explorer" || lastUAP.Name == "Idle") && userActivityPieces.Count >= 3)    // Windows在杀进程前会先转入explorer或Idle，所以出现这种情况时需要再回溯一层UAP
                            {
                                lastUAP = userActivityPieces[userActivityPieces.Count - 3];
                            }
                            
                            if (Process.GetProcesses().Count(p => p.ProcessName == lastUAP.Name) == 0)     // 当前窗口发送了变化时，检测上一个窗口的进程是否被关闭（可能也只是隐藏）。
                            {
                                TDManager.KilledActivities.Add(new UserActivity(lastUAP));  // 记录被杀死的进程
                            }
                            */
                        }
                    }
                    uniqueMonitor.DataUpdateEvent?.Invoke(this, new EventArgs());
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        #endregion
    }
}
