using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;

namespace DesktopLearningAssistant.TaskTomato.Model
{
    public class FocusApp
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FocusAppID { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [Required]
        public string AppName { get; set; }

        /// <summary>
        /// 文件对应的任务
        /// </summary>
        [ForeignKey("TomatoID")]
        public int TomatoID { get; set; }
    }
}
