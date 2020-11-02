using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
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
        /// 所有被关闭了的软件
        /// </summary>
        public List<UserActivity> KilledActivities { get; set; }

        /// <summary>
        /// 所有的活动片
        /// </summary>
        public List<UserActivityPiece> UserActivityPieces { get; set; }

        /// <summary>
        /// 软件的类型字典集合
        /// </summary>
        public Dictionary<string, string> TypeDict { get; set; }

        #endregion

        #region 私有属性

        /// <summary>
        /// Context构造字符串，放在此处以减少重复代码
        /// </summary>
        private DbContextOptions<TimeDataContext> options;

        /// <summary>
        /// 上一次从数据库中读出的实体数量，写回数据库时只写入新增加的
        /// </summary>
        private int lastUAPCount;
        private int lastKACount;

        #endregion

        #region 公有方法

        /// <summary>
        /// 构造方法
        /// </summary>
        public TimeDataManager()
        {
            // 构造options
            ConfigService configService = ConfigService.GetConfigService();
            string dbPath = configService.TSConfig.DbPath;
            var builder = new DbContextOptionsBuilder<TimeDataContext>();
            builder.UseSqlite($"Data Source={dbPath}");
            options = builder.Options;

            // 读出配置文件中的TypeDict
            TypeDict = ConfigService.GetConfigService().TSConfig.TypeDict;
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
                    uniqueTimeDataManager.EnsureDbCreated();            // 只检查一次是否建库建表
                    uniqueTimeDataManager.LoadDataFromDb();             // 读入数据

                    int SaveToDbTimeSlice = ConfigService.GetConfigService().TSConfig.SaveToDbTimeSlice;
                    System.Threading.Timer threadTimer = new System.Threading.Timer(
                        callback: new TimerCallback(TimeWriteToDb), null, 0, SaveToDbTimeSlice);
                }
            }
            return uniqueTimeDataManager;
        }

        /// <summary>
        /// 确保数据库已被创建
        /// </summary>
        public async void EnsureDbCreated()
        {
            using(var context = new TimeDataContext(options))
            {
                await context.Database.EnsureCreatedAsync();
            }
        }

        /// <summary>
        /// 从数据库中读出所有实体
        /// </summary>
        public void LoadDataFromDb()
        {
            using (var context = new TimeDataContext(options))
            {
                UserActivityPieces = context.UserActivityPieces.ToList();
                KilledActivities = context.KilledActivities.ToList();
                lastUAPCount = UserActivityPieces.Count;
                lastKACount = KilledActivities.Count;
            }
        }

        /// <summary>
        /// 将新增加的实体写入数据库
        /// </summary>
        public void SaveDataToDb()
        {
            using (var context = new TimeDataContext(options))
            {
                int newUAPCount = UserActivityPieces.Count - lastUAPCount;
                int newKACount = KilledActivities.Count - lastKACount;
                context.UserActivityPieces.AddRangeAsync(UserActivityPieces.GetRange(lastUAPCount, newUAPCount));
                context.KilledActivities.AddRangeAsync(KilledActivities.GetRange(lastKACount, newKACount));
                context.SaveChanges();
            }
        }

        #endregion

        #region 私有方法

        private static void TimeWriteToDb(object state)
        {
            uniqueTimeDataManager.SaveDataToDb();
        }

        #endregion
    }
}
