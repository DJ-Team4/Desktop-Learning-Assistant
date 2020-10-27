using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.Configuration.Config
{
    [Serializable]
    class TomatoClockConfig
    {
        public string WorkSpan;   //设置一次番茄钟的工作时间
        public string RestSpan;   //设置一次番茄钟的间隔时间
    }
}
