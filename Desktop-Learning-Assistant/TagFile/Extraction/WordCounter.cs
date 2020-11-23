using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TagFile.Extraction.AcAutomaton;

namespace DesktopLearningAssistant.TagFile.Extraction
{
    class WordCounter
    {
        public WordCounter(IEnumerable<string> words)
        {
            this.words = new HashSet<string>(words);
            useAc = this.words.Count > COUNT_BOUND;
            if (useAc)
                acAutomaton = new Utf32AcAutomaton(words);
        }

        public Dictionary<string, int> CountWordsFromFile(string filepath)
        {
            var extractor = ExtractorFactory.CreateExtractor(filepath, LENGTH_LIMIT);
            if (extractor == null)
                return CountWordsFromText("");
            string text = extractor.Text;
            return CountWordsFromText(text);
        }

        public Dictionary<string, int> CountWordsFromText(string text)
        {
            if (useAc)
                return acAutomaton.Query(text);
            else
            {
                var counter = new Dictionary<string, int>();
                foreach (string pattern in words)
                    counter[pattern] = CountSubstr(text, pattern);
                return counter;
            }
        }

        public List<string> OrderedListFromFile(string filepath)
        {
            return DictToSortedList(CountWordsFromFile(filepath));
        }

        public List<string> OrderedListFromText(string text)
        {
            return DictToSortedList(CountWordsFromText(text));
        }

        /// <summary>
        /// 次数等于 0 的单词不出现在内
        /// </summary>
        public static List<string> DictToSortedList(Dictionary<string, int> counterDict)
        {
            List<KeyValuePair<string, int>> kvList = counterDict.ToList();
            //按次数从大到小
            kvList.Sort((p1, p2) => p2.Value.CompareTo(p1.Value));
            var wordList = new List<string>();
            foreach (var p in kvList)
                if (p.Value > 0)
                    wordList.Add(p.Key);
            return wordList;
        }

        private readonly HashSet<string> words;
        private readonly Utf32AcAutomaton acAutomaton = null;
        private readonly bool useAc;

        private static int CountSubstr(string text, string substr)
        {
            int count = 0;
            int minIndex = text.IndexOf(substr, 0);
            while (minIndex != -1)
            {
                minIndex = text.IndexOf(substr, minIndex + substr.Length);
                count++;
            }
            return count;
        }

        private const int COUNT_BOUND = 1;
        private const int LENGTH_LIMIT = 10000;
    }
}
