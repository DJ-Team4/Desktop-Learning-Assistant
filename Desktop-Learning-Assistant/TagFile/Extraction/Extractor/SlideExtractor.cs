using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 使用 OpenXml 提取 PPT 中的文字。
    /// 只支持 XML 形式的 PPT（*.pptx）。
    /// </summary>
    class SlideExtractor : DocumentExtractorBase
    {
        public SlideExtractor(string filepath, int lengthLimit = -1)
        {
            Filepath = filepath;
            LengthLimit = lengthLimit;
        }

        public override IEnumerable<string> EnumerableText
            => new EnumerableSlide(Filepath);
    }
}
