﻿using ChatMicroservice.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ChatMicroservice.Data.Context
{
    public class MyDbContext : DbContext
    {
        //public MyDbContext(DbContextOptions<MyDbContext> dbContextOptions) : base(dbContextOptions) { }

        #region DbSets
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<EmojiCategory> EmojiCategories { get; set; }
        public DbSet<Emoji> Emojis { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Add this in appsettings or ENV (dev, prod) vars
            optionsBuilder.UseMySql("server=localhost;database=PingChatMicroserviceDb;user=root;password=",
                a => a.MigrationsAssembly("ChatMicroservice.Data"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Contact>()
                .HasKey(c => new { c.AccountId, c.ContactAccountId });
        }
    }
}
