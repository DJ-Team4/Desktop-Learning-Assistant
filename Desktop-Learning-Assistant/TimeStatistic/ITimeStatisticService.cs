using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TimeStatistic.Model;

namespace DesktopLearningAssistant.TimeStatistic
{
    /// <summary>
    /// 屏幕时间统计服务的接口
    /// </summary>
    public interface ITimeStatisticService
    {
        /// <summary>
        /// 获取一段时间内的软件使用时间统计
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<UserActivity> GetUserActivitiesWithin(DateTime beginTime, DateTime endTime);

        /// <summary>
        /// 获取所有有记录的时间内的软件使用时间统计
        /// </summary>
        /// <returns></returns>
        List<UserActivity> GetAllUserActivities();

        /// <summary>
        /// 获取beginTime之后的被关闭软件的记录
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<UserActivity> GetKilledActivitiesWithin(DateTime beginTime);

        /// <summary>
        /// 获取一段时间内的各类软件使用时间
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<TypeActivity> GetTypeActivitiesWithin(DateTime beginTime, DateTime endTime);

        /// <summary>
        /// 获取所有有记录的时间内的各类软件使用时间
        /// </summary>
        /// <returns></returns>
        List<TypeActivity> GetAllTypeActivities();

        /// <summary>
        /// 获取被关闭了的前台软件
        /// </summary>
        /// <returns></returns>
        List<UserActivity> GetKilledUserActivities();
    }
}
