using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TagFile;
using DesktopLearningAssistant.TagFile.Model;
using DesktopLearningAssistant.TagFile.Expression;

namespace TagFileTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await QueryTest();
        }

        static async Task QueryTest()
        {
            TagFileService.EnsureDbAndFolderCreated();
            Console.WriteLine("db ok");
            var service = TagFileService.GetService();
            string[] tagNames = { "t1", "t2", "t3", "t4" };
            foreach (string tagName in tagNames)
                await service.AddTagAsync(tagName);

            var f1 = await service.AddFileItemForTestAsync("f1", "f1");
            await f1.AddTagAsync(await service.GetTagAsync("t1"));
            await f1.AddTagAsync(await service.GetTagAsync("t2"));
            await f1.AddTagAsync(await service.GetTagAsync("t3"));

            var f2 = await service.AddFileItemForTestAsync("f2", "f2");
            await f2.AddTagAsync(await service.GetTagAsync("t1"));

            try
            {
                string expr = "(\"t1\" or \"t2\") and not \"t3\"";
                Console.WriteLine(expr);
                PrintEnumerable(await service.QueryAsync(expr));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void TokenizeTest()
        {
            //string e1 = "\"t1\" and (\"t2\" or not \"t3\")";
            //string e2 = "not(\"t1\"and\"t\")";
            string ie = "not \"t1\\\\t2\" and";
            string s = ie;
            Console.WriteLine(s);
            TagExpression.TokenizeTest(s);
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

        static void PrintEnumerable<T>(IEnumerable<T> c)
        {
            foreach (T x in c)
            {
                Console.WriteLine(x);
            }
        }
    }
}
