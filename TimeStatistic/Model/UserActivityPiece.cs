using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeStatisticTest.TimeStatistic.Model
{
    public class UserActivityPiece
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CloseTime { get; set; }
        public TimeSpan SpanTime
        {
            get
            {
                return CloseTime - StartTime;
            }
        }

        public override bool Equals(object obj)
        {
            UserActivityPiece uap2 = obj as UserActivityPiece;
            if (uap2.Name == this.Name && uap2.Detail == this.Detail && uap2.StartTime == this.StartTime)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
