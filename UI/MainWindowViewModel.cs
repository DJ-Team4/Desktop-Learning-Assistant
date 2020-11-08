using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;

namespace UI
{
    class MainWindowViewModel
    {
        SeriesCollection colunmSeriesCollection = new SeriesCollection();
        SeriesCollection pieSeriesCollection = new SeriesCollection();
        List<string> _columnXLabels = new List<string>();
        public MainWindowViewModel()
        {
            GetColunmSeriesData();
            GetPieSeriesData();
        }
        #region 属性
        /// <summary>
        /// 柱状图集合
        /// </summary>
        public SeriesCollection ColunmSeriesCollection
        {
            get
            {
                return colunmSeriesCollection;
            }

            set
            {
                colunmSeriesCollection = value;
            }
        }

        /// <summary>
        /// 饼图图集合
        /// </summary>
        public SeriesCollection PieSeriesCollection
        {
            get
            {
                return pieSeriesCollection;
            }

            set
            {
                pieSeriesCollection = value;
            }
        }

        /// <summary>
        /// 柱状图X坐标
        /// </summary>
        public List<string> ColumnXLabels
        {
            get
            {
                return _columnXLabels;
            }

            set
            {
                _columnXLabels = value;
            }
        }
        #endregion

        #region 方法
        void GetColunmSeriesData()
        {
            List<string> titles = new List<string> { "Edge", "Chrome", "Firefox", "Other" };
            List<double> columnValues = new List<double> { 10, 70, 15, 5 };

            for (int i = 0; i < titles.Count; i++)
            {
                ColumnXLabels.Add(titles[i]);
            }
            ColumnSeries colunmseries = new ColumnSeries();
            colunmseries.DataLabels = true;
            colunmseries.Title = "浏览器份额";
            colunmseries.Values = new ChartValues<double>(columnValues);
            ColunmSeriesCollection.Add(colunmseries);

        }

        void GetPieSeriesData()
        {
            List<string> titles = new List<string> { "C#", "Java", "Python" };
            List<double> pieValues = new List<double> { 60, 30, 10 };
            ChartValues<double> chartvalue = new ChartValues<double>();
            for (int i = 0; i < titles.Count; i++)
            {
                chartvalue = new ChartValues<double>();
                chartvalue.Add(pieValues[i]);
                PieSeries series = new PieSeries();
                series.DataLabels = true;
                series.Title = titles[i];
                series.Values = chartvalue;
                PieSeriesCollection.Add(series);
            }
        }
        #endregion
    }
}
