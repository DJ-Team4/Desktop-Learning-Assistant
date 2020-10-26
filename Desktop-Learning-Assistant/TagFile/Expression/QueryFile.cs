using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Expression
{
    public class QueryFile
    {
        public int Id { get; set; }
        public HashSet<string> Tags { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Id: {Id}, Tags: ");
            bool isfirst = true;
            foreach (string tag in Tags)
            {
                if (isfirst)
                    isfirst = false;
                else
                    sb.Append(", ");
                sb.Append(tag);
            }
            return sb.ToString();
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Id == ((QueryFile)obj).Id;
        }

        // override object.GetHashCode
        public override int GetHashCode() => Id;
    }
}
