using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TomatoClock.Model;
using DesktopLearningAssistant.TomatoClock.SQLite;

namespace DesktopLearningAssistant.TomatoClock
{
    
    class Program
    {
        static void Main(string[] args)
        {
            TaskInfo test = new TaskInfo();
            Console.WriteLine("Input task name:\n");
            test.Name = Console.ReadLine().ToString();
            Console.WriteLine("Input task notes:\n");
            test.Notes = Console.ReadLine().ToString();
            Console.WriteLine("Input task start time: yyyy-MM-dd\n");
            test.StartTime = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
            Console.WriteLine("Input task finish time: yyyy-MM-dd\n");
            test.Deadline = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
            Console.WriteLine("Input task podomoro number:\n");
            test.TomatoNum = int.Parse(Console.ReadLine());

            TaskService taskServicetest = new TaskService();
            taskServicetest.AddTask(test);
            TaskInfo test1 = taskServicetest.ReadTask(test.TaskID);
            Print(test1);

            /*
            Console.WriteLine("Change tast notes:\n");
            test.Notes = Console.ReadLine();
            taskServicetest.ModifyTask(test);
            TaskInfo test2 = taskServicetest.ReadTask(test.TaskID);

            Print(test1);
            Print(test2);
            */
            Console.WriteLine("Delete task: \n");
            taskServicetest.DeletTask(test.TaskID);
            if(test.TaskID == 0) Console.WriteLine("Task Deleted!");
            taskServicetest.ReadTask(test.TaskID);

            Console.ReadLine();
            
        }
        public static void Print(TaskInfo taskInfo)
        {
            Console.WriteLine("Task--------------------------\n");
            Console.WriteLine("Name:  " + taskInfo.Name);
            Console.WriteLine("StartTime:  " + taskInfo.StartTime.ToString("yyyy-MM-dd"));
            Console.WriteLine("Deadline:  " + taskInfo.Deadline.ToString("yyyy-MM-dd"));
            Console.WriteLine("TomatoClock number:  " + taskInfo.TomatoNum);
        }
    }
}
