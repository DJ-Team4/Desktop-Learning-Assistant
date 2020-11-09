using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace DesktopLearningAssistant.TomatoClock.SQLite
{
    /// <summary> 
    /// 创建 DbContext 类
    /// </summary> 
    public class TaskTomatoContext : DbContext
    {
        public TaskTomatoContext() : base() { }

        public DbSet<TaskList> Tasks { get; set; }
        public DbSet<TaskTomatoList> TaskTomatoes { get; set; }
        public DbSet<TaskFileList> TaskFileLists { get; set; }
    }
}
