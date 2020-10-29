using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Expression
{
    /// <summary>
    /// 在 parse 的过程中出现的表达式错误
    /// </summary>
    public class ParseException : InvalidExpressionException
    {
        public ParseException() { }
        public ParseException(string message)
            : base(message) { }
    }
}
