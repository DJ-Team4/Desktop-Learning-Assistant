using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 用于获取 PPT 中每张幻灯片内容的可迭代对象
    /// </summary>
    public class EnumerableSlide : IEnumerable<string>
    {
        public EnumerableSlide(string filepath) => Filepath = filepath;

        public string Filepath { get; set; }

        public IEnumerator<string> GetEnumerator()
        {
            if (new System.IO.FileInfo(Filepath).Length == 0)
                yield break; // Empty File
            int slideCnt = CountSlides(Filepath);
            if (slideCnt == 0)
                yield break; // Empty PPT
            using (PresentationDocument ppt = PresentationDocument.Open(Filepath, false))
            {
                PresentationPart part = ppt.PresentationPart;
                OpenXmlElementList slideIds = part.Presentation.SlideIdList.ChildElements;
                for (int index = 0; index < slideCnt; index++)
                {
                    // Get the relationship ID of the slide.
                    string relId = (slideIds[index] as SlideId).RelationshipId;
                    // Get the slide part from the relationship ID.
                    SlidePart slide = (SlidePart)part.GetPartById(relId);
                    // Text in the single slide
                    var sb = new StringBuilder();
                    // Get the inner text of the slide.
                    IEnumerable<A.Text> texts = slide.Slide.Descendants<A.Text>();
                    foreach (A.Text text in texts)
                        sb.Append(text.Text);
                    string textInSlide = sb.ToString().Trim();
                    if (textInSlide.Length == 0)
                        continue;
                    yield return textInSlide;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// Count the slides in the presentation.
        /// </summary>
        private static int CountSlides(string filepath)
        {
            // Open the presentation as read-only.
            using (PresentationDocument presentationDocument = PresentationDocument.Open(filepath, false))
            {
                // Pass the presentation to the next CountSlides method
                // and return the slide count.
                return CountSlides(presentationDocument);
            }
        }

        /// <summary>
        /// Count the slides in the presentation.
        /// </summary>
        private static int CountSlides(PresentationDocument presentationDocument)
        {
            // Check for a null document object.
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            int slidesCount = 0;

            // Get the presentation part of document.
            PresentationPart presentationPart = presentationDocument.PresentationPart;
            // Get the slide count from the SlideParts.
            if (presentationPart != null)
            {
                slidesCount = presentationPart.SlideParts.Count();
            }
            // Return the slide count to the previous method.
            return slidesCount;
        }
    }
}
