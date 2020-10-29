using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Expression
{
    /// <summary>
    /// 叶子节点（Tag 名节点）
    /// </summary>
    class LeafNode : AstNode
    {
        public string TagName { get; set; }

        public override bool Value(QueryFile file)
        {
            return file.Tags.Contains(TagName);
        }
    }
}
