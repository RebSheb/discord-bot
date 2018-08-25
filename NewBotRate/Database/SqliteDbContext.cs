using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.EntityFrameworkCore;



namespace NewBotRate.Database
{
    public class SqliteDbContext : DbContext
    {
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Tag> Tags { get; set; } 


        protected override void OnConfiguring(DbContextOptionsBuilder Options)
        {
            Options.UseSqlite($"Data Source=../../../Database.sqlite");
        }
    }
}
