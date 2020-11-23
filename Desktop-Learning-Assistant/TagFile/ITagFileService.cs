using DesktopLearningAssistant.TagFile.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLearningAssistant.TagFile
{
    /// <summary>
    /// TagFileService 的抽象外观类
    /// </summary>
    public interface ITagFileService
    {
        #region Tag 相关操作

        /// <summary>
        /// 按 TagId 获取 Tag
        /// </summary>
        Task<Tag> GetTagByIdAsync(int tagId);

        /// <summary>
        /// 按 TagName 获取 Tag
        /// </summary>
        /// <returns>不存在则返回 null</returns>
        Task<Tag> GetTagByNameAsync(string tagName);

        /// <summary>
        /// 将新 Tag 加入系统中，并返回添加的 Tag 对象。
        /// 若该名字的 Tag 已存在，则什么都不做。
        /// </summary>
        Task<Tag> AddTagAsync(string tagName);

        /// <summary>
        /// 将该 Tag 从系统中移除。
        /// 若 Tag 不存在，则什么都不做。
        /// </summary>
        Task RemoveTagAsync(Tag tag);

        /// <summary>
        /// 重命名该 Tag。
        /// 若新名字的 Tag 已经存在，则什么都不做并返回 false。
        /// </summary>
        Task<bool> RenameTagAsync(Tag tag, string newName);

        /// <summary>
        /// 该名字的 Tag 是否存在
        /// </summary>
        Task<bool> IsTagExistAsync(string tagName);

        /// <summary>
        /// 获取含所有 Tag 的 List
        /// </summary>
        Task<List<Tag>> TagListAsync();

        #endregion

        #region TagFileRelation 有关操作

        /// <summary>
        /// 获取 TagFileRelation 对象。
        /// </summary>
        /// <returns>不存在则返回 null</returns>
        Task<TagFileRelation> GetRelationAsync(Tag tag, FileItem fileItem);

        /// <summary>
        /// 插入一个新的 Tag-FileItem 关系，并返回实体类对象。
        /// 若关系已存在，则什么都不做。
        /// </summary>
        /// <returns>关系实体类</returns>
        Task<TagFileRelation> AddRelationAsync(Tag tag, FileItem fileItem);

        /// <summary>
        /// 移除 TagFileRelation 对象。
        /// 若关系不存在则什么都不做。
        /// </summary>
        Task RemoveRelationAsync(TagFileRelation relation);

        /// <summary>
        /// 移除某个 Tag-FileItem 关系。
        /// 若该关系不存在，则什么都不做。
        /// </summary>
        Task RemoveRelationAsync(Tag tag, FileItem fileItem);

        /// <summary>
        /// 查询关系是否存在
        /// </summary>
        Task<bool> IsRelationExistAsync(Tag tag, FileItem fileItem);

        /// <summary>
        /// 获取含所有关系的列表
        /// </summary>
        Task<List<TagFileRelation>> RelationListAsync();

        Task UpdateFileRelationAsync(FileItem fileItem, IEnumerable<Tag> newTags);

        #endregion

        #region FileItem 有关操作

        /// <summary>
        /// 按 FileItemId 获取 FileItem
        /// </summary>
        Task<FileItem> GetFileItemAsync(int fileItemId);

        /// <summary>
        /// 将文件以快捷方式的形式加入仓库
        /// </summary>
        /// <param name="filepath">文件路径</param>
        Task<FileItem> AddShortcutToRepoAsync(string filepath);

        /// <summary>
        /// 将文件移动到仓库中
        /// </summary>
        Task<FileItem> MoveFileToRepoAsync(string filepath);

        /// <summary>
        /// 获取文件在仓库内的真实路径
        /// </summary>
        string GetRealFilepath(FileItem fileItem);

        /// <summary>
        /// 删除文件
        /// </summary>
        Task DeleteFileAsync(FileItem fileItem);

        /// <summary>
        /// 将文件移动到回收站
        /// </summary>
        Task DeleteFileToRecycleBinAsync(FileItem fileItem);

        /// <summary>
        /// 重命名文件
        /// </summary>
        Task RenameFileItemAsync(FileItem fileItem, string newName);

        /// <summary>
        /// 获取含所有 FileItem 的列表
        /// </summary>
        Task<List<FileItem>> FileItemListAsync();

        #endregion

        #region 查询相关操作

        /// <summary>
        /// 表达式查询
        /// </summary>
        /// <exception cref="InvalidExpressionException">查询表达式非法</exception>
        Task<List<FileItem>> QueryAsync(string expression);

        /// <summary>
        /// 获取不含任何标签的文件
        /// </summary>
        Task<List<FileItem>> FilesWithoutTagAsync();

        /// <summary>
        /// 推荐标签
        /// </summary>
        Task<List<Tag>> RecommendTagAsync(string filepath);

        #endregion
    }
}
