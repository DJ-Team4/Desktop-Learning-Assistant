using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;
using DesktopLearningAssistant.TimeStatistic.Model;
using DesktopLearningAssistant.TimeStatistic;
using DesktopLearningAssistant.TaskTomato;
using DesktopLearningAssistant.TaskTomato.Model;
using System.Windows;
using System.Threading;
using DesktopLearningAssistant.Configuration;
using System.Drawing;

namespace UI
{
    public class MainWindowViewModel
    {
        ITimeStatisticService timeStatisticService = new TimeStatisticService();
        TaskTomatoService taskTomatoService = new TaskTomatoService();

        #region 属性

        /// <summary>
        /// 折线图集合
        /// </summary>
        public SeriesCollection LineSeriesCollection { get; set; }

        /// <summary>
        /// 柱状图集合
        /// </summary>
        public SeriesCollection TodayColumnSeriesCollection { get; set; }
        public SeriesCollection WeekColumnSeriesCollection { get; set; }


        /// <summary>
        /// 饼图图集合
        /// </summary>
        public SeriesCollection TodayPieSeriesCollection { get; set; }
        public SeriesCollection WeekPieSeriesCollection { get; set; }

        /// <summary>
        /// 折线图X坐标
        /// </summary>
        public List<string> LineXLabels { get; set; }

        /// <summary>
        /// 柱状图X坐标
        /// </summary>
        public List<string> TodayColumnXLabels { get; set; }
        public List<string> WeekColumnXLabels { get; set; }

        public int CurrentTaskId { get; set; }
        public List<RelativeFileItem> RelativeFileItems { get; set; }

        /// <summary>
        /// 窗口起始位置
        /// </summary>
        public double left = SystemParameters.WorkArea.Width;

        /// <summary>
        /// 白名单列表
        /// </summary>
        public List<string> WhiteListNames { get; set; }

        #endregion

        #region 公有方法

        public MainWindowViewModel()
        {
            LineSeriesCollection = new SeriesCollection();
            LineXLabels = new List<string>();

            TodayColumnSeriesCollection = new SeriesCollection();
            WeekColumnSeriesCollection = new SeriesCollection();

            TodayPieSeriesCollection = new SeriesCollection();
            WeekPieSeriesCollection = new SeriesCollection();

            TodayColumnXLabels = new List<string>();
            WeekColumnXLabels = new List<string>();

            CurrentTaskId = 1;
            RelativeFileItems = new List<RelativeFileItem>();
            WhiteListNames = new List<string>();

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
            GetRecentFiles();
            GetLineSeriesData();
            GetWhiteLists();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 最近k个任务效率折线图
        /// </summary>
        private void GetLineSeriesData()
        {
            LineSeriesCollection.Clear();
            LineXLabels.Clear();
            List<double> values = new List<double>() { 0.7, 0.8, 0.65, 0.89, 0.73, 0.3, 0.56, 0.74};
            //List<TaskEfficiency> taskEfficiencies = taskTomatoService.GetTaskEfficiencies(DateTime.Now, 5);
            //for (int i = 0; i < taskEfficiencies.Count && i < 8; i++)
            //{
                //LineXLabels.Add(taskEfficiencies[i].Name);
                //values.Add(taskEfficiencies[i].Efficiency);
            //}
            
            LineSeriesCollection.Add(new LineSeries
            {
                //Title = "Today",
                DataLabels = false,
                Values = new ChartValues<double>(values)
            });
        }

        /// <summary>
        /// 今日每个软件的时间统计柱状图
        /// </summary>
        private void GetTodayColunmSeriesData()
        {
            TodayColumnSeriesCollection.Clear();
            TodayColumnXLabels.Clear();

            List<double> columnValues = new List<double>();
            List<UserActivity> userActivities = timeStatisticService.GetUserActivitiesWithin(DateTime.Today, DateTime.Now);
            for (int i = 0; i < userActivities.Count && i < 5; i++)
            {
                TodayColumnXLabels.Add(userActivities[i].Name);
                columnValues.Add(Math.Round(userActivities[i].SpanTime.TotalMinutes, 2));
            }

            TodayColumnSeriesCollection.Add(new ColumnSeries
            {
                Title = "Today",
                Values = new ChartValues<double>(columnValues),
                DataLabels = true
            });
        }
        
        /// <summary>
        /// 一周每个软件的时间统计柱状图
        /// </summary>
        private void GetWeekColunmSeriesData()
        {
            DateTime beginTime = DateTime.Today.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));
            WeekColumnSeriesCollection.Clear();
            WeekColumnXLabels.Clear();

            List<double> columnValues = new List<double>();
            List<UserActivity> userActivities = timeStatisticService.GetUserActivitiesWithin(beginTime, DateTime.Now);
            for (int i = 0; i < userActivities.Count && i < 5; i++)
            {
                WeekColumnXLabels.Add(userActivities[i].Name);
                columnValues.Add(Math.Round(userActivities[i].SpanTime.TotalMinutes, 2));
            }

            WeekColumnSeriesCollection.Add(new ColumnSeries
            {
                Title = "Week",
                Values = new ChartValues<double>(columnValues),
                DataLabels = true
            });
        }

        /// <summary>
        /// 更新今日的饼状图
        /// </summary>
        private void GetTodayPieSeriesData()
        {
            TodayPieSeriesCollection.Clear();

            List<TypeActivity> ActivityData = timeStatisticService.GetTypeActivitiesWithin(DateTime.Today, DateTime.Now);
            for (int i = 0; i < ActivityData.Count; i++)
            {
                PieSeries series = new PieSeries();
                ChartValues<double> chartValue = new ChartValues<double>
                {
                    Math.Round(ActivityData[i].SpanTime.TotalMinutes, 2)
                };
                series.DataLabels = true;
                series.Title = ActivityData[i].TypeName;
                series.Values = chartValue;
                TodayPieSeriesCollection.Add(series);
            }
        }

        /// <summary>
        /// 更新一周的饼状图
        /// </summary>
        private void GetWeekPieSeriesData()
        {
            DateTime beginTime = DateTime.Today.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));
            WeekPieSeriesCollection.Clear();

            List<TypeActivity> ActivityData = timeStatisticService.GetTypeActivitiesWithin(beginTime, DateTime.Now);
            for (int i = 0; i < ActivityData.Count; i++)
            {
                PieSeries series = new PieSeries();
                ChartValues<double> chartValue = new ChartValues<double>
                {
                    Math.Round(ActivityData[i].SpanTime.TotalMinutes, 2)
                };
                series.DataLabels = true;
                series.Title = ActivityData[i].TypeName;
                series.Values = chartValue;
                WeekPieSeriesCollection.Add(series);
            }
        }

        private void GetRecentFiles()
        {
            RelativeFileItems.Clear();
            TaskTomatoService tts = TaskTomatoService.GetTaskTomatoService();
            TaskInfo taskInfo = tts.GetTaskWithID(CurrentTaskId);

            if (taskInfo == null) return;

            var relativeFiles = taskInfo.RelativeFiles;
            foreach (var file in relativeFiles)
            {
                RelativeFileItems.Add(new RelativeFileItem()
                {
                    IconImage = Image.FromFile("./Image/Set.png"),
                    FilePath = file.FilePath
                });
            }
        }

        private void GetWhiteLists()
        {
            ConfigService configService = ConfigService.GetConfigService();
            //WhiteListNames = configService.TTConfig.WhiteLists.Keys.ToList();
        }

        #endregion
    }
}
