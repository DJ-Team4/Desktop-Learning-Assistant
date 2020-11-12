using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DesktopLearningAssistant.TaskTomato.Model;

namespace DesktopLearningAssistant.TaskTomato
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

        public DbSet<TaskInfo> TaskModels { get; set; }

        public DbSet<Tomato> TomatoesModels { get; set; }

        public DbSet<TaskFile> TaskFileModels { get; set; }

        public DbSet<FocusApp> FocusAppModels { get; set; }
    }
}
