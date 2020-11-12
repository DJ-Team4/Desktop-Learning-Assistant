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

        public void SetDefault()
        {
            DbPath = "TaskTomato.db";
        }
    }
}
