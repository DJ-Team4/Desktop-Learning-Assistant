using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction
{
    /// <summary>
    /// Unicode 代码点可迭代对象。
    /// 迭代此对象可获取每个 Code Point。
    /// <para/>
    /// 由于 string 的内部表示为 UTF-16，
    /// 因此迭代 string 只能获取每个 Code Unit。
    /// </summary>
    class StringUnicodePoints : IEnumerable<int>
    {
        public StringUnicodePoints(string s) => this.s = s;

        public IEnumerator<int> GetEnumerator()
        {
            for (int i = 0; i < s.Length; i += char.IsSurrogatePair(s, i) ? 2 : 1)
                yield return char.ConvertToUtf32(s, i);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();

        private readonly string s;
    }
}
