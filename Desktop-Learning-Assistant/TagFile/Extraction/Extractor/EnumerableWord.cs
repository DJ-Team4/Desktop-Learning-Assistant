using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 用于获取 Word 文档中每段内容的可迭代对象
    /// </summary>
    public class EnumerableWord : IEnumerable<string>
    {
        public EnumerableWord(string filepath) => Filepath = filepath;

        public string Filepath { get; set; }

        public IEnumerator<string> GetEnumerator()
        {
            if (new System.IO.FileInfo(Filepath).Length == 0)
                yield break; // Empty File
            using (WordprocessingDocument wdDoc = WordprocessingDocument.Open(Filepath, false))
            {
                // Manage namespaces to perform XPath queries.
                NameTable nt = new NameTable();
                XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
                nsManager.AddNamespace("w", wordmlNamespace);

                // Get the document part from the package.  
                // Load the XML in the document part into an XmlDocument instance.
                XmlDocument xdoc = new XmlDocument(nt);
                xdoc.Load(wdDoc.MainDocumentPart.GetStream());

                XmlNodeList paragraphNodes = xdoc.SelectNodes("//w:p", nsManager);
                foreach (XmlNode paragraphNode in paragraphNodes)
                {
                    // Get text in a paragrath
                    var paraSb = new StringBuilder();
                    XmlNodeList textNodes = paragraphNode.SelectNodes(".//w:t", nsManager);
                    foreach (XmlNode textNode in textNodes)
                        paraSb.Append(textNode.InnerText);
                    string para = paraSb.ToString().Trim();
                    if (para.Length == 0)
                        continue;
                    yield return para;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();

        private static readonly string wordmlNamespace
            = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    }
}
