using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.AcAutomaton
{
    /// <summary>
    /// AC 自动机泛型类
    /// </summary>
    /// <typeparam name="TChar">字符类型</typeparam>
    class BasicAcAutomaton<TChar>
    {
        /// <summary>
        /// 插入模式串，返回代表该模式串的节点。
        /// 必须在 Build 之前插入。
        /// </summary>
        /// <param name="pattern">模式串</param>
        /// <returns>代表该模式串的节点</returns>
        public BasicTrieNode<TChar> Insert(IEnumerable<TChar> pattern)

        {
            if (isBuild)
                throw new ApplicationException("Trie has been build, cannot insert.");
            var p = root;
            foreach (TChar ch in pattern)
            {
                if (!p.Children.Has(ch))
                    p.Children[ch] = new BasicTrieNode<TChar>(root, ch);
                p = p.Children[ch];
            }
            p.IsPattern = true;
            return p;
        }

        /// <summary>
        /// 建造
        /// </summary>
        public void Build()
        {
            if (isBuild)
                throw new ApplicationException("Build again.");
            var q = new Queue<BasicTrieNode<TChar>>();
            foreach (var kv in root.Children)
                q.Enqueue(kv.Value);
            while (q.Count != 0)
            {
                var u = q.Dequeue();
                foreach (var kv in u.Children)
                {
                    TChar ch = kv.Key;
                    var child = kv.Value;
                    child.Fail = u.Fail.Children[ch];
                    q.Enqueue(child);
                }
            }
            TopoSort();
            isBuild = true;
        }

        /// <summary>
        /// 求解每个模式串在给定文本串中的出现次数。
        /// 该函数执行结束后查看相应节点的 Ans 即可获取结果。
        /// </summary>
        /// <param name="text">文本串</param>
        public void Query(IEnumerable<TChar> text)
        {
            if (!isBuild)
                Build();
            ClearAns();
            var p = root;
            foreach (TChar ch in text)
            {
                p = p.Children[ch];
                // 若孩子存在，则 p 跳到孩子节点
                // 若孩子不存在，则 p 递归跳 fail
                // 最终，指针 p 都是跳到当前文本段的最长真后缀
                p.Ans += 1;
            }
            // 按拓扑序累加 Ans
            foreach (var u in topo)
            {
                var v = u.Fail;
                if (!v.IsRoot)
                    v.Ans += u.Ans;
            }
        }

        /// <summary>
        /// 将 Trie 中所有节点的 Ans 置零
        /// </summary>
        public void ClearAns() => PreOrderTraverse(u => u.Ans = 0);

        /// <summary>
        /// 求解节点按 fail 指针的拓扑序
        /// </summary>
        private void TopoSort()
        {
            // 遍历 Trie 中的所有节点
            // 若未访问过，则调用拓扑排序的 DFS 函数
            PreOrderTraverse(u =>
            {
                if (u.TopoVis == 0)
                    TopoDfs(u);
            });
            topo.Reverse();
        }

        /// <summary>
        /// 拓扑排序的 DFS 函数
        /// </summary>
        private void TopoDfs(BasicTrieNode<TChar> u)
        {
            u.TopoVis = -1;
            var v = u.Fail; // fail 出边指向的点
            if (!v.IsRoot)
            {
                Debug.Assert(v.TopoVis != -1);
                if (v.TopoVis == 0)
                    TopoDfs(v);
            }
            u.TopoVis = 1;
            topo.Add(u);
        }

        /// <summary>
        /// 先序遍历以当前节点为根的子树
        /// </summary>
        /// <param name="u">当前节点</param>
        /// <param name="action">要执行的操作</param>
        private void PreOrderTraverseDfs(BasicTrieNode<TChar> u,
                                         Action<BasicTrieNode<TChar>> action)
        {
            action?.Invoke(u);
            foreach (var kv in u.Children)
                PreOrderTraverseDfs(kv.Value, action);
        }

        /// <summary>
        /// 先序遍历整棵 Trie 树
        /// </summary>
        /// <param name="action">要执行的操作</param>
        private void PreOrderTraverse(Action<BasicTrieNode<TChar>> action)
            => PreOrderTraverseDfs(root, action);

        /// <summary>
        /// 不使用拓扑排序优化的查询，速度更慢
        /// </summary>
        /// <param name="text">文本串</param>
        private void QueryWithoutTopoSort(IEnumerable<TChar> text)
        {
            if (!isBuild)
                Build();
            ClearAns();
            var p = root;
            foreach (TChar ch in text)
            {
                p = p.Children[ch];
                for (var j = p; !j.IsRoot; j = j.Fail)
                {
                    j.Ans += 1;
                }
            }
        }

        /// <summary>
        /// Trie 的根
        /// </summary>
        private readonly BasicTrieNode<TChar> root = new BasicTrieNode<TChar>();

        /// <summary>
        /// 节点按 fail 指针的拓扑序
        /// </summary>
        private readonly List<BasicTrieNode<TChar>> topo = new List<BasicTrieNode<TChar>>();

        private bool isBuild = false;
    }
}
