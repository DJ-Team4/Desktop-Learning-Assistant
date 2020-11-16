using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Model
{
    /// <summary>
    /// 文件实体类
    /// </summary>
    public class FileItem
    {
        /// <summary>
        /// 文件 Id，主键
        /// </summary>
        public int FileItemId { get; set; }

        /// <summary>
        /// 显示的文件名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 实际的文件名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 含有该文件的 TagFileRelation 的集合。
        /// 可通过该属性获取该文件的所有标签
        /// </summary>
        public virtual ICollection<TagFileRelation> Relations { get; private set; }
            = new List<TagFileRelation>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"File Display Name: {DisplayName}, File Real Name: {RealName}, File Id: {FileItemId}\n");
            sb.Append($"Relations of {DisplayName}:\n");
            foreach (var relation in Relations)
            {
                sb.Append($"Tag Name: {relation.Tag.TagName}, " +
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
            var other = (FileItem)obj;
            if (FileItemId == other.FileItemId)
            {
                Debug.Assert(DisplayName == other.DisplayName && RealName == other.RealName);
                return true;
            }
            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return FileItemId ^ DisplayName.GetHashCode() ^ RealName.GetHashCode();
        }
    }
}
