using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TagFile.Extraction.Extractor;

namespace DesktopLearningAssistant.TagFile.Extraction
{
    static class ExtractorFactory
    {
        /// <summary>
        /// 根据文件后缀名获取文档提取器的工厂方法
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="lengthLimit">最大字数限制</param>
        /// <returns>若无对应的提取器则返回 null</returns>
        public static IDocumentExtractor CreateExtractor(string filepath, int lengthLimit = -1)
        {
            string ext = Path.GetExtension(filepath);
            if (ext == ".docx")
                return new WordExtractor(filepath, lengthLimit);
            else if (ext == ".pptx")
                return new SlideExtractor(filepath, lengthLimit);
            else if (ext == ".xlsx")
                return new ExcelExtractor(filepath, lengthLimit);
            else if (ext == ".pdf")
                return new PdfExtractor(filepath, lengthLimit);
            else if (ext == ".txt" || ext == ".md")
                return new PlainTextExtractor(filepath, lengthLimit);
            else
                return null;
        }
    }
}
