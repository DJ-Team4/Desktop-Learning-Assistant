using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 用于获取纯文本文件中每行内容的可迭代对象
    /// </summary>
    class EnumerablePlainText : IEnumerable<string>
    {
        public EnumerablePlainText(string filepath) => Filepath = filepath;

        public string Filepath { get; set; }

        public IEnumerator<string> GetEnumerator()
        {
            var lines = File.ReadLines(Filepath);
            foreach (string line in lines)
            {
                string tline = line.Trim();
                if (tline.Length == 0)
                    continue;
                yield return tline;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
