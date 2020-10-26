using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile
{
    //与非法表达式有关的异常

    public class InvalidExpressionException : ApplicationException
    {
        public InvalidExpressionException() { }
        public InvalidExpressionException(string message)
            : base(message) { }
    }

    public class TokenizeException : InvalidExpressionException
    {
        public TokenizeException() { }
        public TokenizeException(string message)
            : base(message) { }
    }

    public class ParseException : InvalidExpressionException
    {
        public ParseException() { }
        public ParseException(string message)
            : base(message) { }
    }
}
