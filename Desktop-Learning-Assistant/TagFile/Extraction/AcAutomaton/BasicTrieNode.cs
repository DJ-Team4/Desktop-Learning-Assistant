using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.AcAutomaton
{
    /// <summary>
    /// Trie 节点
    /// </summary>
    /// <typeparam name="TChar">字符类型</typeparam>
    class BasicTrieNode<TChar>
    {
        /// <summary>
        /// 用于创建根节点的构造函数
        /// </summary>
        public BasicTrieNode()
        {
            root = this;
            Children = new BasicTrieChildren<TChar>(this);
            Fail = root;
            CharOfNode = default;
        }

        /// <summary>
        /// 用于创建非根节点的构造函数
        /// </summary>
        /// <param name="root">Trie 的根</param>
        public BasicTrieNode(BasicTrieNode<TChar> root, TChar charOfNode)
        {
            this.root = root;
            Children = new BasicTrieChildren<TChar>(this);
            Fail = root;
            CharOfNode = charOfNode;
        }

        /// <summary>
        /// 孩子集合
        /// </summary>
        public BasicTrieChildren<TChar> Children { get; private set; }

        /// <summary>
        /// fail 指针。
        /// 根节点的 fail 指针指向自己。
        /// </summary>
        public BasicTrieNode<TChar> Fail { get; set; }

        /// <summary>
        /// 该节点是否是模式串
        /// </summary>
        public bool IsPattern { get; set; } = false;

        /// <summary>
        /// 该点代表的字符串的计数
        /// </summary>
        public int Ans { get; set; } = 0;

        /// <summary>
        /// 该节点是否为根节点
        /// </summary>
        public bool IsRoot { get => this == root; }

        /// <summary>
        /// 拓扑排序中的访问标记
        /// </summary>
        public short TopoVis { get; set; } = 0;

        public TChar CharOfNode { get; set; }

        public override string ToString()
        {
            return $"char: {CharOfNode}, Ans: {Ans}, IsPattern: {IsPattern}, IsRoot: {IsRoot}";
        }

        /// <summary>
        /// 整颗 Trie 的根
        /// </summary>
        private readonly BasicTrieNode<TChar> root;
    }
}
