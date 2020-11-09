using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.AcAutomaton
{
    /// <summary>
    /// 孩子节点集合，包装了一下 Dictionary
    /// </summary>
    /// <typeparam name="TChar">字符类型</typeparam>
    class BasicTrieChildren<TChar>
        : IEnumerable<KeyValuePair<TChar, BasicTrieNode<TChar>>>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="node">所属的节点</param>
        public BasicTrieChildren(BasicTrieNode<TChar> node) => this.node = node;

        /// <summary>
        /// 是否有某个字符对应的孩子
        /// </summary>
        public bool Has(TChar ch) => children.ContainsKey(ch);

        public BasicTrieNode<TChar> this[TChar ch]
        {
            // 若有对应孩子，则返回孩子节点
            // 若无孩子，则递归跳 fail，返回最长真后缀节点
            get
            {
                if (Has(ch))
                    return children[ch];
                else
                {
                    if (node.IsRoot)
                        return node; // 该节点为根，递归出口
                    else
                        return node.Fail.Children[ch]; // 递归跳 fail
                }
            }
            set => children[ch] = value;
        }

        public IEnumerator<KeyValuePair<TChar, BasicTrieNode<TChar>>> GetEnumerator()
            => children.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// 字符 -> 节点
        /// </summary>
        private readonly Dictionary<TChar, BasicTrieNode<TChar>> children
            = new Dictionary<TChar, BasicTrieNode<TChar>>();

        /// <summary>
        /// 所属节点
        /// </summary>
        private readonly BasicTrieNode<TChar> node;
    }
}
