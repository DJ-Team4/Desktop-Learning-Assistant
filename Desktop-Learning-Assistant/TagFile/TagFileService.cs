using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TagFile.Model;
using DesktopLearningAssistant.TagFile.Context;
using Microsoft.EntityFrameworkCore;
using IWshRuntimeLibrary;
using System.IO;

namespace DesktopLearningAssistant.TagFile
{
    public class TagFileService
    {
        #region Tag 相关操作

        /// <summary>
        /// 按 Id 获取 Tag
        /// </summary>
        /// <returns>不存在则返回 null</returns>
        public async Task<Tag> GetTagByIdAsync(int tagId)
        {
            return await context.Tags.FindAsync(tagId);
        }

        /// <summary>
        /// 按 TagName 获取 Tag
        /// </summary>
        /// <returns>不存在则返回 null</returns>
        public async Task<Tag> GetTagByNameAsync(string tagName)
        {
            return await context.Tags.Where(tag => tag.TagName == tagName).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 将新 Tag 加入系统中，并返回添加的 Tag 对象。
        /// 若该名字的 Tag 已存在，则什么都不做。
        /// </summary>
        public async Task<Tag> AddTagAsync(string tagName)
        {
            Tag tag = await GetTagByNameAsync(tagName);
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
        /// 若新名字的 Tag 已经存在，则返回 false
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
            return await GetTagByNameAsync(tagName) != null;
        }

        #endregion

        #region TagFileRelation 有关操作

        /// <summary>
        /// 获取 TagFileRelation 对象。
        /// </summary>
        /// <returns>不存在则返回 null</returns>
        public async Task<TagFileRelation> GetRelationAsync(Tag tag, FileItem fileItem)
        {
            return await context.Relations.FindAsync(tag.TagId, fileItem.FileItemId);
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
                    TagId = tag.TagId,
                    FileItemId = fileItem.FileItemId,
                    CreateTime = DateTime.Now
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
        /// 查询关系是否存在
        /// </summary>
        public async Task<bool> IsRelationExist(Tag tag, FileItem fileItem)
        {
            return (await GetRelationAsync(tag, fileItem)) != null;
        }

        #endregion

        #region FileItem 有关操作

        //TODO only for test adding file item
        public async Task<FileItem> AddFileItemForTestAsync(string dispName, string realName)
        {
            var file = new FileItem { DisplayName = dispName, RealName = realName };
            await context.FileItems.AddAsync(file);
            await context.SaveChangesAsync();
            return file;
        }

        private async Task AddFileItem(FileItem fileItem)
        {
            await context.FileItems.AddAsync(fileItem);
            await context.SaveChangesAsync();//TODO save
        }

        /// <summary>
        /// 将某个文件以快捷方式的形式加入系统中
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        public async Task<FileItem> AddFileLinkToRepoAsync(string filepath)
        {
            string originFilename = Path.GetFileName(filepath);
            string linkName = originFilename + ".lnk";//TODO 解决仓库中有同名文件的问题
            WshShell shell = new WshShellClass();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(RepoPath + linkName);
            shortcut.TargetPath = filepath;
            shortcut.Save();
            //TOOD real name
            var fileItem = new FileItem { DisplayName = linkName, RealName = linkName };
            await AddFileItem(fileItem);
            return fileItem;
        }

        #endregion

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

        private TagFileService()
        {
            var builder = new DbContextOptionsBuilder<TagFileContext>();
            builder.UseSqlite($"Data Source={TagFileConfig.DbPath}");
            context = new TagFileContext(builder.Options);
        }

        private string RepoPath { get => TagFileConfig.RepoPath; }

        private readonly TagFileContext context;

        private static volatile TagFileService uniqueService = null;

        private static readonly object locker = new object();
    }

    //TODO modify this to a config class
    class TagFileConfig
    {
        public static string RepoPath { get; } = "C:/Users/zhb/Desktop/temp/repo";
        public static string DbPath { get; } = "C:/Users/zhb/Documents/sqlitedb/TagFileDB.db";
    }
}
