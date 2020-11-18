using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 文档提取器的公共抽象基类。
    /// 提供了除 EnumerableText 外的属性的实现。
    /// </summary>
    public abstract class DocumentExtractorBase : IDocumentExtractor
    {
        public string Filepath { get; set; }

        public int LengthLimit { get; set; } = -1;

        public string Text
        {
            get
            {
                var sb = new StringBuilder();
                foreach (string line in
                    new LengthLimitEnumerable(EnumerableText, LengthLimit))
                {
                    sb.Append(line).Append(Environment.NewLine);
                }
                return sb.ToString();
            }
        }

        public string[] TextArray
        {
            get
            {
                var lines = new List<string>();
                foreach (string line in
                    new LengthLimitEnumerable(EnumerableText, LengthLimit))
                {
                    lines.Add(line);
                }
                return lines.ToArray();
            }
        }

        public string TextWithoutNewLine
        {
            get
            {
                var sb = new StringBuilder();
                foreach (string line in
                    new LengthLimitEnumerable(EnumerableText, LengthLimit))
                {
                    sb.Append(line);
                }
                return sb.ToString();
            }
        }

        public abstract IEnumerable<string> EnumerableText { get; }
    }
}
