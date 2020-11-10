using System;
using System.Collections.Generic;

namespace DesktopLearningAssistant.TomatoClock.Model
{
    public class TaskInfo
    {
        List<Tomato> TomatoList = new List<Tomato>();  //task和Tomato clock是一对多关系，一个task中的Tomato clock用一个list来存储
        //记录当前任务的相关软件
        public int TaskID { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime Deadline { get; set; }
        public int TomatoNum { get; set; }
        public int TomatoCount { get; set; }
        public int TaskState { get; set; }   //记录任务完成状态
    }

}