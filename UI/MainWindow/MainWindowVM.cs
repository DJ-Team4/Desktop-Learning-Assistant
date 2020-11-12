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

        public MainWindowViewModel()
        {
            Update();
        }

        #region 属性
        /// <summary>
        /// 折线图集合
        /// </summary>
        public SeriesCollection LineSeriesCollection { get; set; } = new SeriesCollection();

        /// <summary>
        /// 柱状图集合
        /// </summary>
        public SeriesCollection ColumnSeriesCollction_today { get; set; } = new SeriesCollection();
        public SeriesCollection ColumnSeriesCollction_week { get; set; } = new SeriesCollection();


        /// <summary>
        /// 饼图图集合
        /// </summary>
        public SeriesCollection PieSeriesCollection_today { get; set; } = new SeriesCollection();
        public SeriesCollection PieSeriesCollection_week { get; set; } = new SeriesCollection();

        /// <summary>
        /// 折线图X坐标
        /// </summary>
        public List<string> LineXLabels { get; set; } = new List<string>();

        /// <summary>
        /// 柱状图X坐标
        /// </summary>
        public List<string> ColumnXLabels { get; set; } = new List<string>();
        #endregion

        #region 公有方法

        /// <summary>
        /// 更新界面数据
        /// </summary>
        public void Update()
        {
            //GetLineSeriesData();
            GetTodayColunmSeriesData();
            GetWeekColunmSeriesData();
            GetPieSeriesData_today();
            GetPieSeriesData_week();
        }

        #endregion

        #region 私有方法

        private void GetLineSeriesData()
        {
            List<string> titles = new List<string> { "苹果", "香蕉", "梨" };
            List<List<double>> values = new List<List<double>>
            {
                new List<double> { 30, 40, 60 },
                new List<double> { 20, 10, 50 },
                new List<double> { 10, 50, 30 }
            };
            List<string> _dates = new List<string>();
            _dates = GetCurrentMonthDates();
            for (int i = 0; i < titles.Count; i++)
            {
                LineSeries lineseries = new LineSeries();
                lineseries.DataLabels = true;
                lineseries.Title = titles[i];
                lineseries.Values = new ChartValues<double>(values[i]);
                LineXLabels.Add(_dates[i]);
                LineSeriesCollection.Add(lineseries);
            }
        }

        /// <summary>
        /// 根据给定的时间返回一个饼状图
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private SeriesCollection GetColumnSeries(DateTime beginTime,DateTime endTime)
        {
            SeriesCollection seriesCollection = new SeriesCollection();
            ColumnXLabels.Clear();
            seriesCollection.Clear();
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
            return seriesCollection;
        }

        /// <summary>
        /// 今日每个软件的时间统计柱状图
        /// </summary>
        private void GetTodayColunmSeriesData()
        {
            ColumnSeriesCollction_today = GetColumnSeries(DateTime.Today, DateTime.Now);
        }
        
        /// <summary>
        /// 一周每个软件的时间统计柱状图
        /// </summary>
        private void GetWeekColunmSeriesData()
        {
            ColumnSeriesCollction_week = GetColumnSeries(DateTime.Today.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))), DateTime.Now);
        }

        /// <summary>
        /// 根据给定的时间返回一个饼状图
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private SeriesCollection GetPieSeries(DateTime beginTime, DateTime endTime)
        {
            SeriesCollection seriesCollection = new SeriesCollection();

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

            return seriesCollection;
        }

        /// <summary>
        /// 更新今日的饼状图
        /// </summary>
        private void GetPieSeriesData_today()
        {
            PieSeriesCollection_today = GetPieSeries(DateTime.Today, DateTime.Now);
        }

        /// <summary>
        /// 更新一周的饼状图
        /// </summary>
        private void GetPieSeriesData_week()
        {
            PieSeriesCollection_week = GetPieSeries(DateTime.Today.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))), DateTime.Now);
        }

        /*private void ThreeColumnData()
        {
            List<string> titles = new List<string> { "苹果", "香蕉", "梨" };
            //三列示例数据
            List<List<double>> threeColunmValues = new List<List<double>>
            {
                new List<double> { 30, 40, 60 },
                new List<double> { 20, 10, 50 },
                new List<double> { 10, 50, 30 }
            };


            for (int i = 0; i < titles.Count; i++)
            {
                ColumnSeries colunmseries = new ColumnSeries();
                colunmseries.DataLabels = true;
                colunmseries.Title = titles[i];
                colunmseries.Values = new ChartValues<double>(threeColunmValues[i]);

                seriesCollection.Add(colunmseries);
            }
        }*/

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
