using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TimeStatistic.Model;

namespace DesktopLearningAssistant.TimeStatistic
{
    /// <summary>
    /// TimeDataManager管理TimeStatistic相关的所有数据，负责在初始化时从数据库读入历史数据，以及定时将新数据写入数据库
    /// </summary>
    public class TimeDataManager
    {
        #region 静态变量

        /// <summary>
        /// 单例对象
        /// </summary>
        private static TimeDataManager uniqueTimeDataManager;

        /// <summary>
        /// 确保线程同步的锁标识
        /// </summary>
        private static readonly object locker = new object();

        #endregion

        #region 公有属性

        /// <summary>
        /// 所有被关闭了的软件
        /// </summary>
        public List<UserActivity> KilledActivity { get; set; }

        /// <summary>
        /// 所有的活动片
        /// </summary>
        public List<UserActivityPiece> UserActivityPieces { get; set; }

        /// <summary>
        /// 软件的类型字典集合
        /// </summary>
        public Dictionary<string, string> TypeDict { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 构造方法
        /// </summary>
        public TimeDataManager()
        {
            this.KilledActivity = new List<UserActivity>();
            this.UserActivityPieces = new List<UserActivityPiece>();
            this.TypeDict = new Dictionary<string, string>();
        }

        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <returns></returns>
        public static TimeDataManager GetTimeDataManager()
        {
            if (uniqueTimeDataManager == null)
            {
                lock (locker)
                {
                    uniqueTimeDataManager = new TimeDataManager();
                }
            }
            return uniqueTimeDataManager;
        }

        #endregion
    }
}
