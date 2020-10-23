using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Model
{
    /// <summary>
    /// 表示标签-文件关联的实体类
    /// </summary>
    public class TagFileRelation
    {
        public string TagName { get; set; }
        public Tag Tag { get; set; }

        public int FileItemId { get; set; }
        public FileItem FileItem { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LocalCreateTime { get => CreateTime.ToLocalTime(); }

        public override string ToString()
        {
            return $"Tag Name: {TagName}, File Item Id: {FileItemId}, Create Time: {CreateTime}";
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (TagFileRelation)obj;
            return TagName == other.TagName && FileItemId == other.FileItemId;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return TagName.GetHashCode() ^ FileItemId;
        }
    }
}
