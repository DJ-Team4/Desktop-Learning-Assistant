using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 文档文字提取器
    /// </summary>
    public interface IDocumentExtractor
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        string Filepath { get; set; }

        /// <summary>
        /// 最大长度，设为负数表示不限制。
        /// </summary>
        int LengthLimit { get; set; }

        /// <summary>
        /// 文本内容
        /// </summary>
        string Text { get; }

        /// <summary>
        /// 所有行的内容
        /// </summary>
        string[] TextArray { get; }

        /// <summary>
        /// 不带换行符的文本内容
        /// </summary>
        string TextWithoutNewLine { get; }

        /// <summary>
        /// 获取每段内容的可迭代对象
        /// </summary>
        IEnumerable<string> EnumerableText { get; }
    }
}
