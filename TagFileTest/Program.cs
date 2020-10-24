using System;
using System.Collections.Generic;
using System.IO;
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
            var file = await service.GetFileItemAsync(3);
            await file.DeleteToRecycleBin();
            Console.WriteLine(file);
        }

        static async Task TestShortcut()
        {
            string filepath = @"C:\Users\zhb\Desktop\temp\file.txt";
            TagFileService.EnsureDbAndFolderCreated();
            Console.WriteLine("db ok");
            var service = TagFileService.GetService();
            var file = await service.AddShortcutToRepoAsync(filepath);
            var tag = await service.AddTagAsync("tagname");
            await file.AddTagAsync(tag);
            Console.WriteLine(file);
            Console.WriteLine(tag);
        }

        static async Task TestMoveFile()
        {
            string filepath = @"C:\Users\zhb\Desktop\temp\movefile.txt";
            TagFileService.EnsureDbAndFolderCreated();
            Console.WriteLine("db ok");
            var service = TagFileService.GetService();
            var file = await service.MoveFileToRepoAsync(filepath);
            var tag = await service.AddTagAsync("tagname");
            await file.AddTagAsync(tag);
            Console.WriteLine(file);
            Console.WriteLine(tag);
        }

        static async Task Test1()
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
    }
}
