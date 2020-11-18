using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DesktopLearningAssistant.TagFile.Extraction.Extractor
{
    /// <summary>
    /// 用于获取 Excel 中每个单元格内容的可迭代对象
    /// </summary>
    class EnumerableExcel : IEnumerable<string>
    {
        public EnumerableExcel(string filepath) => Filepath = filepath;

        public string Filepath { get; set; }

        public IEnumerator<string> GetEnumerator()
        {
            if (new System.IO.FileInfo(Filepath).Length == 0)
                yield break; // Empty File
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(Filepath, false))
            {
                WorkbookPart workbookPart = document.WorkbookPart;
                SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (sstpart == null)
                    yield break; // Empty Excel
                SharedStringTable sst = sstpart.SharedStringTable;
                foreach (WorksheetPart worksheetPart in workbookPart.WorksheetParts)
                {
                    Worksheet sheet = worksheetPart.Worksheet;
                    foreach (Cell cell in sheet.Descendants<Cell>())
                    {
                        if (cell.DataType != null)
                        {
                            string cellText;
                            if (cell.DataType == CellValues.SharedString)
                            {
                                int ssid = int.Parse(cell.CellValue.Text);
                                cellText = sst.ChildElements[ssid].InnerText;
                            }
                            else
                            {
                                cellText = cell.CellValue.Text;
                            }
                            cellText = cellText.Trim();
                            if (cellText.Length == 0)
                                continue;
                            yield return cellText;
                        }
                    }
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
