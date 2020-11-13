using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DesktopLearningAssistant.TaskTomato.Model
{
    public class TaskFile
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileID { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [Required]
        public string FilePath { get; set; }

        /// <summary>
        /// 文件对应的任务
        /// </summary>
        [ForeignKey("TaskInfo")]
        public int TaskID { get; set; }

        public virtual TaskInfo TaskInfo { get; set; }
    }
}
