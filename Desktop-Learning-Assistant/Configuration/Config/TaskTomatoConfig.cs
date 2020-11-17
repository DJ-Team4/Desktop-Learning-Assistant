using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.Configuration.Config
{
    public class TaskTomatoConfig
    {
        public string DbPath { get; set; }       // 数据库路径

        public Dictionary<string, List<string>> WhiteLists { get; set; }       // 所有白名单

        public void SetDefault()
        {
            DbPath = "TaskTomato.db";
            WhiteLists = new Dictionary<string, List<string>>
            {
                { "Coding", new List<string>() { "QQ", "vs" } }
            };
        }
    }
}
