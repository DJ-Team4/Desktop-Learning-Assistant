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
        static void Main(string[] args)
        {
            string e1 = "\"t1\" and (\"t2\" or not \"t3\")";
            string e2 = "not(\"t1\"and\"t\")";
            string ie = "not \"t1\\\\t2\" and";
            string s = ie;
            Console.WriteLine(s);
            TagExpression.Test(s);
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
    }
}
