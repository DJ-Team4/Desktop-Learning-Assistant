using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TaskTomato.Model;

namespace DesktopLearningAssistant.TaskTomato
{
    public interface ITomatoService
    {
        void AddTask(TaskInfo taskInfo);

        void DeletTask(int iTaskID);

        void ModifyTask(TaskInfo taskInfo);

        TaskInfo ReadTask(int iTaskID);

        void ChangeTaskToFinishState(int iTaskID);

        int AddTomatoStartTime(int iTaskID);

        void AddTomatoEndTime(int iTaskID, int iTomatoID);

        List<Tomato> ReadTomato(int iTaskID);

        List<string> RecentTenApp(DateTime iTime);
    }
}
