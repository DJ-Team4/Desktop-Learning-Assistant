using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.Configuration.Config
{
    public class GlobalConfig
    {
        public int X { get; set; }
        public int Y { get; set; }

        public void SetDefault()
        {
            // 默认的配置
            X = 0;
            Y = 0;
        }
    }
}
