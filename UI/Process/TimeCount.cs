using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Process
{
    public class TimeCount
    {
        /// <summary>
        /// 实现倒计时功能的类
        /// </summary>
   
            private Int32 _TotalSecond;
            public Int32 TotalSecond
            {
                get { return _TotalSecond; }
                set { _TotalSecond = value; }
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            public TimeCount(Int32 totalSecond)
            {
                this._TotalSecond = totalSecond;
            }

            /// <summary>
            /// 减秒
            /// </summary>
            /// <returns></returns>
            public bool TimeCountDown()
            {
                if (_TotalSecond == 0)
                { return false;}
                else
                {
                    _TotalSecond--;
                    return true;
                }
            }

            /// <summary>
            /// 获取小时显示值
            /// </summary>
            /// <returns></returns>
            public string GetHour()
            {
                return String.Format("{0:D2}", (_TotalSecond / 3600));
            }

            /// <summary>
            /// 获取分钟显示值
            /// </summary>
            /// <returns></returns>
            public string GetMinute()
            {
                return String.Format("{0:D2}", (_TotalSecond % 3600) / 60);
            }

            /// <summary>
            /// 获取秒显示值
            /// </summary>
            /// <returns></returns>
            public string GetSecond()
            {
                return String.Format("{0:D2}", _TotalSecond % 60);
            }

        }

    
}
