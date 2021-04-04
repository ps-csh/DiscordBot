using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using DiscordBot.Utility;
using System.IO;
using DiscordBot.Startup;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore;
using DiscordBot.Bot;
using DiscordBot.Models.Yandere;
using Microsoft.Extensions.Configuration.Json;
using System.Configuration;

namespace DiscordBot.Models
{
    public class DatabaseContext : DbContext
    {
        #region Models

        public DbSet<Tag> Tags { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Anime> Anime { get; set; }
        public DbSet<Pool> YanderePools { get; set; }
        public DbSet<Post> YanderePosts { get; set; }

        #endregion

        private readonly ILogger Logger;

        public DatabaseContext(DbContextOptions options) :base(options)
        {
            //TODO:
            // This will cause issues when migrating database
            //bool result = Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {      
            //TODO: Is this a required fallback for configuring?
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["default"].ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
