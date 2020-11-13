using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.Linq;

namespace DesktopLearningAssistant.TaskTomato.Model
{
    public class Tomato
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TomatoID { get; set; }

        /// <summary>
        /// 番茄起始时间
        /// </summary>
        [Required]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 番茄结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 对应的任务
        /// </summary>
        [ForeignKey("TaskInfo")]
        public int TaskID { get; set; }

        /// <summary>
        /// 对应的任务实体
        /// </summary>
        [Required]
        public virtual TaskInfo TaskInfo { get; set; }

        /// <summary>
        /// 专注的软件列表
        /// </summary>
        public virtual ICollection<FocusApp> FocusApps { get; set; } = new ObservableCollection<FocusApp>();
    }
}
