using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Demo;
using StatisticService.Model;

namespace StatisticService
{
    class Program
    {
        static void Main(string[] args)
        {
            StatisticService.StartMonitor();

            while(true)
            {
                Console.Clear();
                List<Task> tasks = StatisticService.GetAllTasks();
                foreach (var task in tasks)
                {
                    Console.WriteLine(task.ToString());
                }
                Thread.Sleep(1000);
            }
        }
    }
}
