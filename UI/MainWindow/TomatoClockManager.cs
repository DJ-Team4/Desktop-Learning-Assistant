using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace UI
{
    /// <summary>
    /// 番茄钟状态
    /// </summary>
    public enum ClockState
    {
        Stop,
        Working,
        Relaxing
    }

    /// <summary>
    /// 负责管理时钟状态和运行，且提供时钟完成、时钟变化等事件给调用者
    /// </summary>
    public class TomatoClockManager
    {
        #region 委托和事件

        /// <summary>
        /// 工作钟完成事件
        /// </summary>
        public delegate void WorkClockFinishedEventHandler(object sender);

        /// <summary>
        /// 休息钟完成事件
        /// </summary>
        public delegate void RelaxClockFinishedEventHandler(object sender);

        /// <summary>
        /// 一秒间隔事件
        /// </summary>
        public delegate void ClockTickDelegate(object sender, TimeSpan timeSpan);

        public event WorkClockFinishedEventHandler WorkClockFinishedEvent;
        public event RelaxClockFinishedEventHandler RelaxClockFinishedEvent;
        public event ClockTickDelegate ClockTickEvent;

        #endregion

        #region 变量

        private Timer clockTimer;
        private TimeSpan workTimeSpan;
        private TimeSpan relaxTimeSpan;

        /// <summary>
        /// clockTimer运行的时间间隔，达到一定间隔就重置clockTimer
        /// </summary>
        private TimeSpan clockTimeSpan;

        public ClockState clockState;

        public double Percentage
        {
            get
            {
                if (clockState == ClockState.Working)
                {
                    return clockTimeSpan.TotalSeconds / workTimeSpan.TotalSeconds;
                }
                else if (clockState == ClockState.Relaxing)
                {
                    return clockTimeSpan.TotalSeconds / relaxTimeSpan.TotalSeconds;
                }
                else
                {
                    return 1;
                }
            }
        } 

        #endregion

        #region 公有方法

        public TomatoClockManager(TimeSpan workTimeSpan, TimeSpan relaxTimeSpan)
        {
            clockTimer = new Timer();
            clockTimer.Interval = 1;        // TODO：测试完毕后改回秒
            clockTimer.Elapsed += OnTimedEvent;
            clockTimeSpan = TimeSpan.FromSeconds(0);
            clockState = ClockState.Stop;

            this.workTimeSpan = workTimeSpan;
            this.relaxTimeSpan = relaxTimeSpan;
        }

        public void StartWorkClock()
        {
            ResetClockTimer();
            clockTimer.Start();
            clockState = ClockState.Working;
        }

        public void StartRelaxClock()
        {
            ResetClockTimer();
            clockTimer.Start();
            clockState = ClockState.Relaxing;
        }

        public void AbortClock()
        {
            ResetClockTimer();
            clockTimer.Stop();
            clockState = ClockState.Stop;
        }

        #endregion

        #region 私有方法

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            clockTimeSpan += TimeSpan.FromSeconds(1);
            ClockTickEvent?.Invoke(this, clockTimeSpan);

            if (clockState == ClockState.Working && clockTimeSpan == workTimeSpan)  // 工作钟已完成
            {
                ResetClockTimer();
                clockTimer.Stop();
                clockState = ClockState.Stop;
                WorkClockFinishedEvent?.Invoke(this);
                return;
            }

            if (clockState == ClockState.Relaxing && clockTimeSpan == relaxTimeSpan)  // 工作钟已完成
            {
                ResetClockTimer();
                clockTimer.Stop();
                clockState = ClockState.Stop;
                RelaxClockFinishedEvent?.Invoke(this);
                return;
            }
        }

        private void ResetClockTimer()
        {
            clockTimeSpan = TimeSpan.FromSeconds(0);
        }

        #endregion
    }
}
