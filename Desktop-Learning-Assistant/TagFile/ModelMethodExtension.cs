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
        /// 为 FileItem 增加一个标签。
        /// 若 FileItem 已包含该标签，则什么都不做。
        /// 若系统中不含该名字的标签，会自动生成新标签并插入。
        /// </summary>
        public static async Task AddTagAsync(this FileItem fileItem, string tagName)
        {
            var service = TagFileService.GetService();
            Tag tag = await service.AddTagAsync(tagName);
            await service.AddRelationAsync(tag, fileItem);
        }

        /// <summary>
        /// 移除 FileItem 的某个标签。
        /// 若 FileItem 不含该标签，则什么都不做。
        /// </summary>
        public static async Task RemoveTag(this FileItem fileItem, Tag tag)
        {
            await TagFileService.GetService().RemoveRelationAsync(tag, fileItem);
        }

        #endregion

        #region Tag 类的扩展方法

        /// <summary>
        /// 重命名该 Tag。
        /// 若新名字的 Tag 已经存在，则返回 false
        /// </summary>
        public static async Task Rename(this Tag tag, string newName)
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
        /// <param name="tag"></param>
        /// <param name="fileItem"></param>
        /// <returns></returns>
        public static async Task RemoveFileAsync(this Tag tag, FileItem fileItem)
        {
            await TagFileService.GetService().RemoveRelationAsync(tag, fileItem);
        }

        #endregion
    }
}
