using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DesktopLearningAssistant.TomatoClock.SQLite
{
    class TaskTomatoContext
    {
        /// <summary> 
        /// 创建 Task 和 Tomato 两个实体类
        /// </summary> 
        public class Task
        {
            public int TaskID { get; set; } //auto primary key
            public string Name { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime Deadline { get; set; }
            public string Notes { get; set; }
            public int TomatoNum { get; set; }
            public int TomatoCount { get; set; }
            public bool State { get; set; }
        }

        public class TaskTomato
        {
            public int TomatoID { get; set; }
            public DateTime BeginTime { get; set; }
            public DateTime EndTime { get; set; }

            public int TaskID { get; set; } //foreign key
            public Task Task { get; set; } //m..1
        }

        /// <summary> 
        /// 创建 DbContext 类
        /// </summary> 
        public class TaskContext : DbContext
        {
            public TaskContext() : base("TaskDataBase")
            {
                Database.SetInitializer(new DropCreateDatabaseIfModelChanges<TaskContext>());
            }

            public DbSet<Task> Tasks { get; set; }
            public DbSet<TaskTomato> TaskTomatoes { get; set; }
        }
    }
}
