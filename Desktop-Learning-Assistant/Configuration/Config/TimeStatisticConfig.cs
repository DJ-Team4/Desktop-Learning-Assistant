using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.Configuration.Config
{
    public class TimeStatisticConfig
    {
        public int TimeSlice { get; set; }       // 轮询时间
        public int SaveToDbTimeSlice { get; set; }   // 存入数据库的轮询时间
        public string DbPath { get; set; }        // 数据库位置
        public Dictionary<string, string> TypeDict { get; set; }         // 软件类型
        public List<string> TypeList { get; set; }      // 允许设置的软件类型

        public void SetDefault()
        {
            // 默认的配置
            TimeSlice = 200;
            SaveToDbTimeSlice = 5000;
            DbPath = "TimeStatistic.db";
            TypeDict = new Dictionary<string, string>
            {
                { "UI", "学习" },
                { "devenv", "工具" },
                { "chrome", "工具" },
                { "QQ", "社交" }
            };

            TypeList = new List<string>()
            {
                "社交",
                "学习",
                "娱乐",
                "工具",
                "其他"
            };
        }
    }
}
