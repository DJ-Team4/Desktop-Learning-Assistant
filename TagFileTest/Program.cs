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
            TagFileService.EnsureDbAndFolderCreated();
            Console.WriteLine("db ok");
            var service = TagFileService.GetService();
            var tag = await service.AddTagAsync("tagname");
            //var file = await service.AddFileItemForTestAsync("disp", "real");
            var file = await service.GetFileItemAsync("disp");
            await file.AddTagAsync(tag);
            Console.WriteLine(tag);
            Console.WriteLine(file);
        }

        static async Task Test1()
        {
            await Task.Delay(100);
        }
    }
}
