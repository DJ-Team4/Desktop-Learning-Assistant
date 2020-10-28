using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeStatisticTest.TimeStatistic.Model;

namespace TimeStatisticTest.TimeStatistic
{
    public class TimeDataManager
    {
        private static TimeDataManager uniqueTimeDataManager;
        private static readonly object locker = new object();   // 定义一个标识确保线程同步

        public List<Model.UserActivity> KilledActivity { get; set; }
        public List<UserActivityPiece> UserActivityPieces { get; set; }
        public Dictionary<string, string> TypeDict { get; set; }           // 每款软件与类型的字典集合

        public TimeDataManager()
        {
            this.KilledActivity = new List<UserActivity>();
            this.UserActivityPieces = new List<UserActivityPiece>();
            this.TypeDict = new Dictionary<string, string>();
        }

        public static TimeDataManager GetTimeDataManager()     // 获取单例对象
        {
            if (uniqueTimeDataManager == null)
            {
                lock(locker)
                {
                    uniqueTimeDataManager = new TimeDataManager();
                }
            }
            return uniqueTimeDataManager;
        }
    }
}
