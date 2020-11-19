using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 用于获取 PDF 中每行内容的可迭代对象
    /// </summary>
    class EnumerablePdf : IEnumerable<string>
    {
        public EnumerablePdf(string filepath) => Filepath = filepath;

        public string Filepath { get; set; }

        public IEnumerator<string> GetEnumerator()
        {
            if (new System.IO.FileInfo(Filepath).Length == 0)
                yield break; // Empty File
            var reader = new PdfReader(Filepath);
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                //text in a page
                string textInPage = PdfTextExtractor.GetTextFromPage(
                    reader, i, new SimpleTextExtractionStrategy());
                //lines in a page
                string[] lines = textInPage.Trim().Replace("\r", "").Split('\n');
                foreach (string line in lines)
                {
                    string tline = line.Trim();
                    if (tline.Length == 0)
                        continue;
                    yield return tline;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
