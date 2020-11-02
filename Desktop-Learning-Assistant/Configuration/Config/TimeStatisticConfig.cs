using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.Configuration.Config
{
    public class TimeStatisticConfig
    {
        public int TimeSlice;       // 轮询时间
        public string DbPath = "TimeStatistic.db";        // 数据库位置
        public Dictionary<string, string> TypeDict;         // 软件类型
    }
}
