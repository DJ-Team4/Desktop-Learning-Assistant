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
        public bool AddTask(TaskInfo task)
        {
            return true;
        }
        public void DeletTask()
        {
            
        }
        public bool ModifyTask()
        {
            return true;
        }
        public TaskInfo ReadTasK(int TaskID)
        {
            TaskInfo taskInfo = new TaskInfo();
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
