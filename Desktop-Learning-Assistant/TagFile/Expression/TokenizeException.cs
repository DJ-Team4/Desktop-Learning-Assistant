using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Expression
{
    /// <summary>
    /// 在 tokenize 过程中出现的表达式错误
    /// </summary>
    public class TokenizeException : InvalidExpressionException
    {
        public TokenizeException() { }
        public TokenizeException(string message)
            : base(message) { }
    }
}
