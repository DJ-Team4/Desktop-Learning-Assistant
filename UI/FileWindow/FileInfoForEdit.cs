using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.FileWindow
{
    public class FileInfoForEdit
    {
        public string Filename { get; set; }

        public string CreateAt { get; set; }

        public string AccessAt { get; set; }

        public HashSet<string> TagNames { get; }
            = new HashSet<string>();
    }
}
