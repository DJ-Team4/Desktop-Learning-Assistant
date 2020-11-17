using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UI.FileWindow
{
    class IntelliUtils
    {
        /// <summary>
        /// 双引号是否为偶数
        /// </summary>
        public static bool IsQuoteOdd(string text)
        {
            int quoteCnt = 0;
            foreach (char ch in text)
                if (ch == '"')
                    quoteCnt++;
            return quoteCnt % 2 != 0;
        }

        /// <summary>
        /// 提取最后一个双引号后面的内容
        /// </summary>
        public static string ExtractPrefix(string text)
        {
            var reg = new Regex(@"""([^""]*)$");
            Match match = reg.Match(text);
            return match.Groups[1].Value;
        }
    }
}
