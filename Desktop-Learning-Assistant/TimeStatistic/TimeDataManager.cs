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
        public static TimeDataManager uniqueTimeDataManager;

        public List<Model.UserActivity> KilledActivity { get; set; }
        public List<UserActivityPiece> UserActivityPieces { get; set; }

        public TimeDataManager GetTimeDataManager()     // 获取单例对象
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
