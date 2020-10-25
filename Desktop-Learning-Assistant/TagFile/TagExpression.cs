using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile
{
    public static class TagExpression
    {
        public static void Test(string expr)
        {
            var tokens = Tokenize(expr);
            tokens.ForEach(t => Console.Write(t));
            Console.WriteLine();
        }

        private class File
        {
            public int Id { get; private set; }
            public HashSet<string> Tags { get; private set; }
        }

        private class Token
        {
            public Token(string name)
            {
                IsName = true;
                Name = name;
            }

            public Token(Operator op)
            {
                IsName = false;
                Operator = op;
            }

            public bool IsName { get; private set; }

            public bool IsOperator { get => !IsName; }

            private string name;
            public string Name
            {
                get => IsName ? name : throw new ApplicationException(
                    "Try to get a name from a non-name token.");
                private set => name = value;
            }

            private Operator op;
            public Operator Operator
            {
                get => IsOperator ? op : throw new ApplicationException(
                    "Try to get an operator from a non-operator token.");
                private set => op = value;
            }

            public bool OperatorTest(Operator op) => IsOperator && Operator == op;

            public override string ToString()
            {
                if (IsName)
                    return $"\"{Name}\"";
                else
                {
                    if (Operator == Operator.LeftBrace)
                        return "(";
                    else if (Operator == Operator.RightBrace)
                        return ")";
                    else
                        return Operator.ToString();
                }
            }
        }

        private enum Operator
        {
            And,
            Or,
            Not,
            LeftBrace,
            RightBrace
        }

        //dict[start] = stop
        //[start, stop)
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

        //[start, stop)
        private static Token GetNameToken(int start, int stop, string expression)
        {
            Debug.Assert(stop - start >= 2);
            Debug.Assert(expression[start] == '"' && expression[stop - 1] == '"');
            var sb = new StringBuilder();
            bool afterSlash = false;
            for (int i = start + 1; i < stop - 1; i++)
            {
                char ch = expression[i];
                if(!afterSlash)
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

        private static bool IsAnd(string expression, int start)
        {
            return start + 3 - 1 < expression.Length
                && (expression.Substring(start, 3) == "and"
                    || expression.Substring(start, 3) == "AND");
        }

        private static bool IsOr(string expression, int start)
        {
            return start + 2 - 1 < expression.Length
                && (expression.Substring(start, 2) == "or"
                    || expression.Substring(start, 2) == "OR");
        }

        private static bool IsNot(string expression, int start)
        {
            return start + 3 - 1 < expression.Length
                && (expression.Substring(start, 3) == "not"
                    || expression.Substring(start, 3) == "NOT");
        }

        /// <summary>
        /// expression string to tokens
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
    }

    public class InvalidExpressionException : ApplicationException
    {
        public InvalidExpressionException() { }
        public InvalidExpressionException(string message)
            : base(message) { }
    }

    public class TokenizeException : InvalidExpressionException
    {
        public TokenizeException() { }
        public TokenizeException(string message)
            : base(message) { }
    }

    public class CalculateException : InvalidExpressionException
    {
        public CalculateException() { }
        public CalculateException(string message)
            : base(message) { }
    }
}
