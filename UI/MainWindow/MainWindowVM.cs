using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;
using DesktopLearningAssistant.TimeStatistic.Model;
using DesktopLearningAssistant.TimeStatistic;

namespace UI
{
    class MainWindowViewModel
    {
        ITimeStatisticService timeStatisticService = new TimeStatisticService();

        public MainWindowViewModel()
        {
            GetLineSeriesData();
            GetColunmSeriesData();
            GetPieSeriesData_today();
            GetPieSeriesData_yesterday();
        }
        #region 属性
        /// <summary>
        /// 折线图集合
        /// </summary>
        public SeriesCollection LineSeriesCollection { get; set; } = new SeriesCollection();

        /// <summary>
        /// 柱状图集合
        /// </summary>
        public SeriesCollection ColunmSeriesCollection { get; set; } = new SeriesCollection();

        /// <summary>
        /// 饼图图集合
        /// </summary>
        public SeriesCollection PieSeriesCollection_today { get; set; } = new SeriesCollection();
        public SeriesCollection PieSeriesCollection_yesterday { get; set; } = new SeriesCollection();

        /// <summary>
        /// 折线图X坐标
        /// </summary>
        public List<string> LineXLabels { get; set; } = new List<string>();

        /// <summary>
        /// 柱状图X坐标
        /// </summary>
        public List<string> ColumnXLabels { get; set; } = new List<string>();
        #endregion

        #region 方法
        void GetLineSeriesData()
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

        void GetColunmSeriesData()
        {
            List<double> columnValues = new List<double> ();
            List<UserActivity> userActivities = timeStatisticService.GetUserActivitiesWithin(DateTime.Today, DateTime.Now);
            for (int i = 0; i < userActivities.Count; i++)
            {
                ColumnXLabels.Add(userActivities[i].Name);
            }
            
            ColumnSeries colunmseries = new ColumnSeries();
            colunmseries.DataLabels = true;
            for (int i = 0; i < userActivities.Count; i++)
            {
                columnValues.Add(userActivities[i].SpanTime.TotalHours);
            }
            colunmseries.Values = new ChartValues<double>(columnValues);
            ColunmSeriesCollection.Add(colunmseries);

        }

        void GetPieSeriesData_today()
        {
            List<string> titles=new List<string> ();
            List<TypeActivity> ActivityData = timeStatisticService.GetTypeActivitiesWithin(DateTime.Today,DateTime.Now);
            for(int i=0;i < ActivityData.Count && i < 4;i++)
            {
                titles.Add(ActivityData[i].TypeName);
            }
            List<double> pieValues = new List<double> ();
            for (int i = 0; i < ActivityData.Count && i < 4; i++)
            {
                pieValues.Add(ActivityData[i].SpanTime.TotalMinutes);
            }
            ChartValues<double> chartvalue;
            for (int i = 0; i < ActivityData.Count && i < 4; i++)
            {
                chartvalue = new ChartValues<double>
                {
                    pieValues[i]
                };
                PieSeries series = new PieSeries();
                series.DataLabels = true;
                series.Title = titles[i];
                series.Values = chartvalue;
                PieSeriesCollection_today.Add(series);
            }
        }

        void GetPieSeriesData_yesterday()
        {
            List<string> titles = new List<string> ();
            List<double> pieValues = new List<double> ();
            List<TypeActivity> ActivityData = timeStatisticService.GetTypeActivitiesWithin(DateTime.Today.AddDays(-1), DateTime.Today.AddSeconds(-1));
            for (int i = 0; i < ActivityData.Count && i < 4; i++)
            {
                titles.Add(ActivityData[i].TypeName);
            }
            for (int i = 0; i < ActivityData.Count && i < 4; i++)
            {
                pieValues.Add(ActivityData[i].SpanTime.TotalHours);
            }
            ChartValues<double> chartvalue = new ChartValues<double>();
            for (int i = 0; i < titles.Count; i++)
            {
                chartvalue = new ChartValues<double>();
                chartvalue.Add(pieValues[i]);
                PieSeries series = new PieSeries();
                series.DataLabels = true;
                series.Title = titles[i];
                series.Values = chartvalue;
                PieSeriesCollection_yesterday.Add(series);
            }
        }

        void ThreeColumnData()
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

                ColunmSeriesCollection.Add(colunmseries);
            }
        }

        /// <summary>
        /// 获取当前月的每天的日期
        /// </summary>
        /// <returns>日期集合</returns>
        List<string> GetCurrentMonthDates()
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
