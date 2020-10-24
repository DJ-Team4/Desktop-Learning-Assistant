using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Model
{
    /// <summary>
    /// 标签实体类
    /// </summary>
    public class Tag
    {
        public string TagName { get; set; }

        public virtual ICollection<TagFileRelation> Relations { get; private set; }
            = new ObservableCollection<TagFileRelation>();

        // override object.ToString
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Tag Name: {TagName}\n");
            sb.Append($"Relations of {TagName}:\n");
            foreach (var relation in Relations)
            {
                sb.Append($"File Display Name:{relation.FileItem.DisplayName}, " +
                          $"File Id: {relation.FileItem.FileItemId}, " +
                          $"Local Create Time: {relation.LocalCreateTime}\n");
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
            Tag other = (Tag)obj;
            return TagName == other.TagName;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return TagName.GetHashCode();
        }
    }
}
