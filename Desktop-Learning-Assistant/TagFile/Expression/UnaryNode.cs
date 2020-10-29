using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Expression
{
    /// <summary>
    /// 一元运算符节点（Not）
    /// </summary>
    public class UnaryNode : AstNode
    {
        public AstNode Child { get; set; }

        public override bool Value(QueryFile file)
        {
            return !Child.Value(file);
        }
    }
}
