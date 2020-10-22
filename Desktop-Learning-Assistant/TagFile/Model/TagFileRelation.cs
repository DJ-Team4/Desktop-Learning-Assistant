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
        public int TagId { get; set; }
        public Tag Tag { get; set; }

        public int FileItemId { get; set; }
        public FileItem FileItem { get; set; }

        public DateTime CreateTime { get; set; }

        public override string ToString()
        {
            return $"Tag Name: {Tag.TagName}, File Item Id: {FileItemId}, Create Time: {CreateTime}";
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
        public override int GetHashCode()
        {
            return TagId ^ FileItemId;
        }
    }
}
