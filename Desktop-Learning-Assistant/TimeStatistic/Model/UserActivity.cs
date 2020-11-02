using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DesktopLearningAssistant.TimeStatistic.Model
{
    /// <summary>
    /// 用户活动，即一个软件的使用记录
    /// </summary>
    public class UserActivity
    {
        #region 公共属性

        /// <summary>
        /// 活动的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 活动的窗口标题信息
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 活动被关闭的时间
        /// </summary>
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// 活动的时间长度
        /// </summary>
        public TimeSpan SpanTime { get; set; }

        #endregion

        #region 方法

        public UserActivity()
        {
            Name = "Null";
            Detail = "Null";
            CloseTime = DateTime.Now;
            SpanTime = TimeSpan.Zero;
        }

        /// <summary>
        /// 从一个活动片中构造活动
        /// </summary>
        /// <param name="uap"></param>
        public UserActivity(UserActivityPiece uap)
        {
            this.Name = uap.Name;
            this.Detail = uap.Detail;
            this.CloseTime = uap.CloseTime;
            this.SpanTime = uap.SpanTime;
        }

        /// <summary>
        /// 判断某个活动片是否和本活动是同一个软件
        /// </summary>
        /// <param name="uap"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 将和本活动是同一软件的活动片的时间叠加到本活动中
        /// </summary>
        /// <param name="uap"></param>
        public void AddUserActivityPiece(UserActivityPiece uap)
        {
            this.Detail = uap.Detail;     // 保持最新信息
            this.CloseTime = uap.CloseTime;
            this.SpanTime += uap.SpanTime;
        }

        /// <summary>
        /// 将活动信息转为字符串，便于调试和打印日志
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Name: {Name}\n\t{SpanTime.TotalSeconds}s\n\t{Detail}";
        }
        #endregion
    }
}
