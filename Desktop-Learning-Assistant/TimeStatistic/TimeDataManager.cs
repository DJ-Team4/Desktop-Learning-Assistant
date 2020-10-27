using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TimeStatistic.Model;

namespace DesktopLearningAssistant.TimeStatistic
{
    public class TimeDataManager
    {
        private static TimeDataManager uniqueTimeDataManager;

        public List<Model.UserActivity> KilledActivity { get; set; }
        public List<UserActivityPiece> UserActivityPieces { get; set; }
        public Dictionary<string, string> TypeDict { get; set; }           // 每款软件与类型的字典集合

        public static TimeDataManager GetTimeDataManager()     // 获取单例对象
        {
            if (uniqueTimeDataManager == null)
            {
                lock(uniqueTimeDataManager)
                {
                    uniqueTimeDataManager = new TimeDataManager();
                }
            }
            return uniqueTimeDataManager;
        }
    }
}
