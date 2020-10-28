using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeStatisticTest.TimeStatistic.Model;

namespace TimeStatisticTest.TimeStatistic
{
    public class TimeStatisticService
    {
        private static TimeStatisticService uniqueTimeStatisticService;
        private static readonly object locker = new object();   // 定义一个标识确保线程同步

        private TimeDataManager TDManager;

        public TimeStatisticService()
        {
            TDManager = TimeDataManager.GetTimeDataManager();       // 注入TimeDataManager
        }

        public static TimeStatisticService GetTimeStatisticService()
        {
            if (uniqueTimeStatisticService == null)
            {
                lock (locker)
                {
                    uniqueTimeStatisticService = new TimeStatisticService();
                }
            }
            return uniqueTimeStatisticService;
        }

        public List<UserActivity> GetUserActivitiesWithin(DateTime beginTime, DateTime endTime)
        {
            lock(TDManager.UserActivityPieces)
            {
                List<UserActivityPiece> userActivityPieces = TDManager.UserActivityPieces;
                List<UserActivityPiece> piecesWithSpan = userActivityPieces.FindAll(uap => uap.StartTime >= beginTime && uap.CloseTime <= endTime);
                return MergeUserActivityPiece(piecesWithSpan);
            }
        }

        public List<UserActivity> GetAllUserActivities()
        {
            lock (TDManager.UserActivityPieces)
            {
                List<UserActivityPiece> userActivityPieces = TDManager.UserActivityPieces;
                return MergeUserActivityPiece(TDManager.UserActivityPieces);
            }
        }

        public List<UserActivity> GetKilledUserActivities()
        {
            return TDManager.KilledActivity;
        }

        public void ChangeTaskType(string taskName, string typeName)
        {
            TDManager.TypeDict[taskName] = typeName;
        }

        private List<UserActivity> MergeUserActivityPiece(List<UserActivityPiece> uaps)      // 把给定的一堆任务片统计成不重复的任务
        {
            List<UserActivity> userActivities = new List<UserActivity>();

            foreach (UserActivityPiece uap in uaps)        // 把时间片累计起来
            {
                UserActivity t = userActivities.FirstOrDefault(task => task.IsSameActivity(uap));
                if (t == null)
                {
                    t = new UserActivity(uap);
                    userActivities.Add(t);
                }
                else
                {
                    t.AddUserActivityPiece(uap);      // 如果已经创建了这个进程的任务，那么把tp时间片加进去
                }
            }

            return userActivities;
        }
    }
}
