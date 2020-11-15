using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Model
{
    /// <summary>
    /// 表示标签-文件关系的实体类
    /// </summary>
    public class TagFileRelation
    {
        /// <summary>
        /// 对应标签的名字，外键
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// 对应的 Tag 对象
        /// </summary>
        public virtual Tag Tag { get; set; }

        /// <summary>
        /// 对应文件的 Id，外键
        /// </summary>
        public int FileItemId { get; set; }

        /// <summary>
        /// 对应的 FileItem 对象
        /// </summary>
        public virtual FileItem FileItem { get; set; }

        /// <summary>
        /// 标签被打上的时间（UTC 时间）
        /// </summary>
        public DateTime UtcCreateTime { get; set; }

        /// <summary>
        /// 标签被打上的时间（本地时间）
        /// </summary>
        public DateTime LocalCreateTime { get => UtcCreateTime.ToLocalTime(); }

        public override string ToString()
        {
            return $"Tag Name: {Tag.TagName}, File Item Id: {FileItemId}, Local Create Time: {LocalCreateTime}";
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (TagFileRelation)obj;
            return TagId == other.TagId && FileItemId == other.FileItemId;
        }

        // override object.GetHashCode
        public override int GetHashCode() => TagId ^ FileItemId;
    }
}
