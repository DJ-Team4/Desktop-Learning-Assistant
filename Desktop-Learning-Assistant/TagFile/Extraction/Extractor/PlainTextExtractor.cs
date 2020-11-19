using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 提取纯文本文件中的文字
    /// </summary>
    class PlainTextExtractor : DocumentExtractorBase
    {
        public PlainTextExtractor(string filepath, int lengthLimit = -1)
        {
            Filepath = filepath;
            LengthLimit = lengthLimit;
        }

        public override IEnumerable<string> EnumerableText
            => new EnumerablePlainText(Filepath);
    }
}
