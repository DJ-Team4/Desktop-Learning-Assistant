<<<<<<< HEAD:Desktop-Learning-Assistant/TomatoClock/TaskTomatoContext.cs
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DesktopLearningAssistant.TomatoClock.SQLite
{
    /// <summary> 
    /// 创建 DbContext 类
    /// </summary> 
    public class TaskTomatoContext : DbContext
    {
        public TaskTomatoContext(DbContextOptions<TaskTomatoContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<TaskList> Tasks { get; set; }
        public DbSet<TaskTomatoList> TaskTomatoes { get; set; }
        public DbSet<TaskFileList> TaskFileLists { get; set; }
    }
}
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DesktopLearningAssistant.TomatoClock.SQLite
{
    /// <summary> 
    /// 创建 DbContext 类
    /// </summary> 
    public class TaskTomatoContext : DbContext
    {
        public TaskTomatoContext() : base("TaskDataBase")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<TaskTomatoContext>());
        }

        public DbSet<TaskList> Tasks { get; set; }
        public DbSet<TaskTomatoList> TaskTomatoes { get; set; }
    }
}
>>>>>>> 771b088233c1a3c3f4a8b61e05bc0f79c40421c6:TomatoClock/TaskTomatoContext.cs
