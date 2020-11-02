using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.Configuration;
using DesktopLearningAssistant.TimeStatistic.Model;
using Microsoft.EntityFrameworkCore;

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
        /// 数据库
        /// </summary>
        public TimeDataContext context;

        /// <summary>
        /// 所有被关闭了的软件
        /// </summary>
        public List<UserActivity> KilledActivity
        {
            get
            {
                return context.KilledActivities.ToList();
            }
        }

        /// <summary>
        /// 所有的活动片
        /// </summary>
        public List<UserActivityPiece> UserActivityPieces
        {
            get
            {
                return context.UserActivityPieces.ToList(); ;
            }
        }

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
            ConfigService configService = ConfigService.GetConfigService();
            string dbPath = configService.TSConfig.DbPath;
            var builder = new DbContextOptionsBuilder<TimeDataContext>();
            builder.UseSqlite($"Data Source={dbPath}");
            context = new TimeDataContext(builder.Options); // KilledActivities和UserActivitiesPieces通过数据库读写
            context.Database.EnsureCreated();

            TypeDict = configService.TSConfig.TypeDict;     // TypeDict通过配置文件读写
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

        /// <summary>
        /// 通过此接口把新的KilledActivity写入内存
        /// </summary>
        /// <param name="ua"></param>
        public async void AddKilledActivity(UserActivity ua)
        {
            await context.KilledActivities.AddAsync(ua);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// 通过此接口把新的UserActivityPiece写入内存
        /// </summary>
        /// <param name="uap"></param>
        public async void AddUserActivityPiece(UserActivityPiece uap)
        {
            await context.UserActivityPieces.AddAsync(uap);
            await context.SaveChangesAsync();
        }

        #endregion
    }
}
