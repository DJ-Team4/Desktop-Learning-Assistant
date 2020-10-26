using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TimeStatistic.Model;

namespace DesktopLearningAssistant.TimeStatistic
{
    public class TimeStatisticService
    {
        private static TimeStatisticService uniqueTimeStatisticService;

        private Dictionary<string, string> typeDict = new Dictionary<string, string>();

        public static List<UserActivity> GetTasksWithin(DateTime t1, DateTime t2)
        {
            List<UserActivityPiece> pieces = taskPieces.FindAll(tp => tp.StartTime >= t1 && tp.CloseTime <= t2);
            return TransferTaskPiece2Task(pieces);
        }

        public static List<UserActivity> GetAllTasks()
        {
            List<UserActivity> tasks;

            lock (taskPieces)            // 防止读的时候数据发生变化
            {
                tasks = TransferTaskPiece2Task(taskPieces);
            }

            return tasks;
        }

        public static List<UserActivity> GetKilledTasks()
        {
            return killedTasks;
        }

        public static void ChangeTaskType(string taskName, string typeName)
        {
            typeDict[taskName] = typeName;
        }



        private static List<UserActivity> TransferTaskPiece2Task(List<UserActivityPiece> tps)      // 把给定的一堆任务片统计成不重复的任务
        {
            List<UserActivity> tasks = new List<UserActivity>();

            foreach (UserActivityPiece tp in tps)        // 把时间片累计起来
            {
                UserActivity t = tasks.FirstOrDefault(task => task.IsSameActivity(tp));
                if (t == null)
                {
                    t = new UserActivity(tp);
                    tasks.Add(t);
                }
                else
                {
                    t.AddUserActivityPiece(tp);      // 如果已经创建了这个进程的任务，那么把tp时间片加进去
                }
            }

            return tasks;
        }
    }
}
