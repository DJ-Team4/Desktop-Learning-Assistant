using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TomatoClock.TomatoTime
{
    public class Time
    {
        private int sec;
        private int min;
        private bool stop;
        public Time(double min)
        {
            this.min = (int)min;
            sec = (int)((min - this.min) * 60);
            stop = false;
        }
        public void SubOneSecond()
        {
            if (sec > 0)
                sec--;
            else if (min > 0)
            {
                min--;
                sec = 59;
            }
            else
                stop = true;
        }
        public bool Stop
        {
            get { return Stop; }
        }
        public string ShowTime()
        {
            StringBuilder SB = new StringBuilder();
            SB.Append(min < 10 ? "0" + min : min.ToString()).Append(sec < 10 ? "0" + sec : sec.ToString());
            return SB.ToString();
        }
    }
}
