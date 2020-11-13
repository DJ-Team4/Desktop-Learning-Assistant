using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;
using DesktopLearningAssistant.TimeStatistic.Model;
using DesktopLearningAssistant.TimeStatistic;
using System.Threading;

namespace UI
{
    class MainWindowViewModel
    {
        ITimeStatisticService timeStatisticService = new TimeStatisticService();

        #region 属性

        /// <summary>
        /// 柱状图集合
        /// </summary>
        public SeriesCollection TodayColumnSeriesCollction { get; set; }
        public SeriesCollection WeekColumnSeriesCollction { get; set; }


        /// <summary>
        /// 饼图图集合
        /// </summary>
        public SeriesCollection TodayPieSeriesCollection { get; set; }
        public SeriesCollection WeekPieSeriesCollection { get; set; }

        /// <summary>
        /// 柱状图X坐标
        /// </summary>
        public List<string> TodayColumnXLabels { get; set; }
        public List<string> WeekColumnXLabels { get; set; }
        #endregion

        #region 公有方法

        public MainWindowViewModel()
        {
            TodayColumnSeriesCollction = new SeriesCollection();
            WeekColumnSeriesCollction = new SeriesCollection();

            TodayPieSeriesCollection = new SeriesCollection();
            WeekPieSeriesCollection = new SeriesCollection();

            TodayColumnXLabels = new List<string>();
            WeekColumnXLabels = new List<string>();

            Update();
        }

        /// <summary>
        /// 更新界面数据
        /// </summary>
        public void Update()
        {
            GetTodayColunmSeriesData();
            GetWeekColunmSeriesData();
            GetTodayPieSeriesData();
            GetWeekPieSeriesData();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 今日每个软件的时间统计柱状图
        /// </summary>
        private void GetTodayColunmSeriesData()
        {
            GetColumnSeries(DateTime.Today, DateTime.Now, out SeriesCollection seriesCollection, out List<string> columnXLabels);
            TodayColumnSeriesCollction = seriesCollection;
            TodayColumnXLabels = columnXLabels;
        }
        
        /// <summary>
        /// 一周每个软件的时间统计柱状图
        /// </summary>
        private void GetWeekColunmSeriesData()
        {
            DateTime beginTime = DateTime.Today.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));
            GetColumnSeries(beginTime, DateTime.Now, out SeriesCollection seriesCollection, out List<string> columnXLabels);
            WeekColumnSeriesCollction = seriesCollection;
            WeekColumnXLabels = columnXLabels;
        }

        /// <summary>
        /// 更新今日的饼状图
        /// </summary>
        private void GetTodayPieSeriesData()
        {
            GetPieSeries(DateTime.Today, DateTime.Now, out SeriesCollection seriesCollection);
            TodayPieSeriesCollection = seriesCollection;
        }

        /// <summary>
        /// 更新一周的饼状图
        /// </summary>
        private void GetWeekPieSeriesData()
        {
            DateTime beginTime = DateTime.Today.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));
            GetPieSeries(beginTime, DateTime.Now, out SeriesCollection seriesCollection);
            WeekPieSeriesCollection = seriesCollection;
        }

        /// <summary>
        /// 根据给定的时间返回一个柱状图
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private void GetColumnSeries(DateTime beginTime, DateTime endTime, out SeriesCollection seriesCollection, out List<string> ColumnXLabels)
        {
            seriesCollection = new SeriesCollection();
            ColumnXLabels = new List<string>();
            ColumnSeries colunmSeries = new ColumnSeries();

            List<double> columnValues = new List<double>();
            List<UserActivity> userActivities = timeStatisticService.GetUserActivitiesWithin(beginTime, endTime);
            for (int i = 0; i < userActivities.Count && i < 5; i++)
            {
                ColumnXLabels.Add(userActivities[i].Name);
                columnValues.Add(Math.Round(userActivities[i].SpanTime.TotalMinutes, 2));
            }

            colunmSeries.DataLabels = true;
            colunmSeries.Values = new ChartValues<double>(columnValues);
            seriesCollection.Add(colunmSeries);
        }

        /// <summary>
        /// 根据给定的时间返回一个饼状图
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private void GetPieSeries(DateTime beginTime, DateTime endTime, out SeriesCollection seriesCollection)
        {
            seriesCollection = new SeriesCollection();

            // 数据迁移
            List<TypeActivity> ActivityData = timeStatisticService.GetTypeActivitiesWithin(beginTime, endTime);
            for (int i = 0; i < ActivityData.Count && i < 4; i++)
            {
                PieSeries series = new PieSeries();
                ChartValues<double> chartValue = new ChartValues<double>
                {
                    Math.Round(ActivityData[i].SpanTime.TotalMinutes, 2)
                };
                series.DataLabels = true;
                series.Title = ActivityData[i].TypeName;
                series.Values = chartValue;
                seriesCollection.Add(series);
            }
        }

        /// <summary>
        /// 获取当前月的每天的日期
        /// </summary>
        /// <returns>日期集合</returns>
        private List<string> GetCurrentMonthDates()
        {
            List<string> dates = new List<string>();
            DateTime dt = DateTime.Now;
            int year = dt.Year;
            int mouth = dt.Month;
            int days = DateTime.DaysInMonth(year, mouth);
            //本月第一天时间      
            DateTime dt_First = dt.AddDays(1 - (dt.Day));
            dates.Add(String.Format("{0:d}", dt_First.Date));

            for (int i = 1; i < days; i++)
            {
                DateTime temp = dt_First.AddDays(i);
                dates.Add(String.Format("{0:d}", temp.Date));
            }
            return dates;
        }

        #endregion
    }
}
