using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction
{
    /// <summary>
    /// string 的一些扩展方法
    /// </summary>
    static class StringExtension
    {
        /// <summary>
        /// 获取 Unicode 代码点可迭代对象
        /// </summary>
        public static StringUnicodePoints CodePoints(this string s)
            => new StringUnicodePoints(s);

        /// <summary>
        /// 最后一个字符，保证时间复杂度 O(1)
        /// </summary>
        public static char LastChar(this string s) => s[s.Length - 1];
    }
}
