using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TimeStatistic.Model
{
    /// <summary>
    /// 类别的名称和统计时间，前后端中间件
    /// </summary>
    public class TypeActivity
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        public String TypeName { get; set; }

        /// <summary>
        /// 类别的使用时间
        /// </summary>
        public TimeSpan SpanTime { get; set; }
    }
}
