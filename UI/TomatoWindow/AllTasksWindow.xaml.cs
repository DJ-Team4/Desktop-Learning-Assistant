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


            // AllTasksListView.Items.Add(new  )
        }
     
    }


    class AllTaskShow
    {
        public int TaskID { set; get; }
        public string Name { set; get; }
        public DateTime StartTime { set; get; }
        public DateTime EndTime { set; get; }
        public List<DesktopLearningAssistant.TomatoClock.Model.Tomato> TomaList { set; get; }
        public List<DesktopLearningAssistant.TomatoClock.Model.Tomato> present_tomato { get; set; }
        public int TaskState { get; set; }
        public string Notes { get; set; }



        public AllTaskShow(int taskId, string name, DateTime startTime, DateTime EndTime,
            List<DesktopLearningAssistant.TomatoClock.Model.Tomato> tomaList,
            List<DesktopLearningAssistant.TomatoClock.Model.Tomato> present_tomato, int TaskState, string notes        )
        {
            this.TaskID = taskId;
            this.Name = name;
            this.StartTime = startTime;
            this.EndTime = EndTime;
            this.TomaList = tomaList;
            this.present_tomato = present_tomato;
            this.TaskState = TaskState;
            this.Notes = notes;

        }


    }
}
