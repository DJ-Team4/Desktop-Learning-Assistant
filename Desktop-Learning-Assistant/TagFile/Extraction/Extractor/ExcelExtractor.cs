using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 使用 OpenXml 提取 Excel 中的文字。
    /// 只支持 XML 格式的 Excel（*.xlsx）。
    /// </summary>
    class ExcelExtractor : DocumentExtractorBase
    {
        public ExcelExtractor(string filepath, int lengthLimit = -1)
        {
            Filepath = filepath;
            LengthLimit = lengthLimit;
        }

        public override IEnumerable<string> EnumerableText
            => new EnumerableExcel(Filepath);
    }
}
