using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile
{
    /// <summary>
    /// 与文件有关的操作
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// 创建快捷方式
        /// </summary>
        public static void CreateShortcut(string targetPath, string shortcutPath)
        {
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShellClass();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.Save();
        }

        /// <summary>
        /// 使用系统默认的程序打开文件
        /// </summary>
        public static void OpenFile(string filepath)
        {
            System.Diagnostics.Process.Start(filepath);
        }

        /// <summary>
        /// 在“资源管理器”中显示该文件
        /// </summary>
        public static void ShowInExplorer(string filepath)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/select," + filepath);
        }

        /// <summary>
        /// 将文件放入回收站
        /// </summary>
        public static void DeleteFileToRecycleBin(string filepath)
        {
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                filepath,
                Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }

        /// <summary>
        /// 将文件夹放入回收站
        /// </summary>
        public static void DeleteFolderToRecycleBin(string folderpath)
        {
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(
                folderpath,
                Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }

        /// <summary>
        /// 获取 string 的最后一个字符，保证时间复杂度 O(1)
        /// </summary>
        private static char LastChar(this string s) => s[s.Length - 1];

        /// <summary>
        /// 保证文件夹路径以斜线结尾
        /// </summary>
        /// <param name="folderpath">文件夹路径，结尾有无斜线均可</param>
        public static string FolderEndWithSlash(string folderpath)
        {
            return folderpath.LastChar() == '/'
                || folderpath.LastChar() == '\\' ? folderpath
                                                 : folderpath + "\\";
        }

        /// <summary>
        /// 构造“文件在文件夹中”的路径，自动处理斜线
        /// </summary>
        public static string FileInFolder(string folderpath, string filename)
        {
            return Path.GetFullPath(FolderEndWithSlash(folderpath) + Path.GetFileName(filename));
        }

        /// <summary>
        /// 获取一个合适的文件名。
        /// 若文件夹中已有同名文件则返回自动编号的文件名。
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="folderpath">文件夹路径</param>
        /// <returns>自动编号的文件名</returns>
        public static string GetAvailableFileName(string filename, string folderpath)
        {
            filename = Path.GetFileName(filename);
            if (!File.Exists(Path.Combine(folderpath, filename)))
                return filename;

            string nameWithoutExt = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);
            int num = 1;
            while (File.Exists(Path.Combine(folderpath, $"{nameWithoutExt} ({num}){ext}")))
                num += 1;
            return $"{nameWithoutExt} ({num}){ext}";
        }

        /// <summary>
        /// 将文件移动到文件夹，若重名则自动编号。
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="folderpath">文件夹路径</param>
        /// <returns>移动后的文件名（不含路径）</returns>
        public static string MoveFileAutoNumber(string filepath, string folderpath)
        {
            string realName = GetAvailableFileName(filepath, folderpath);
            File.Move(filepath, Path.Combine(folderpath, realName));
            return realName;
        }

        /// <summary>
        /// 重命名文件。若有重名则自动编号。
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="newName">新文件名</param>
        /// <returns>重命名后的文件名（不含路径）</returns>
        public static string RenameFileAutoNumber(string filepath, string newName)
        {
            string folderpath = Path.GetDirectoryName(filepath);
            newName = Path.GetFileName(newName);
            string realNewName = GetAvailableFileName(newName, folderpath);
            File.Move(filepath, Path.Combine(folderpath, realNewName));
            return realNewName;
        }
    }
}
