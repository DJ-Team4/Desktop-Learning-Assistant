using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data.Entity;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DesktopLearningAssistant.TomatoClock.SQLite
{
    /// <summary> 
    /// 创建 Task 和 Tomato 两个实体类
    /// </summary> 
    public class TaskList
    {
        [Key,Column(Order =1)]
        public int TaskID { get; set; } //auto primary key
        [Required]
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime Deadline { get; set; }
        public string Notes { get; set; }
        public int TomatoNum { get; set; }
        public int TomatoCount { get; set; }
        public int State { get; set; }
        public List<TaskTomatoList> TaskTomatoLists { get; set; } //1..m
    }

    public class TaskTomatoList
    {
        public int TomatoID { get; set; }        //primary key
        [Required]
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }

        public int TaskID { get; set; }         //foreign key
        public TaskList TaskLists { get; set; }          //m..1
    }

}
