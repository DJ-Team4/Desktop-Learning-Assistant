using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TaskTomato.Model
{
    /// <summary>
    /// 任务效率（前端模型）
    /// </summary>
    public class TaskEfficiency
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public int TaskID;

        /// <summary>
        /// 任务名
        /// </summary>
        public string Name;

        /// <summary>
        /// 任务效率
        /// </summary>
        public double Efficiency;
    }
}
