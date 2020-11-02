﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.TimeStatistic.Model;
using Microsoft.EntityFrameworkCore;

namespace DesktopLearningAssistant.TimeStatistic
{
    /// <summary>
    /// Database context of Time Statistic Service
    /// </summary>
    public class TimeDataContext : DbContext
    {
        public TimeDataContext(DbContextOptions<TimeDataContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }

        //Entity mappings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserActivity>().HasKey(ua => new { ua.Name, ua.CloseTime});     // 确认主键

            modelBuilder.Entity<UserActivityPiece>().HasKey(uap => new { uap.Name, uap.StartTime });    // 确认主键
        }

        public DbSet<UserActivity> KilledActivities;
        public DbSet<UserActivityPiece> UserActivityPieces;
    }
}
