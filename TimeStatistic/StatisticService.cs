using StatisticService.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace StatisticService
{
    public class StatisticService
    {
        private static List<Task> killedTasks = new List<Task>();
        private static List<TaskPiece> taskPieces = new List<TaskPiece>();
        private static Dictionary<string, string> typeDict = new Dictionary<string, string>();
        private static int timeslice;                   // Monitor的检查周期，单位：毫秒
        private static bool monitorStarted = false;     // 是否已经开启了一个监控线程标志

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public static void StartMonitor()
        {
            if (!monitorStarted)
            {
                monitorStarted = true;
                Import();
                timeslice = 200;                  // 暂时直接写死，应该由配置文件设置
                Thread thread = new Thread(new ThreadStart(StatisticService.Monitor));
                thread.Start();
            }
        }

        public static List<Task> GetTasksWithin(DateTime t1, DateTime t2)
        {
            List<TaskPiece> pieces = taskPieces.FindAll(tp => tp.StartTime >= t1 && tp.CloseTime <= t2);
            return TransferTaskPiece2Task(pieces);
        }

        public static List<Task> GetAllTasks()
        {
            List<Task> tasks;

            lock(taskPieces)            // 防止读的时候数据发生变化
            {
                tasks = TransferTaskPiece2Task(taskPieces);
            }

            return tasks;
        }

        public static List<Task> GetKilledTasks()
        {
            return killedTasks;
        }

        public static void ChangeTaskType(string taskName, string typeName)
        {
            typeDict[taskName] = typeName;
        }

        public static void Export()
        {

        }

        public static void Import()
        {
            return;
        }

        private static void Monitor()
        {
            Process cproc = Process.GetCurrentProcess();

            while (true)
            {
                try
                {
                    IntPtr hWnd = GetForegroundWindow();
                    uint pid = new uint();
                    GetWindowThreadProcessId(hWnd, out pid);
                    Process proc = Process.GetProcessById(Convert.ToInt32(pid));

                    if (proc == cproc)      // TODO: 由于这个函数是由另一个线程单独执行，所以用普通方法无法检查是否为当前进程
                    {
                        Thread.Sleep(timeslice);
                        continue;
                    }

                    TaskPiece currentTP = new TaskPiece
                    {
                        Name = proc.ProcessName,
                        Detail = proc.MainWindowTitle,
                        StartTime = DateTime.Now,
                        CloseTime = DateTime.Now,
                        TaskType = typeDict.ContainsKey(proc.ProcessName) ? typeDict[proc.ProcessName] : ""
                    };

                    // Get Exe Path
                    try
                    {
                        currentTP.Path = proc.MainModule.FileName;
                    }
                    catch (Exception)
                    {
                        currentTP.Path = "";        // 窗口不一定是exe，所以发生异常时这个路径置为空
                    }

                    lock (taskPieces)
                    {
                        if (taskPieces.Count == 0)
                        {
                            taskPieces.Add(currentTP);
                            Thread.Sleep(timeslice);
                            continue;
                        }

                        TaskPiece lastTP = taskPieces[taskPieces.Count - 1];    // 上一个任务片片
                        lastTP.CloseTime = DateTime.Now;        // 先更新上一个任务片的结束时间
                        if (!lastTP.Equals(currentTP))
                        {
                            taskPieces.Add(currentTP);

                            if (Process.GetProcesses().Count(p => p.ProcessName == lastTP.Name) == 0)     // 当前窗口发送了变化时，检测上一个窗口的进程是否被关闭（可能也只是隐藏）。
                            {
                                killedTasks.Add(new Task(lastTP));  // 记录被杀死的进程
                            }
                        }
                    }
                    
                    Thread.Sleep(timeslice);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private static List<Task> TransferTaskPiece2Task(List<TaskPiece> tps)      // 把给定的一堆任务片统计成不重复的任务
        {
            List<Task> tasks = new List<Task>();

            foreach (TaskPiece tp in tps)        // 把时间片累计起来
            {
                Task t = tasks.FirstOrDefault(task => task.IsSameProc(tp));
                if (t == null)
                {
                    t = new Task(tp);
                    tasks.Add(t);
                }
                else
                {
                    t.Add(tp);      // 如果已经创建了这个进程的任务，那么把tp时间片加进去
                }
            }

            return tasks;
        }
    }
}
