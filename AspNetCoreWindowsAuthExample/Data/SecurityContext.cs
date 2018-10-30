using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using AspNetCoreWindowsAuthExample.Models;

namespace AspNetCoreWindowsAuthExample.Data
{
    public class SecurityContext : DbContext
    {
        public SecurityContext(DbContextOptions<SecurityContext> options) : base(options)
        {
        }

        public virtual DbSet<UserInformation> UserInformations { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<SysConfig> SysConfigs { get; set; }

        public virtual DbSet<UserInformationUserRole> UserInformationUserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Build the many to many relationship between users and roles
            modelBuilder.Entity<UserInformationUserRole>()
        .HasKey(ir => new { ir.UserInformationId, ir.UserRoleId });

            modelBuilder.Entity<UserInformationUserRole>()
                .HasOne(ir => ir.UserInformation)
                .WithMany(ui => ui.UserInformationUserRoles)
                .HasForeignKey(ir => ir.UserInformationId);

            modelBuilder.Entity<UserInformationUserRole>()
                .HasOne(ir => ir.UserRole)
                .WithMany(ur => ur.UserInformationUserRoles)
                .HasForeignKey(ir => ir.UserRoleId);

            //Use the new HasData to seed the database with values.
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserRoleId = 1, Name = "Administrator", Description = "Users with admin access" },
                new UserRole { UserRoleId = 2, Name = "User", Description = "Users with base level Access" }
            );

            modelBuilder.Entity<UserInformation>().HasData(
                new UserInformation { UserInformationId = 1, LanId = @"na\karnopp", FirstName = "Jim", LastName = "Karnopp", Email = "jim@kartech.com", Enabled = true }

                );

            modelBuilder.Entity<UserInformationUserRole>().HasData(
                new UserInformationUserRole { UserInformationId = 1, UserRoleId = 1 }
            );

            modelBuilder.Entity<SysConfig>().HasData(
                new SysConfig
                {
                    AppFolder = "AspNetCoreWindowsAuthExample",
                    AppFromEmail = "example@noReply.kartech.com",
                    AppFromName = "Example Application",
                    AppName = "AspNetCoreWindowsAuthExample",
                    BusinessOwnerEmail = "jim@kartech.com",
                    BusinessOwnerName = "Jim Karnopp",
                    DeveloperEmail = "jim@kartech.com",
                    DeveloperName = "Jim Karnopp",
                    Rebuild = "False",
                    SmtpPort = 25,
                    SmtpServer = "smtp.MailServer.com",
                    SysConfigId = 1
                });
        }
    }
}