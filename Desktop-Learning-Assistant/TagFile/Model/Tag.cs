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
        /// <summary>
        /// 标签 Id，主键
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// 标签名字，必须唯一
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// 含有该标签的 TagFileRelation 的集合。
        /// 可通过该属性获取所有含该标签的文件。
        /// </summary>
        public virtual ICollection<TagFileRelation> Relations { get; private set; }
            = new List<TagFileRelation>();

        // override object.ToString
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Tag Name: {TagName}\n");
            sb.Append($"Relations of {TagName}:\n");
            foreach (var relation in Relations)
            {
                sb.Append($"File Display Name:{relation.FileItem.DisplayName}, " +
                          $"File Real Name: {relation.FileItem.RealName}, " +
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
            return TagId == other.TagId;
        }

        // override object.GetHashCode
        public override int GetHashCode() => TagId;
    }
}
