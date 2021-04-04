using DiscordBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace DiscordBot.Startup
{
    class DatabaseContextDesignFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite(configuration.GetConnectionString("default"))
                .Options;

            return new DatabaseContext(options);
        }
    }
}
