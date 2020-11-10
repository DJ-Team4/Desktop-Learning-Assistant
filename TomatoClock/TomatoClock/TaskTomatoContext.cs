using System;
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