using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.Entity;
using DesktopLearningAssistant.TomatoClock.Model;
using DesktopLearningAssistant.TomatoClock.SQLite;

namespace DesktopLearningAssistant.TomatoClock.SQLite
{
    class TaskService
    {
        public void AddTask(TaskInfo taskInfo)
        {
            using (var context = new TaskTomatoContext())
            {
                var task = new TaskList()
                {
                    Name = taskInfo.Name,
                    Notes = taskInfo.Notes,
                    StartTime = taskInfo.StartTime,
                    Deadline = taskInfo.Deadline,
                    TomatoNum = taskInfo.TomatoNum,
                    TomatoCount = 0,
                    State = taskInfo.TaskState
                };
                context.Tasks.Add(task);
                context.SaveChanges();
            }
        }
        public void DeletTask(int TaskID)
        {
            using (var context = new TaskTomatoContext())
            {
                var task = context.Tasks.Include(t => t.TaskTomatoLists).FirstOrDefault(tt => tt.TaskID == TaskID);
                if (task != null)
                {
                    context.Tasks.Remove(task);
                    context.SaveChanges();
                }
            }
            /*
            SQLiteHelper DB = new SQLiteHelper("D:/TaskInfo.db");
            string sql = "DELETE FROM Task WHERE TaskID = '" + task.TaskID + "'";
            DB.ExecuteNonQuery(sql, null);
            Console.WriteLine(task.TaskID + "被删除！");
            */
        }
        public void ModifyTask(TaskInfo taskInfo)
        {
            using (var context = new TaskTomatoContext())
            {
                var task = new TaskList()
                {
                    TaskID = taskInfo.TaskID,
                    Name = taskInfo.Name,
                    Notes = taskInfo.Notes,
                    StartTime = taskInfo.StartTime,
                    Deadline = taskInfo.Deadline,
                    TomatoNum = taskInfo.TomatoNum,
                    TomatoCount = taskInfo.TomatoCount,
                    State = taskInfo.TaskState
                };
                context.Entry(task).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public TaskInfo ReadTask(int TaskID)
        {
            using (var context = new TaskTomatoContext())
            {
                TaskInfo taskInfo = new TaskInfo();
                var task = context.Tasks.SingleOrDefault(t => t.TaskID == TaskID);
                if (task != null)
                {
                    taskInfo.Name = task.Name;
                    taskInfo.Notes = task.Notes;
                    taskInfo.StartTime = task.StartTime;
                    taskInfo.Deadline = task.Deadline;
                    taskInfo.TomatoCount = task.TomatoCount;
                    taskInfo.TomatoNum = task.TomatoNum;
                    taskInfo.TaskState = task.State;
                }
                return taskInfo;
            }
            /*
            TaskInfo taskInfo = new TaskInfo();
            string sql = "SELECT * FROM Task WHERE TaskID = '" + TaskID + "'";
            SQLiteHelper DB = new SQLiteHelper("D:/TaskInfo.db");
            SQLiteDataReader dr = DB.ExecuteReader(sql, null);
            while (dr.Read())
            {
                taskInfo.TaskID = (int)dr["TaskID"];
                taskInfo.Name = dr["name"].ToString();
            }
            return taskInfo;
            */
        }
        public int AddTomatoStartTime(int iTaskID)
        {
            using (var context = new TaskTomatoContext())
            {
                var tomato = new TaskTomatoList() { BeginTime = DateTime.Now, TaskID = iTaskID };
                context.Entry(tomato).State = EntityState.Added;
                context.SaveChanges();
                return tomato.TomatoID;
            }
        }
        public void AddTomatoEndTime(int iTaskID, int iTomatoID)
        {
            using (var context = new TaskTomatoContext())
            {
                var tomato = new TaskTomatoList() { TaskID = iTaskID, TomatoID = iTomatoID, EndTime = DateTime.Now };
                context.Entry(tomato).State = EntityState.Modified;
                var task = context.Tasks.FirstOrDefault(t => t.TaskID == iTaskID);
                if (task != null)
                {
                    task.TomatoCount = AddTomatoNum(tomato.BeginTime, tomato.EndTime, task.TomatoCount);
                }
                context.SaveChanges();
            }
        }
        public List<Tomato> ReadTomato(int iTaskID)    //提供id为TaskID的所有番茄钟信息（起止时间）以查找时段内的应用程序信息
        {
            List<Tomato> TomatoList = new List<Tomato>();
            using (var context = new TaskTomatoContext())
            {
                var query = context.TaskTomatoes.Where(tt => tt.TaskList.TaskID == iTaskID).OrderBy(tt => tt.TomatoID);
                foreach (var tt in query)
                {
                    Tomato tomato = new Tomato();
                    tomato.StartTime = tt.BeginTime;
                    tomato.EndTime = tt.EndTime;
                    TomatoList.Add(tomato);
                }
                return TomatoList;
            }
        }
        private int AddTomatoNum(DateTime iBeginTime, DateTime iEndTime, int iTomatoCount)
        {
            TimeSpan ts1 = new TimeSpan(iBeginTime.Ticks);
            TimeSpan ts2 = new TimeSpan(iEndTime.Ticks);
            TimeSpan ts = ts2.Subtract(ts1);
            int TimeSpanSecond = int.Parse(ts.TotalSeconds.ToString());
            if (TimeSpanSecond >= 25 * 60) { return iTomatoCount++; }
            else { return iTomatoCount; }
        }
    }
}
