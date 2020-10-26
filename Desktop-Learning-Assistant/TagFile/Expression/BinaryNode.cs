using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Expression
{
    /// <summary>
    /// 二元运算符节点（And、Or）
    /// </summary>
    public class BinaryNode : AstNode
    {
        private Operator op;
        public Operator Operator
        {
            get => op;
            set
            {
                Debug.Assert(op != Operator.LeftBrace
                          && op != Operator.RightBrace
                          && op != Operator.Not);
                op = value;
            }
        }

        public AstNode LeftChild { get; set; }

        public AstNode RightChild { get; set; }

        public override bool Value(QueryFile file)
        {
            if (Operator == Operator.And)
                return LeftChild.Value(file) && RightChild.Value(file);
            else // Operator == Operator.Or
            {
                Debug.Assert(Operator == Operator.Or);
                return LeftChild.Value(file) || RightChild.Value(file);
            }
        }
    }
}
