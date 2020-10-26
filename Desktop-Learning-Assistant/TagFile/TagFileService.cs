using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TagFile.Model;
using DesktopLearningAssistant.TagFile.Expression;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DesktopLearningAssistant.TagFile
{
    public class TagFileService
    {
        #region Tag 相关操作

        /// <summary>
        /// 按 TagName 获取 Tag
        /// </summary>
        /// <returns>不存在则返回 null</returns>
        public async Task<Tag> GetTagAsync(string tagName)
        {
            return await context.Tags.FindAsync(tagName);
        }

        /// <summary>
        /// 将新 Tag 加入系统中，并返回添加的 Tag 对象。
        /// 若该名字的 Tag 已存在，则什么都不做。
        /// </summary>
        public async Task<Tag> AddTagAsync(string tagName)
        {
            Tag tag = await GetTagAsync(tagName);
            if (tag == null)
            {
                tag = new Tag { TagName = tagName };
                await context.Tags.AddAsync(tag);
                await context.SaveChangesAsync();
            }
            return tag;
        }

        /// <summary>
        /// 将该 Tag 从系统中移除
        /// </summary>
        public async Task RemoveTagAsync(Tag tag)
        {
            context.Tags.Remove(tag);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// 重命名该 Tag。
        /// 若新名字的 Tag 已经存在，则什么都不做并返回 false。
        /// </summary>
        public async Task<bool> RenameTagAsync(Tag tag, string newName)
        {
            if (!await IsTagExist(newName))
            {
                tag.TagName = newName;
                await context.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 该名字的 Tag 是否存在
        /// </summary>
        public async Task<bool> IsTagExist(string tagName)
        {
            return await GetTagAsync(tagName) != null;
        }

        /// <summary>
        /// 获取含所有 Tag 的 List
        /// </summary>
        public async Task<List<Tag>> GetTagListAsync()
        {
            return await context.Tags.ToListAsync();
        }

        #endregion

        #region TagFileRelation 有关操作

        /// <summary>
        /// 获取 TagFileRelation 对象。
        /// </summary>
        /// <returns>不存在则返回 null</returns>
        public async Task<TagFileRelation> GetRelationAsync(Tag tag, FileItem fileItem)
        {
            return await context.Relations.FindAsync(tag.TagName, fileItem.FileItemId);
        }

        /// <summary>
        /// 插入一个新的 Tag-FileItem 关系，并返回实体类对象。
        /// 若关系已存在，则什么都不做。
        /// </summary>
        /// <returns>关系实体类</returns>
        public async Task<TagFileRelation> AddRelationAsync(Tag tag, FileItem fileItem)
        {
            var relation = await GetRelationAsync(tag, fileItem);
            if (relation == null)
            {
                relation = new TagFileRelation
                {
                    TagName = tag.TagName,
                    FileItemId = fileItem.FileItemId,
                    CreateTime = DateTime.UtcNow
                };
                await context.Relations.AddAsync(relation);
                await context.SaveChangesAsync();
            }
            return relation;
        }

        /// <summary>
        /// 移除 TagFileRelation 对象
        /// </summary>
        public async Task RemoveRelationAsync(TagFileRelation relation)
        {
            context.Relations.Remove(relation);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// 移除某个 Tag-FileItem 关系。
        /// 若该关系不存在，则什么都不做。
        /// </summary>
        public async Task RemoveRelationAsync(Tag tag, FileItem fileItem)
        {
            var relation = await GetRelationAsync(tag, fileItem);
            if (relation != null)
                await RemoveRelationAsync(relation);
        }

        /// <summary>
        /// 查询关系是否存在
        /// </summary>
        public async Task<bool> IsRelationExist(Tag tag, FileItem fileItem)
        {
            return (await GetRelationAsync(tag, fileItem)) != null;
        }

        #endregion

        #region FileItem 有关操作

        //only for test adding file item
        public async Task<FileItem> AddFileItemForTestAsync(string dispName, string realName)
        {
            var file = new FileItem { DisplayName = dispName, RealName = realName };
            await context.FileItems.AddAsync(file);
            await context.SaveChangesAsync();
            return file;
        }

        /// <summary>
        /// 按 FileItemId 获取 FileItem
        /// </summary>
        public async Task<FileItem> GetFileItemAsync(int fileItemId)
        {
            return await context.FileItems.FindAsync(fileItemId);
        }

        /// <summary>
        /// 添加 FileItem
        /// </summary>
        private async Task AddFileItemAsync(FileItem fileItem)
        {
            await context.FileItems.AddAsync(fileItem);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// 将文件以快捷方式的形式加入仓库
        /// </summary>
        /// <param name="filepath">文件路径</param>
        public async Task<FileItem> AddShortcutToRepoAsync(string filepath)
        {
            string targetName = Path.GetFileName(filepath);
            string displayName = targetName + ".lnk";
            string shortcutName = FileUtils.GetAvailableFileName(displayName, RepoPath);
            FileUtils.CreateShortcut(filepath, FileUtils.FileInFolder(RepoPath, shortcutName));
            var fileItem = new FileItem
            {
                DisplayName = displayName,
                RealName = shortcutName
            };
            await AddFileItemAsync(fileItem);
            return fileItem;
        }

        /// <summary>
        /// 将文件移动到仓库中
        /// </summary>
        public async Task<FileItem> MoveFileToRepoAsync(string filepath)
        {
            string realName = await Task.Run(
                () => FileUtils.MoveFileAutoNumber(filepath, RepoPath));
            var fileItem = new FileItem
            {
                DisplayName = Path.GetFileName(filepath),
                RealName = realName
            };
            await AddFileItemAsync(fileItem);
            return fileItem;
        }

        /// <summary>
        /// 获取文件在仓库内的真实路径
        /// </summary>
        public string GetRealFilepath(FileItem fileItem)
            => FileUtils.FileInFolder(RepoPath, fileItem.RealName);

        /// <summary>
        /// 删除文件
        /// </summary>
        public async Task DeleteFileAsync(FileItem fileItem)
        {
            string path = GetRealFilepath(fileItem);
            if (File.Exists(path))
            {
                await Task.Run(() => File.Delete(path));
            }
            context.FileItems.Remove(fileItem);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// 将文件移动到回收站
        /// </summary>
        public async Task DeleteFileToRecycleBinAsync(FileItem fileItem)
        {
            if (File.Exists(GetRealFilepath(fileItem)))
            {
                //move to temp recycle first
                string curFilename = FileUtils.MoveFileAutoNumber(
                    GetRealFilepath(fileItem), TempRecyclePath);
                string curPath = Path.Combine(TempRecyclePath, curFilename);
                //then send to system recycle bin
                await Task.Run(() => FileUtils.DeleteFileToRecycleBin(curPath));
            }
            context.FileItems.Remove(fileItem);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        public async Task RenameFileItemAsync(FileItem fileItem, string newName)
        {
            fileItem.DisplayName = newName;
            fileItem.RealName = await Task.Run(() =>
                FileUtils.RenameFileAutoNumber(GetRealFilepath(fileItem), newName));
            await context.SaveChangesAsync();
        }

        #endregion

        /// <summary>
        /// 表达式查询
        /// </summary>
        public async Task<List<FileItem>> Query(string expression)
        {
            var files = new List<FileItem>();
            var idList = TagExpression.Query(context.Relations, expression);
            foreach (int fileItemId in idList)
                files.Add(await GetFileItemAsync(fileItemId));
            return files;
        }

        /// <summary>
        /// 获取单例对象
        /// </summary>
        public static TagFileService GetService()
        {
            if (uniqueService == null)
            {
                lock (locker)
                {
                    if (uniqueService == null)
                        uniqueService = new TagFileService();
                }
            }
            return uniqueService;
        }

        /// <summary>
        /// 确保数据库和仓库文件夹已创建
        /// </summary>
        public static void EnsureDbAndFolderCreated()
        {
            var builder = new DbContextOptionsBuilder<TagFileContext>();
            builder.UseSqlite($"Data Source={TagFileConfig.DbPath}");
            using (var context = new TagFileContext(builder.Options))
            {
                context.Database.EnsureCreated();
            }
            //TODO ensure repo folder created
        }

        public static async Task EnsureDbAndFolderCreatedAsync()
        {
            var builder = new DbContextOptionsBuilder<TagFileContext>();
            builder.UseSqlite($"Data Source={TagFileConfig.DbPath}");
            using (var context = new TagFileContext(builder.Options))
            {
                await context.Database.EnsureCreatedAsync();
            }
            //TODO ensure repo folder created
        }

        private TagFileService()
        {
            var builder = new DbContextOptionsBuilder<TagFileContext>();
            builder.UseSqlite($"Data Source={TagFileConfig.DbPath}");
            context = new TagFileContext(builder.Options);
        }

        private string RepoPath { get => TagFileConfig.RepoPath; }

        private string TempRecyclePath { get => TagFileConfig.TempRecyclePath; }

        private readonly TagFileContext context;

        private static volatile TagFileService uniqueService = null;

        private static readonly object locker = new object();
    }

    //TODO modify this to a config class
    class TagFileConfig
    {
        public static string RepoPath { get; } = "C:/Users/zhb/Desktop/temp/tag-file/repo";
        public static string DbPath { get; } = "C:/Users/zhb/Documents/sqlitedb/TagFileDB.db";
        public static string TempRecyclePath { get; } = "C:/Users/zhb/Desktop/temp/tag-file/temp-recycle";
    }
}
