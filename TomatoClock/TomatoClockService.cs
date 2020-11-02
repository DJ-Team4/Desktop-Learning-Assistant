using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using DesktopLearningAssistant.TomatoClock.Model;
using DesktopLearningAssistant.TomatoClock.SQLite;

namespace DesktopLearningAssistant.TomatoClock
{
    class TaskService
    {

        public bool AddTask(TaskInfo task)
        {
            string sql = "INSERT INTO Task(name,StartTime,Deadline,notes,TomatoNum,TomatoCount,State)" +
                "values(@name,@StartTime,@Deadline,@notes,@TomatoNum,@TomatoCount,@State)";
            SQLiteHelper DB = new SQLiteHelper("D:/TaskInfo.db");
            SQLiteParameter[] parameters = new SQLiteParameter[]{
            new SQLiteParameter("@name",task.Name.ToString()),
            new SQLiteParameter("@StartTime",task.StartTime.ToString()),
            new SQLiteParameter("@Deadline",task.Deadline),
            new SQLiteParameter("@notes",task.Notes.ToString()),
            new SQLiteParameter("@TomatoNum",0),
            new SQLiteParameter("@TomatoCount",0),
            new SQLiteParameter("@State",task.TaskState.ToString())
            };
            DB.ExecuteNonQuery(sql, parameters);
            return true;
        }
        public void DeletTask(TaskInfo task)
        {
            SQLiteHelper DB = new SQLiteHelper("D:/TaskInfo.db");
            string sql = "DELETE FROM Task WHERE TaskID = '" + task.TaskID + "'";
            DB.ExecuteNonQuery(sql, null);
            Console.WriteLine(task.TaskID + "被删除！");
        }
        public bool ModifyTask(TaskInfo task)
        {
            ReadTask(task.TaskID);
            AddTask(task);

            return true;
        }
        public TaskInfo ReadTask(int TaskID)
        {
            TaskInfo taskInfo = new TaskInfo();
            string sql = "SELECT * FROM Task WHERE TaskID = '" + TaskID + "'";
            SQLiteHelper DB = new SQLiteHelper("D:/TaskInfo.db");
            DB.ExecuteReader(sql, null);

            return taskInfo;
        }
        public int AddTomatoStartTime(int TaskID)
        {
            int TomatoID = 0;
            return TomatoID;
        }
        public void AddTomatoEndTime(int TaskID, int TomatoID)
        {

        }
        public int ReadTomato(int TaskID)
        {
            int count = 0;
            return count;
        }
        private void AddTomatoNum()
        {

        }
    }
}
