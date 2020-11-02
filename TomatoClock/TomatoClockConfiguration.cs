using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TomatoClock
{
    class TomatoClockConfiguration
    {
        static void DataInit(string[] args)
        {
            CreateDB("TaskInfo");

            var ds = string.Format("data source={0}", "D:/TaskInfo.db");

            //创建Task表
            using (SQLiteConnection conn = new SQLiteConnection(ds))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "" +
                        "CREATE TABLE Task (" +
                        "TaskID INT(10)     NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                        "name VARCHAR(100)," +
                        "StartTime INT(20)," +
                        "Deadline INT(20)," +
                        "notes VARCHAR(150)," +
                        "TomatoNum INT(3)," +
                        "TomatoCount INT(3)," +
                        "State INT(2)); ";
                    cmd.ExecuteNonQuery();
                }
            }

            //创建Task_Tomato表
            using (SQLiteConnection conn = new SQLiteConnection(ds))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandText = "" +
                        "CREATE TABLE Task_Tomato (" +
                        "TomatoID INT(10)     NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                        "TaskID INT(10)     FOREIGN KEY," +
                        "BeginTime INT(20)," +
                        "EndTime INT(20));";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool CreateDB(string db_name)
        {
            var fileName = "D:/" + db_name + ".db";
            SQLiteConnection.CreateFile(fileName);
            return true;
        }
    }
}
