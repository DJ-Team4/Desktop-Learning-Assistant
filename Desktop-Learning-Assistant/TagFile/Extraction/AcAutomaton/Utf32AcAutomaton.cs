using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.AcAutomaton
{
    /// <summary>
    /// 支持 Unicode 字符的 AC 自动机（内部表示为 UTF-32）。
    /// <para/>
    /// 包装了一下 BasicAcAutomaton。
    /// </summary>
    class Utf32AcAutomaton
    {
        /// <summary>
        /// 构造函数，建立 AC 自动机。
        /// </summary>
        /// <param name="patterns">模式串集合</param>
        public Utf32AcAutomaton(IEnumerable<string> patterns)
        {
            foreach (string pattern in patterns)
            {
                var node = ac.Insert(pattern.CodePoints());
                NodeInTrie[pattern] = node;
            }
            ac.Build();
        }

        /// <summary>
        /// 查询所有模式串在文本串中的出现次数。
        /// </summary>
        /// <param name="text">文本串</param>
        /// <returns>表示“模式串 -> 出现次数”的字典</returns>
        public Dictionary<string, int> Query(string text)
        {
            var patternAns = new Dictionary<string, int>();
            ac.Query(text.CodePoints());
            foreach (var kv in NodeInTrie)
            {
                string pattern = kv.Key;
                int ans = kv.Value.Ans;
                patternAns[pattern] = ans;
            }
            return patternAns;
        }

        /// <summary>
        /// AC 自动机
        /// </summary>
        private readonly BasicAcAutomaton<int> ac = new BasicAcAutomaton<int>();

        /// <summary>
        /// 模式串 -> Trie 中的节点
        /// </summary>
        private readonly Dictionary<string, BasicTrieNode<int>> NodeInTrie
            = new Dictionary<string, BasicTrieNode<int>>();
    }
}
