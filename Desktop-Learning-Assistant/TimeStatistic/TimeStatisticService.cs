using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TimeStatistic.Model;

namespace DesktopLearningAssistant.TimeStatistic
{
    /// <summary>
    /// 屏幕时间统计的服务类，负责将活动数据进行转换，按指定要求返回活动统计信息
    /// </summary>
    public class TimeStatisticService : ITimeStatisticService
    {
        #region 静态变量

        /// <summary>
        /// 单例变量
        /// </summary>
        private static TimeStatisticService uniqueTimeStatisticService;

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

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public TimeStatisticService()
        {
            TDManager = TimeDataManager.GetTimeDataManager();       // 注入TimeDataManager
        }

        #region 接口方法

        /// <summary>
        /// 获取单例对象的方法
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 获取指定时间范围内的活动统计
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<UserActivity> GetUserActivitiesWithin(DateTime beginTime, DateTime endTime)
        {
            lock (TDManager.UserActivityPieces)
            {
                List<UserActivityPiece> userActivityPieces = TDManager.UserActivityPieces;
                List<UserActivityPiece> piecesWithSpan = userActivityPieces.FindAll(uap => uap.StartTime >= beginTime && uap.CloseTime <= endTime);
                List<UserActivity> userActivities = MergeUserActivityPiece(piecesWithSpan).OrderByDescending(ua => ua.SpanTime).ToList();   // 按时间长度降序排序
                return userActivities;
            }
        }

        /// <summary>
        /// 获得所有记录内的活动统计
        /// </summary>
        /// <returns></returns>
        public List<UserActivity> GetAllUserActivities()
        {
            lock (TDManager.UserActivityPieces)
            {
                List<UserActivityPiece> userActivityPieces = TDManager.UserActivityPieces;
                List<UserActivity> userActivities = MergeUserActivityPiece(TDManager.UserActivityPieces).OrderByDescending(ua => ua.SpanTime).ToList();
                return userActivities;
            }
        }

        /// <summary>
        /// 获取beginTime之后的被关闭软件的记录
        /// </summary>
        /// <param name="beginTime"></param>
        /// <returns></returns>
        public List<UserActivity> GetKilledActivitiesWithin(DateTime beginTime)
        {
            lock (TDManager.KilledActivity)
            {
                List<UserActivity> killedActivities = TDManager.KilledActivity;
                return killedActivities.FindAll(ka => ka.CloseTime >= beginTime);
            }

        }

        /// <summary>
        /// 获取所有被杀死了的软件
        /// </summary>
        /// <returns></returns>
        public List<UserActivity> GetKilledUserActivities()
        {
            return TDManager.KilledActivity;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 将活动片聚合成活动
        /// </summary>
        /// <param name="uaps"></param>
        /// <returns></returns>
        private List<UserActivity> MergeUserActivityPiece(List<UserActivityPiece> uaps)
        {
            List<UserActivity> userActivities = new List<UserActivity>();

            foreach (UserActivityPiece uap in uaps)        // 把活动片累计起来
            {
                UserActivity t = userActivities.FirstOrDefault(task => task.IsSameActivity(uap));
                if (t == null)
                {
                    t = new UserActivity(uap);
                    userActivities.Add(t);
                }
                else
                {
                    t.AddUserActivityPiece(uap);      // 如果已经创建了这个进程的任务，那么把活动片加进去
                }
            }

            return userActivities;
        }

        #endregion
    }
}
