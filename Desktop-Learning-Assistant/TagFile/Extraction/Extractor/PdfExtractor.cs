using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 使用 iTextSharp 提取 PDF 中的文字
    /// </summary>
    class PdfExtractor : DocumentExtractorBase
    {
        public PdfExtractor(string filepath, int lengthLimit = -1)
        {
            Filepath = filepath;
            LengthLimit = lengthLimit;
        }

        public override IEnumerable<string> EnumerableText
            => new EnumerablePdf(Filepath);
    }
}
