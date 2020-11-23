using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UI.AllTaskWindow
{
    public class ImageItem
    {
        public ImageSource ImageSrc { get; set; }
    }

    public class TaskItem
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public String StartTime { get; set; }
        public String DeadLine { get; set; }
        public bool FinishedTomato { get; set; }
        public int TotalTomato { get; set; }
        public List<ImageItem> ImageItems { get; set; }
    }
}
