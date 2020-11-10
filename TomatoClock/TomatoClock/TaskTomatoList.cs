using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;

namespace DesktopLearningAssistant.TomatoClock.SQLite
{
    /// <summary> 
    /// 创建 Task 和 Tomato 两个实体类
    /// </summary> 
    public class TaskList
    {
        [Key,Column(Order =1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskID { get; set; } //auto primary key
        [Required]
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime Deadline { get; set; }
        public string Notes { get; set; }
        public int TomatoNum { get; set; }
        public int TomatoCount { get; set; }
        public int State { get; set; }
        //public List<TaskList> TaskLists { get; set; }
        //public List<TaskTomatoList> TaskTomatoLists { get; set; } //1..m
        //public List<TaskFileList> TaskFileLists { get; set; }

        public virtual ICollection<TaskTomatoList> TaskTomatoLists { get; set; }
            = new ObservableCollection<TaskTomatoList>();
        public virtual ICollection<TaskFileList> TaskFileLists { get; set; }
            = new ObservableCollection<TaskFileList>();
    }

    public class TaskTomatoList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TomatoID { get; set; }        //primary key
        [Required]
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<string> DefaultFocusApp { get; set; }

        [ForeignKey("TaskLists")]
        public int TaskID { get; set; }         //foreign key
        [Required]
        public virtual TaskList TaskLists { get; set; }          //m..1
    }

    public class TaskFileList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileID { get; set; }   //primary key
        [Required]
        public string FilePath { get; set; }

        [ForeignKey("TaskLists")]
        public int TaskID { get; set; }   //foreign key
        public virtual TaskList TaskLists { get; set; }
    }
}
