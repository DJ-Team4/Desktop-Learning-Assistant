using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TagFile;
using DesktopLearningAssistant.TagFile.Model;

namespace TagFileTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var service = TagFileService.GetService();
        }

        static async Task Test1()
        {
            var service = TagFileService.GetService();
            var tag = await service.AddTagAsync("tagname");
            var file = await service.AddFileItemForTestAsync("1234", "1234");
            await file.AddTagAsync(tag);
            Console.WriteLine(file);
            Console.WriteLine(tag);

            await file.RemoveTag(tag);
            Console.WriteLine(file);
            Console.WriteLine(tag);
        }

        static void Main2(string[] args)
        {

        }
    }
}
