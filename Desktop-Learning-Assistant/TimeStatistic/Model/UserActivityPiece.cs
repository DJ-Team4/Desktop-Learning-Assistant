using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DesktopLearningAssistant.TimeStatistic.Model
{
    /// <summary>
    /// 活动片，软件作为前台窗口的记录
    /// </summary>
    public class UserActivityPiece
    {
        #region 公共属性

        /// <summary>
        /// ID，自增，不需要手动设置
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 活动片的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 活动片的窗口信息
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 活动片的开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 活动片的结束时间
        /// </summary>
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// 该活动片是否已结束
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// 活动片的时间跨度
        /// </summary>
        public TimeSpan SpanTime
        {
            get
            {
                return CloseTime - StartTime;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 判断两个活动片是否是同一款软件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            UserActivityPiece uap2 = obj as UserActivityPiece;
            if (uap2.Name == this.Name)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 随Equals一并生成的默认GetHashCode方法
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

    }
}
