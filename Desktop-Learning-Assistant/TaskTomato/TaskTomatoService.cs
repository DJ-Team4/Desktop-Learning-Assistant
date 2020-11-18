using System;
using System.IO;
using IWshRuntimeLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.EntityFrameworkCore;
using DesktopLearningAssistant.TaskTomato.Model;
using DesktopLearningAssistant.Configuration;
using DesktopLearningAssistant.TimeStatistic.Model;
using DesktopLearningAssistant.TimeStatistic;

namespace DesktopLearningAssistant.TaskTomato
{
    public class TaskTomatoService
    {
        #region 私有变量

        /// <summary>
        /// 单例变量
        /// </summary>
        private static TaskTomatoService uniqueTaskTomatoService;

        /// <summary>
        /// 确保线程同步的锁标识
        /// </summary>
        private static readonly object locker = new object();

        /// <summary>
        /// 用于操作数据库的 DbContext
        /// </summary>
        private TaskTomatoContext Context
        {
            get
            {
                string DbPath = ConfigService.GetConfigService().TTConfig.DbPath;   // 获取数据库路径

                var builder = new DbContextOptionsBuilder<TaskTomatoContext>();
                builder.UseSqlite($"Data Source={DbPath}");
                TaskTomatoContext context = new TaskTomatoContext(builder.Options);
                return context;
            }
        }

        #endregion

        #region 公共函数

        /// <summary>
        /// 获取单例对象的方法
        /// </summary>
        /// <returns></returns>
        public static TaskTomatoService GetTaskTomatoService()
        {
            if (uniqueTaskTomatoService != null) return uniqueTaskTomatoService;

            lock (locker)
            {
                uniqueTaskTomatoService = new TaskTomatoService();
                using (var context = uniqueTaskTomatoService.Context)
                {
                    context.Database.EnsureCreated();
                }
            }
            return uniqueTaskTomatoService;
        }

        /// <summary>
        /// 释放单例对象
        /// </summary>
        public static void Dispose()
        {
            uniqueTaskTomatoService = null;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="taskInfo"></param>
        public void AddTask(TaskInfo taskInfo)
        {
            using (var context = Context)
            {
                context.TaskModels.Add(taskInfo);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="taskID"></param>
        public void DeleteTask(int taskID)
        {
            using (var context = Context)
            {
                var taskInfo = context.TaskModels.Include(t => t.Tomatoes).FirstOrDefault(tt => tt.TaskID == taskID);
                if (taskInfo != null)
                {
                    context.TaskModels.Remove(taskInfo);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="taskInfo"></param>
        public void ModifyTask(TaskInfo taskInfo)
        {
            using (var context = Context)
            {
                context.Entry(taskInfo).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 根据ID查询任务
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public TaskInfo GetTaskWithID(int taskID)
        {
            using (var context = Context)
            {
                TaskInfo taskInfo = context.TaskModels.Include(tm => tm.Tomatoes).Include(tm => tm.RelativeFiles).FirstOrDefault(t => t.TaskID == taskID);
                return taskInfo;
            }
        }

        /// <summary>
        /// 根据Name查询任务
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TaskInfo GetTaskWithName(string name)
        {
            using (var context = Context)
            {
                TaskInfo taskInfo = context.TaskModels.Include(tm => tm.Tomatoes).Include(tm => tm.RelativeFiles).FirstOrDefault(t => t.Name == name);
                return taskInfo;
            }
        }

        /// <summary>
        /// 获取所有未完成的任务
        /// </summary>
        /// <returns></returns>
        public List<TaskInfo> GetAllUnfinishedTaskInfos()
        {
            using (var context = Context)
            {
                var query = context.TaskModels.Include(tm => tm.Tomatoes).Include(tm => tm.RelativeFiles).Where(tm => !tm.Finished).OrderBy(tm => tm.TaskID);
                return query.ToList();
            }
        }

        /// <summary>
        /// 获取最近的一个未完成任务
        /// </summary>
        /// <returns></returns>
        public TaskInfo GetCurrentTaskInfo()
        {
            using(var context = Context)
            {
                var query = context.TaskModels.Include(tm => tm.Tomatoes).Include(tm => tm.RelativeFiles).Where(tm => !tm.Finished).OrderBy(tm => tm.EndTime).FirstOrDefault();
                return query;
            }
        }

        /// <summary>
        /// 获取所有已完成任务
        /// </summary>
        /// <returns></returns>
        public List<TaskInfo> GetAllFinishedTaskInfo()
        {
            using (var context = Context)
            {
                var query = context.TaskModels.Include(tm => tm.Tomatoes).Include(tm => tm.RelativeFiles).Where(tm => tm.Finished).OrderBy(tm => tm.TaskID);
                return query.ToList();
            }
        }

        /// <summary>
        /// 设定任务已完成
        /// </summary>
        /// <param name="taskID"></param>
        private void SetTaskFinished(int taskID)
        {
            using (var context = Context)
            {
                var task = new TaskInfo() { TaskID = taskID, Finished = true };
                context.Entry(task).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 添加一个已完成的番茄钟
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="tomatoID"></param>
        public void FinishedOneTomato(Tomato tomato)
        {
            using (var context = Context)
            {
                var taskInfo = context.TaskModels.FirstOrDefault(t => t.TaskID == tomato.TaskID);
                if (taskInfo == null)
                {
                    throw new Exception("TaskID of tomato does not exist");
                }

                // 将番茄钟加入表中
                context.TomatoesModels.Add(tomato);

                // 把焦点任务加入表中
                foreach (FocusApp focusApp in tomato.FocusApps)
                {
                    context.FocusAppModels.Add(focusApp);
                }

                // 修改任务状态
                taskInfo.FinishedTomatoCount++;

                // 修改相关文件状态
                List<string> relativeFiles = GetFilePath(tomato.BeginTime, tomato.EndTime);
                foreach (string relativeFile in relativeFiles)
                {
                    context.TaskFileModels.Add(new TaskFile() 
                    { 
                        TaskID = taskInfo.TaskID,
                        FilePath = relativeFile
                    });
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// 获取距离time最近的taskCount个任务效率
        /// </summary>
        /// <param name="time"></param>
        /// <param name="taskCount"></param>
        /// <returns></returns>
        public List<TaskEfficiency> GetTaskEfficiencies(DateTime time, int taskCount)
        {
            List<TaskInfo> allTaskInfos = GetAllFinishedTaskInfo();
            allTaskInfos.Sort((t1, t2) => { return t1.EndTime > t2.EndTime ? 1 : 0; });     // 按结束时间由晚到早排序
            List<TaskEfficiency> taskEfficiencies = new List<TaskEfficiency>();
            foreach (TaskInfo taskInfo in allTaskInfos)
            {
                if (taskEfficiencies.Count >= taskCount) break;
                if (taskInfo.EndTime < time)
                {
                    TaskEfficiency taskEfficiency = new TaskEfficiency()
                    {
                        TaskID = taskInfo.TaskID,
                        Name = taskInfo.Name,
                        Efficiency = GetTaskEfficiency(taskInfo)
                    };
                    taskEfficiencies.Add(taskEfficiency);
                }
            }
            return taskEfficiencies;
        }

        #endregion

        #region 私有函数

        /// <summary>
        /// 查找一段时间内打开的文件
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="taskID"></param>
        /// <returns></returns>
        private List<string> GetFilePath(DateTime beginTime, DateTime endTime)
        {
            string appdata = Environment.GetEnvironmentVariable("AppData");
            string RecentFilePath = $"{appdata}\\Microsoft\\Windows\\Recent";
            DirectoryInfo directoryInfo = new DirectoryInfo(RecentFilePath);
            List<FileInfo> fileInfos = new List<FileInfo>(directoryInfo.GetFiles());
            List<string> recentOpenedFiles = new List<string>();
            
            foreach (var fileInfo in fileInfos)
            {
                if (!fileInfo.Exists) continue;
                if (!(fileInfo.CreationTime > beginTime && fileInfo.CreationTime < endTime)) continue;

                // 获取快捷路径指向的位置
                WshShell shell = new WshShell();
                IWshShortcut lnkPath = (IWshShortcut)shell.CreateShortcut(fileInfo.FullName);
                if (lnkPath.TargetPath == "") continue;
                recentOpenedFiles.Add(lnkPath.TargetPath);
            }

            return recentOpenedFiles;
        }

        /// <summary>
        /// 获取一个任务的效率
        /// </summary>
        /// <param name="taskInfo"></param>
        private double GetTaskEfficiency(TaskInfo taskInfo)
        {
            double efficiency = 0;

            foreach (Tomato tomato in taskInfo.Tomatoes)
            {
                efficiency += GetTomatoEfficiency(tomato);
            }

            return efficiency / taskInfo.Tomatoes.Count;
        }

        /// <summary>
        /// 获取一个番茄的效率
        /// </summary>
        /// <param name="tomato"></param>
        /// <returns></returns>
        private double GetTomatoEfficiency(Tomato tomato)
        {
            TimeStatisticService tss = TimeStatisticService.GetTimeStatisticService();
            List<UserActivity> userActivities = tss.GetUserActivitiesWithin(tomato.BeginTime, tomato.EndTime);
            TimeSpan focusTime = new TimeSpan();
            TimeSpan totalTime = tomato.EndTime - tomato.EndTime;

            List<string> focusAppNames = new List<string>();
            foreach (var focusApp in tomato.FocusApps)
            {
                focusAppNames.Add(focusApp.AppName);
            }

            foreach (UserActivity userActivity in userActivities)
            {
                if (focusAppNames.Contains(userActivity.Name))
                {
                    focusTime += userActivity.SpanTime;
                }
            }

            return focusTime.TotalSeconds / totalTime.TotalSeconds;
        }
        
        #endregion
    }
}
