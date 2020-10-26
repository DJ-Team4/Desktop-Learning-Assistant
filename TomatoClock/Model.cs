using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TomatoClock.TomatoTime;

namespace TomatoClock.Model
{
    public class TaskInfo
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime SetTime { get; set; }
        public Time SetSpan { get; set; }
        public Time LeftTime { get; set; }
    }
}
