using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Expression
{
    public class Token
    {
        /// <summary>
        /// 创建一个 Tag 名 Token
        /// </summary>
        /// <param name="name">Tag 名字</param>
        public Token(string name)
        {
            IsName = true;
            Name = name;
        }

        /// <summary>
        /// 创建一个运算符 Token
        /// </summary>
        /// <param name="op">运算符</param>
        public Token(Operator op)
        {
            IsName = false;
            Operator = op;
        }

        public bool IsName { get; private set; }

        public bool IsOperator { get => !IsName; }

        private string name;
        public string Name
        {
            get
            {
                Debug.Assert(IsName,
                    "Try to get a name from a non-name token.");
                return name;
            }
            private set => name = value;
        }

        private Operator op;
        public Operator Operator
        {
            get
            {
                Debug.Assert(IsOperator,
                    "Try to get an operator from a non-operator token.");
                return op;
            }
            private set => op = value;
        }

        /// <summary>
        /// 测试 Token 是否为某个运算符
        /// </summary>
        public bool OperatorTest(Operator op) => IsOperator && Operator == op;

        public override string ToString()
        {
            if (IsName)
                return $"\"{Name}\"";
            else
            {
                if (Operator == Operator.LeftBrace)
                    return "(";
                else if (Operator == Operator.RightBrace)
                    return ")";
                else
                    return Operator.ToString();
            }
        }
    }
}
