using DesktopLearningAssistant.TagFile.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile
{
    /// <summary>
    /// 为 Model 类增加一些有用、方便的扩展方法
    /// </summary>
    public static class ModelMethodExtension
    {
        #region FileItem 的扩展方法

        /// <summary>
        /// 为 FileItem 增加一个标签。
        /// 若 FileItem 已包含该标签，则什么都不做。
        /// </summary>
        public static async Task AddTagAsync(this FileItem fileItem, Tag tag)
        {
            await TagFileService.GetService().AddRelationAsync(tag, fileItem);
        }

        /// <summary>
        /// 移除 FileItem 的某个标签。
        /// 若 FileItem 不含该标签，则什么都不做。
        /// </summary>
        public static async Task RemoveTagAsync(this FileItem fileItem, Tag tag)
        {
            await TagFileService.GetService().RemoveRelationAsync(tag, fileItem);
        }

        /// <summary>
        /// 文件在仓库内的真实路径
        /// </summary>
        public static string RealPath(this FileItem fileItem)
        {
            return TagFileService.GetService().GetRealFilepath(fileItem);
        }

        /// <summary>
        /// 使用默认程序打开文件
        /// </summary>
        public static void Open(this FileItem fileItem)
        {
            FileUtils.OpenFile(fileItem.RealPath());
        }

        /// <summary>
        /// 在“资源管理器”中显示
        /// </summary>
        public static void ShowInExplorer(this FileItem fileItem)
        {
            FileUtils.ShowInExplorer(fileItem.RealPath());
        }

        /// <summary>
        /// 重命名文件。
        /// 该方法不会检查文件名是否合法。
        /// </summary>
        public static async Task RenameAsync(this FileItem fileItem, string newName)
        {
            await TagFileService.GetService().RenameFileItemAsync(fileItem, newName);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public static async Task DeleteAsync(this FileItem fileItem)
        {
            await TagFileService.GetService().DeleteFileAsync(fileItem);
        }

        /// <summary>
        /// 把文件放入回收站
        /// </summary>
        public static async Task DeleteToRecycleBinAsync(this FileItem fileItem)
        {
            await TagFileService.GetService().DeleteFileToRecycleBinAsync(fileItem);
        }

        #endregion

        #region Tag 类的扩展方法

        /// <summary>
        /// 重命名该 Tag。
        /// 若新名字的 Tag 已经存在，则什么都不做并返回 false。
        /// </summary>
        public static async Task RenameAsync(this Tag tag, string newName)
        {
            await TagFileService.GetService().RenameTagAsync(tag, newName);
        }

        /// <summary>
        /// 为 Tag 增加一个文件。
        /// 若该文件已含该 Tag，则什么都不做。
        /// </summary>
        public static async Task AddFileAsync(this Tag tag, FileItem fileItem)
        {
            await TagFileService.GetService().AddRelationAsync(tag, fileItem);
        }

        /// <summary>
        /// 为 Tag 移除文件。
        /// 若该文件不含该 Tag，则什么都不做。
        /// </summary>
        public static async Task RemoveFileAsync(this Tag tag, FileItem fileItem)
        {
            await TagFileService.GetService().RemoveRelationAsync(tag, fileItem);
        }

        #endregion
    }
}
