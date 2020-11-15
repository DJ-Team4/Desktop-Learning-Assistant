using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TagFile.Model;

namespace DesktopLearningAssistant.TagFile.Expression
{
    /// <summary>
    /// 表达式查询辅助类
    /// </summary>
    public static class TagExpression
    {
        /// <summary>
        /// only for test
        /// </summary>
        public static void TokenizeTest(string expr)
        {
            var tokens = Tokenize(expr);
            tokens.ForEach(t => Console.Write(t));
            Console.WriteLine();
        }

        #region Query

        /// <summary>
        /// 将 TagFileRelation 集合转换为 QueryFile 列表
        /// </summary>
        private static List<QueryFile> ToFiles(IEnumerable<TagFileRelation> relations)
        {
            //dict[id] = tagSet
            var dict = new Dictionary<int, HashSet<string>>();
            foreach (var relation in relations)
            {
                int fileId = relation.FileItemId;
                string tagName = relation.Tag.TagName;
                if (!dict.ContainsKey(fileId))
                    dict[fileId] = new HashSet<string>();
                dict[fileId].Add(tagName);
            }
            var files = new List<QueryFile>();
            foreach (var kv in dict)
            {
                files.Add(new QueryFile
                {
                    Id = kv.Key,
                    Tags = kv.Value
                });
            }
            return files;
        }

        /// <summary>
        /// 使用表达式查询所有符合条件的文件的 Id
        /// </summary>
        /// <param name="relations">关系列表</param>
        /// <param name="expression">表达式</param>
        /// <returns>文件 Id 的列表</returns>
        /// <exception cref="TokenizeException">词法分析时发现错误</exception>
        /// <exception cref="ParseException">语法分析时发现错误</exception>
        public static ICollection<int> Query(IEnumerable<TagFileRelation> relations, string expression)
        {
            var files = ToFiles(relations);
            var root = Parse(Tokenize(expression)); // AST 的根节点
            var idList = new List<int>(); // 符合查询结果的 Id 的列表
            foreach (var file in files)
            {
                if (root.Value(file))
                    idList.Add(file.Id);
            }
            return idList;
        }

        #endregion

        #region Tokenizer

        /// <summary>
        /// 获取表达式中所有 Tag Name 的范围。
        /// 返回 dict[start] = stop，区间 [start, stop)。
        /// </summary>
        private static Dictionary<int, int> NamePosDict(string expression)
        {
            var dict = new Dictionary<int, int>();
            bool inName = false;
            int start = -1;
            for (int i = 0; i < expression.Length; i++)
            {
                char ch = expression[i];
                if (ch == '"')
                {
                    if (!inName)
                    {
                        inName = true;
                        start = i;
                    }
                    else
                    {
                        if (expression[i - 1] != '\\'
                            || (expression[i - 1] == '\\' && expression[i - 2] == '\\'))
                        {
                            Debug.Assert(start != -1);
                            dict[start] = i + 1;
                            inName = false;
                        }
                    }
                }
            }
            if (inName)
                throw new TokenizeException("Double quotes are not match.");
            return dict;
        }

        /// <summary>
        /// 获取一个 Tag 名字 Token。
        /// 区间 [start, stop)
        /// </summary>
        private static Token GetNameToken(int start, int stop, string expression)
        {
            Debug.Assert(stop - start >= 2);
            Debug.Assert(expression[start] == '"' && expression[stop - 1] == '"');
            var sb = new StringBuilder();
            bool afterSlash = false;
            for (int i = start + 1; i < stop - 1; i++)
            {
                char ch = expression[i];
                if (!afterSlash)
                {
                    if (ch == '\\')
                        afterSlash = true;
                    else
                        sb.Append(ch);
                }
                else
                {
                    if (ch == '"' || ch == '\\')
                        sb.Append(ch);
                    else
                        throw new TokenizeException("Invalid character after slash.");
                    afterSlash = false;
                }
            }
            return new Token(sb.ToString());
        }

        /// <summary>
        /// 检查下标 start 开始是否为“与”运算符
        /// </summary>
        private static bool IsAnd(string expression, int start)
        {
            return start + 3 - 1 < expression.Length
                && (expression.Substring(start, 3) == "and"
                    || expression.Substring(start, 3) == "AND");
        }

        /// <summary>
        /// 检查下标 start 开始是否为“或”运算符
        /// </summary>
        private static bool IsOr(string expression, int start)
        {
            return start + 2 - 1 < expression.Length
                && (expression.Substring(start, 2) == "or"
                    || expression.Substring(start, 2) == "OR");
        }

        /// <summary>
        /// 检查下标 start 开始是否为“非”运算符
        /// </summary>
        private static bool IsNot(string expression, int start)
        {
            return start + 3 - 1 < expression.Length
                && (expression.Substring(start, 3) == "not"
                    || expression.Substring(start, 3) == "NOT");
        }

        /// <summary>
        /// 将表达式字符串转换为 Token 序列
        /// </summary>
        private static List<Token> Tokenize(string expression)
        {
            var tokens = new List<Token>();
            var namePos = NamePosDict(expression);
            int i = 0;
            while (i < expression.Length)
            {
                if (namePos.ContainsKey(i))
                {
                    tokens.Add(GetNameToken(i, namePos[i], expression));
                    i = namePos[i];
                }
                else // not in name
                {
                    if (char.IsWhiteSpace(expression[i]))
                        i += 1;
                    else if (expression[i] == '(')
                    {
                        tokens.Add(new Token(Operator.LeftBrace));
                        i += 1;
                    }
                    else if (expression[i] == ')')
                    {
                        tokens.Add(new Token(Operator.RightBrace));
                        i += 1;
                    }
                    else
                    {
                        if (IsAnd(expression, i))
                        {
                            tokens.Add(new Token(Operator.And));
                            i += 3;
                        }
                        else if (IsOr(expression, i))
                        {
                            tokens.Add(new Token(Operator.Or));
                            i += 2;
                        }
                        else if (IsNot(expression, i))
                        {
                            tokens.Add(new Token(Operator.Not));
                            i += 3;
                        }
                        else
                            throw new TokenizeException("Invalid content out of name.");
                    }
                }
            }
            return tokens;
        }

        #endregion

        #region Parser

        /// <summary>
        /// 将 Token 序列转换为 AST
        /// </summary>
        /// <returns>AST 的根节点，若 Token 序列为空则返回 null</returns>
        private static AstNode Parse(List<Token> tokens)
        {
            if (tokens.Count == 0)
                return null;
            return RecursiveParse(tokens, 0, tokens.Count);
        }

        /// <summary>
        /// 递归建立 AST。
        /// 区间 [start, stop)
        /// </summary>
        private static AstNode RecursiveParse(List<Token> tokens, int start, int stop)
        {
            if (start >= stop)
                throw new ParseException("Empty interval.");
            if (stop - start == 1) // only one token, should be name token
            {
                if (!tokens[start].IsName)
                    throw new ParseException();
                return new LeafNode { TagName = tokens[start].Name };
            }

            int p = 0;   // depth of brace
            int c1 = -1; // right most [And]
            int c2 = -1; // right most [Or]
            int c3 = -1; // left most [Not]
            for (int i = start; i < stop; i++)
            {
                var token = tokens[i];
                if (token.OperatorTest(Operator.LeftBrace))
                    p++;
                else if (token.OperatorTest(Operator.RightBrace))
                {
                    p--;
                    if (p < 0)
                        throw new ParseException(
                            "Braces aren't balanced.");
                }
                else if (p == 0)
                {
                    if (token.OperatorTest(Operator.Or))
                        c1 = i;
                    else if (token.OperatorTest(Operator.And))
                        c2 = i;
                    else if (token.OperatorTest(Operator.Not) && c3 == -1)
                        c3 = i;
                }
            }

            if (p != 0)
                throw new ParseException("Braces aren't balanced.");

            if (c1 == -1 && c2 == -1 && c3 == -1)
            {
                if (!(tokens[start].OperatorTest(Operator.LeftBrace)
                     && tokens[stop - 1].OperatorTest(Operator.RightBrace)))
                    throw new ParseException();
                // whole expr wrapped by brace
                return RecursiveParse(tokens, start + 1, stop - 1);
            }
            if (c1 != -1)
            {
                return new BinaryNode
                {
                    Operator = tokens[c1].Operator,
                    LeftChild = RecursiveParse(tokens, start, c1),
                    RightChild = RecursiveParse(tokens, c1 + 1, stop)
                };
            }
            else if (c2 != -1)
            {
                return new BinaryNode
                {
                    Operator = tokens[c2].Operator,
                    LeftChild = RecursiveParse(tokens, start, c2),
                    RightChild = RecursiveParse(tokens, c2 + 1, stop)
                };
            }
            else if (c3 != -1)
            {
                if (c3 != start)
                    throw new ParseException();
                return new UnaryNode { Child = RecursiveParse(tokens, c3 + 1, stop) };
            }
            else
                throw new ParseException();
        }

        #endregion

    }
}
