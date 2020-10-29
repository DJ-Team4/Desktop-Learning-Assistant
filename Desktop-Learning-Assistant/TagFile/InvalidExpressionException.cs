using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile
{
    /// <summary>
    /// 查询表达式非法
    /// </summary>
    public class InvalidExpressionException : ApplicationException
    {
        public InvalidExpressionException() { }
        public InvalidExpressionException(string message)
            : base(message) { }
    }
}
