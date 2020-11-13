using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using DesktopLearningAssistant.TaskTomato.Model;
using DesktopLearningAssistant.TaskTomato;

namespace UI.Tomato
{
    /// <summary>
    /// AllTasksWindow.xaml 的交互逻辑
    /// </summary>
    ///

    public partial class AllTasksWindow : Window
    {

        public AllTasksWindow()
        {
            InitializeComponent();
            List<TaskItem> items = new List<TaskItem>();
            items.Add(new TaskItem()
            {
                ID = 1, Name = "做PPT", State = StateType.Done, StartTime = DateTime.Now.ToString(),
                DeadLine = DateTime.Now.ToString()
            });
            items.Add(new TaskItem()
            {
                ID = 2, Name = "写.NET大作业", State = StateType.NotDone, StartTime = DateTime.Now.ToString(),
                DeadLine = DateTime.Now.ToString()
            });
            items.Add(new TaskItem()
            {
                ID = 3, Name = "背单词", State = StateType.Done, StartTime = DateTime.Now.ToString(),
                DeadLine = DateTime.Now.ToString()
            });
            lvUsers.ItemsSource = items;
            CollectionView view = (CollectionView) CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("State");
            view.GroupDescriptions.Add(groupDescription);
        }
    }

    public enum StateType
    {
        Done,
        NotDone
    };

    public class TaskItem
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public String StartTime { get; set; }
        public String DeadLine { get; set; }
        public int finishedTomato { get; set; }
        public int totalTomato { get; set; }

        public List<Image> TomatImages { get; }
        public StateType State { get; set; }
    }
}
