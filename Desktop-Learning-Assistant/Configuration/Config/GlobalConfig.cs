using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopLearningAssistant.Configuration.Config
{
    public class GlobalConfig
    {
        public double X { get; set; }
        public double Y { get; set; }

        public void SetDefault()
        {
            // 默认的配置
            X = SystemParameters.WorkArea.Width;
            Y = 0;
        }
    }
}
