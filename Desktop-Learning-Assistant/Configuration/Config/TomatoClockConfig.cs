using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.Configuration.Config
{
    public class TomatoClockConfig
    {
        public string WorkSpan;   //设置一次番茄钟的工作时间
        public string RestSpan;   //设置一次番茄钟的间隔时间

        public Dictionary<string, List<string>> WhiteList;

        public void SetDefault()
        {
            WorkSpan = "25";
            RestSpan = "5";
            WhiteList = new Dictionary<string, List<string>>();
            WhiteList.Add("默认", new List<string> { "QQ", "chrome" });       // 默认的白名单
        }
    }
}
