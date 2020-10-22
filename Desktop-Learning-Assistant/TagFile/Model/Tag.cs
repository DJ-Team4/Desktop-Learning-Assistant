using System;
using System.Collections.Generic;
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
        public int TagId { get; set; }

        public string TagName { get; set; }

        public ICollection<TagFileRelation> Relations { get; set; } = new List<TagFileRelation>();

        // override object.ToString
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Tag Name: {TagName}, Tag Id: {TagId}\n");
            sb.Append($"Relations of {TagName}:\n");
            foreach (var relation in Relations)
            {
                sb.Append($"File Display Name:{relation.FileItem.DisplayName}, " +
                          $"File Id: {relation.FileItem.FileItemId}, " +
                          $"Create Time: {relation.CreateTime}\n");
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
            if (TagId == other.TagId)
            {
                Debug.Assert(TagName == other.TagName);
                return true;
            }
            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return TagId ^ TagName.GetHashCode();
        }
    }
}
