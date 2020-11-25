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
using System.Windows.Media.Imaging;
using System.IO;

namespace UI
{
    public class MainWindowViewModel
    {
        private TimeStatisticService tss = new TimeStatisticService();
        private TaskTomatoService tts = new TaskTomatoService();

        public TaskInfo currentTaskInfo;

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

        public List<RelativeFileItem> RelativeFileItems { get; set; }

        /// <summary>
        /// 窗口起始位置
        /// </summary>
        public double Left { get; set; } = SystemParameters.WorkArea.Width;

        /// <summary>
        /// 白名单列表
        /// </summary>
        public List<string> WhiteListKeys { get; set; }

        #endregion

        #region 公有方法

        public MainWindowViewModel()
        {
            tss = TimeStatisticService.GetTimeStatisticService();
            tts = TaskTomatoService.GetTaskTomatoService();

            LineSeriesCollection = new SeriesCollection();
            LineXLabels = new List<string>();

            TodayColumnSeriesCollection = new SeriesCollection();
            WeekColumnSeriesCollection = new SeriesCollection();

            TodayPieSeriesCollection = new SeriesCollection();
            WeekPieSeriesCollection = new SeriesCollection();

            TodayColumnXLabels = new List<string>();
            WeekColumnXLabels = new List<string>();

            RelativeFileItems = new List<RelativeFileItem>();
            WhiteListKeys = new List<string>();

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
            GetLineSeriesData();
            UpdateWhiteKeys();
        }

        public void UpdateRelativeFiles()
        {
            RelativeFileItems.Clear();
            if (currentTaskInfo == null) return;

            var relativeFiles = currentTaskInfo.RelativeFiles;
            foreach (var file in relativeFiles)
            {
                if (!File.Exists(file.FilePath)) continue;      // 排除已删除文件
                if (!file.FilePath.Contains(".")) continue;     // 排除文件夹

                RelativeFileItem relativeFileItem = new RelativeFileItem()
                {
                    FilePath = file.FilePath
                };
                if (!RelativeFileItems.Contains(relativeFileItem)) RelativeFileItems.Add(relativeFileItem);     // 去重
            }
        }

        public void UpdateWhiteKeys()
        {
            ConfigService configService = ConfigService.GetConfigService();
            WhiteListKeys = configService.TTConfig.WhiteLists.Keys.ToList();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 最近k个任务的效率折线图
        /// </summary>
        private void GetLineSeriesData()
        {
            LineSeriesCollection.Clear();
            LineXLabels.Clear();
            List<double> values = new List<double>();
            List<TaskEfficiency> taskEfficiencies = tts.GetTaskEfficiencies(DateTime.Now, 5);
            for (int i = 0; i < taskEfficiencies.Count && i < 8; i++)
            {
                LineXLabels.Add(taskEfficiencies[i].Name);
                values.Add(Math.Round(taskEfficiencies[i].Efficiency, 2));
            }
            
            LineSeriesCollection.Add(new LineSeries
            {
                Title = "Efficiency",
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
            List<UserActivity> userActivities = tss.GetUserActivitiesWithin(DateTime.Today, DateTime.Now);
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
            List<UserActivity> userActivities = tss.GetUserActivitiesWithin(beginTime, DateTime.Now);
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

            List<TypeActivity> ActivityData = tss.GetTypeActivitiesWithin(DateTime.Today, DateTime.Now);
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

            List<TypeActivity> ActivityData = tss.GetTypeActivitiesWithin(beginTime, DateTime.Now);
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

        #endregion
    }
}
