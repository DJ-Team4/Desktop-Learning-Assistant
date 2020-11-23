using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 使用 OpenXml 提取 Word 中的文件。
    /// 只支持 XML 形式的 Word 文档（*.docx)。
    /// </summary>
    public class WordExtractor : DocumentExtractorBase
    {
        public WordExtractor(string filepath, int lengthLimit = -1)
        {
            Filepath = filepath;
            LengthLimit = lengthLimit;
        }

        public override IEnumerable<string> EnumerableText
            => new EnumerableWord(Filepath);
    }
}
