using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DesktopLearningAssistant.TimeStatistic.Model
{
    public class UserActivity
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public TimeSpan SpanTime { get; set; }

        public UserActivity(UserActivityPiece uap)
        {
            this.Name = uap.Name;
            this.Detail = uap.Detail;
            this.SpanTime = uap.SpanTime;
        }

        public bool IsSameActivity(UserActivityPiece uap)
        {
            if (uap.Name == this.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddUserActivityPiece(UserActivityPiece uap)
        {
            this.SpanTime += uap.SpanTime;
            this.Detail = uap.Detail;     // 保持最新的详细信息
        }

        public override string ToString()
        {
            return $"Name: {Name}\n\t{SpanTime.TotalSeconds}s\n\t{Detail}";
        }
    }
}
