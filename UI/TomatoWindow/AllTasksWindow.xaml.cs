using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DesktopLearningAssistant.TomatoClock.Model;

namespace UI.Tomato
{
    /// <summary>
    /// AllTasksWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AllTasksWindow : Window
    {
        public AllTasksWindow()
        {
            InitializeComponent();
            TaskInfo _task = new TaskInfo();


            AllTasksListView.Items.Add(new AllTaskShow(1, "六级试卷", "2016-09-27 01:02:03", "2016-09-28 01:02:03",
                4, 1, false, "19年12月试卷"));
        }
     
    }


    class AllTaskShow
    {
        public int TaskID { set; get; }
        public string Name { set; get; }
        public string StartTime { set; get; }
        public string EndTime { set; get; }
        public int todo_numebr { get; set; }
        public bool TaskState { get; set; }
        public int completion_number { get; set; }
        public string Notes { get; set; }



        public AllTaskShow(int taskId, string name, string startTime, string endTime,
            int todoNumebr,
            int completion_number, bool TaskState, string notes )
        {
            this.TaskID = taskId;
            this.Name = name;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.todo_numebr = todoNumebr;
            this.completion_number = completion_number;
            this.TaskState = TaskState;
            this.Notes = notes;

        }


    }
}
