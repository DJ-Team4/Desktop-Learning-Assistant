using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.Linq;

namespace DesktopLearningAssistant.TaskTomato.Model
{
    /// <summary> 
    /// 创建 Task 和 Tomato 两个实体类
    /// </summary> 
    public class TaskInfo
    {
        /// <summary>
        /// ID(Auto Primary Key)
        /// </summary>
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskID { get; set; }

        /// <summary>
        /// 任务名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 任务开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 任务截止时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 任务详情
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// 任务指定所需的番茄数
        /// </summary>
        public int TotalTomatoCount { get; set; }

        /// <summary>
        /// 已完成的番茄数
        /// </summary>
        public int FinishedTomatoCount { get; set; }

        /// <summary>
        /// 任务的完成状态
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// 任务的番茄
        /// </summary>
        public virtual ICollection<Tomato> Tomatoes { get; set; } = new ObservableCollection<Tomato>();

        /// <summary>
        /// 任务的相关文件
        /// </summary>
        public virtual ICollection<TaskFile> RelativeFiles { get; set; } = new ObservableCollection<TaskFile>();
    }
}
