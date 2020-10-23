using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TagFile.Model;
using Microsoft.EntityFrameworkCore;

namespace DesktopLearningAssistant.TagFile.Context
{
    public class TagFileContext : DbContext
    {
        public TagFileContext(DbContextOptions<TagFileContext> options)
            : base(options) { }

        //Entity mappings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<FileItem> FileItems { get; set; }
        public DbSet<TagFileRelation> Relations { get; set; }
    }
}
