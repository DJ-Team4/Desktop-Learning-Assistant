using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 用于限制总字数的可迭代对象
    /// </summary>
    class LengthLimitEnumerable : IEnumerable<string>
    {
        /// <summary>
        /// 用于限制总字数的可迭代对象
        /// </summary>
        /// <param name="lines">字符串来源</param>
        /// <param name="lengthLimit">总字数限制，负数表示不限制</param>
        public LengthLimitEnumerable(IEnumerable<string> lines, int lengthLimit)
        {
            this.lines = lines;
            this.lengthLimit = lengthLimit;
        }

        public IEnumerator<string> GetEnumerator()
        {
            int total = 0;
            foreach (string theLine in lines)
            {
                string line = theLine.Trim();
                if (line.Length == 0)
                    continue;
                if (lengthLimit < 0 || total + line.Length <= lengthLimit)
                {
                    total += line.Length;
                    yield return line;
                }
                else
                {
                    int tLen = lengthLimit - total;
                    yield return line.Substring(0, tLen);
                    break;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();

        private readonly int lengthLimit;

        private readonly IEnumerable<string> lines;
    }
}
