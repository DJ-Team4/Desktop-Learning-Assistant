using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TomatoClock.Model;

namespace DesktopLearningAssistant.TomatoClock
{
    class TaskService
    {
        private int nextID;
        List<TaskInfo> TaskInfos;
        public void AddTask(TaskInfo task)
        {
            task.TaskID = nextID;
            nextID++;
            TaskInfos.Add(task);
            //////////////////////////////////////////////////////
            TaskInfo NewTask = new TaskInfo();
            Console.WriteLine("Task Infomation: \n");
            NewTask.Name = Console.ReadLine();

            Console.WriteLine("Set Start Time: \n"+"eg:For 2020/10/26 8:39:00, Input 2020102684100 ");
            string SetStartLine = Console.ReadLine();
            //NewTask.SetTime = DateTime.ParseExact(SetStartLine, "yyyyMMddhhmmss", System.Globalization.CultureInfo.CurrentCulture);

            Console.WriteLine("Set Time Span: ");

        }
        public void DeletTask() { }
        public void ModifyTask() { }
        public void ShowTask() { }
    }
}
