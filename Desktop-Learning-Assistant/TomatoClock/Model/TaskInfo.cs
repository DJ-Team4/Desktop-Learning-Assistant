using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TomatoClock.TomatoTime;

namespace DesktopLearningAssistant.TomatoClock.Model
{
    public class TaskInfo
    {
        List<Tomato> TomatoList = new List<Tomato>();  //task和Tomato clock是一对多关系，一个task中的Tomato clock用一个list来存储
        Tomato present_tomato = new Tomato();   //记录当前进行的番茄钟的信息
        public string Name { get; set; }
        public string Notes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime Deadline { get; set; }
    }
}
