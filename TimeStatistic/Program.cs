using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using TimeStatisticTest.TimeStatistic;
using TimeStatisticTest.TimeStatistic.Model;

namespace TimeStatisticTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ActivityMonitor monitor = ActivityMonitor.GetMonitor();
            monitor.Start();
            TimeStatisticService timeStatisticService = TimeStatisticService.GetTimeStatisticService();

            while (true)
            {
                DateTime beginTime = DateTime.Now - TimeSpan.FromSeconds(5);
                DateTime endTime = DateTime.Now;
                
                List<UserActivity> userActivities = timeStatisticService.GetUserActivitiesWithin(beginTime, endTime);   // 获取最近五秒的活动统计
                // List<UserActivity> userActivities = timeStatisticService.GetAllUserActivities();        // 获取所有的活动统计
                Console.Clear();
                //Console.WriteLine(beginTime.ToString());
                //Console.WriteLine(endTime.ToString());
                foreach (UserActivity userActivity in userActivities)
                {
                    Console.WriteLine(userActivity.ToString());
                }
                Console.WriteLine("\n————————————————————\n");
                List<UserActivity> killedActivities = timeStatisticService.GetKilledUserActivities();
                foreach (UserActivity killedActivity in killedActivities)
                {
                    Console.WriteLine(killedActivity.ToString());
                }
                Thread.Sleep(1000);
            }
            
            
        }
    }
}
