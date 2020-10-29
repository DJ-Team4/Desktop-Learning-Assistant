using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Expression
{
    /// <summary>
    /// 用于表达式查询的 File 实体类
    /// </summary>
    public class QueryFile
    {
        /// <summary>
        /// 文件 Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 该文件所含的 Tag
        /// </summary>
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
