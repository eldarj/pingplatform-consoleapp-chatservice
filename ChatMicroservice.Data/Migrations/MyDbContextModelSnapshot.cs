﻿// <auto-generated />
using System;
using ChatMicroservice.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ChatMicroservice.Data.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ChatMicroservice.Data.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AvatarImageUrl");

                    b.Property<string>("CoverImageUrl");

                    b.Property<string>("DataSpaceDirName");

                    b.Property<DateTime>("DateRegistered");

                    b.Property<string>("Firstname");

                    b.Property<string>("Lastname");

                    b.Property<string>("PhoneNumber")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("ChatMicroservice.Data.Models.Contact", b =>
                {
                    b.Property<int>("AccountId");

                    b.Property<int>("ContactAccountId");

                    b.Property<string>("ContactName");

                    b.Property<DateTime>("DateAdded");

                    b.HasKey("AccountId", "ContactAccountId");

                    b.HasIndex("ContactAccountId");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("ChatMicroservice.Data.Models.Contact", b =>
                {
                    b.HasOne("ChatMicroservice.Data.Models.Account", "Account")
                        .WithMany("Contacts")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ChatMicroservice.Data.Models.Account", "ContactAccount")
                        .WithMany()
                        .HasForeignKey("ContactAccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
