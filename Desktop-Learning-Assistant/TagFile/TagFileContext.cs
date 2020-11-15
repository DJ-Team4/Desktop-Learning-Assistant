using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TagFile.Model;
using Microsoft.EntityFrameworkCore;

namespace DesktopLearningAssistant.TagFile
{
    /// <summary>
    /// Database context of tag file service
    /// </summary>
    public class TagFileContext : DbContext
    {
        public TagFileContext(DbContextOptions<TagFileContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }

        //Entity mappings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>().HasIndex(tag => tag.TagName)
                                      .IsUnique();

            modelBuilder.Entity<TagFileRelation>()
                        .HasKey(tf => new { tf.TagId, tf.FileItemId });

            modelBuilder.Entity<TagFileRelation>()
                        .HasOne(tf => tf.Tag)
                        .WithMany(tag => tag.Relations)
                        .HasForeignKey(tf => tf.TagId);

            modelBuilder.Entity<TagFileRelation>()
                        .HasOne(tf => tf.FileItem)
                        .WithMany(file => file.Relations)
                        .HasForeignKey(tf => tf.FileItemId);

            modelBuilder.Entity<TagFileRelation>()
                        .Ignore(tf => tf.LocalCreateTime);
        }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<FileItem> FileItems { get; set; }
        public DbSet<TagFileRelation> Relations { get; set; }
    }
}
