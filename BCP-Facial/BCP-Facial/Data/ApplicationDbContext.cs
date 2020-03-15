using System;
using System.Collections.Generic;
using System.Text;
using BCP_Facial.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BCP_Facial.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public static readonly string _hashSalt = "3fVPsEy07Kcwk3vr2Y1O6zhYsjK6fqf6v0MOsaEqIEMTw58mU00Qn5CGccAhtFcO";
        public static readonly string LecturerGuid = "EF7A8FDE-0005-4085-B26F-37D7278BE768";
        public static readonly string AdminGuid = "4FBD4989-DF6E-479A-AE7D-641700E09A84";
        public DbSet<IdentityUser> _AspNetUsers { get; set; }
        public DbSet<BCPUser> _BCPUsers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassAllocation> ClassAllocations { get; set; }
        public DbSet<Recognizer> Recognizers { get; set; }
        public DbSet<RecognizerTask> RecognizerTasks { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<AttendanceItem> AttendanceItems { get; set; }
        public DbSet<SiteConfig> SiteConfigs { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<GroupImage> GroupImages { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<BCPUser>()
                .HasIndex(e => e.Email)
                .IsUnique(true);
            builder.Entity<Class>()
                .HasIndex(e => e.ClassCode)
                .IsUnique(true);
            builder.Entity<IdentityRole>()
                .HasData(
                new IdentityRole { Id = LecturerGuid, Name = "LECTURER", NormalizedName = "LECTURER" },
                new IdentityRole { Id = AdminGuid, Name = "ADMIN", NormalizedName = "ADMIN" });
            builder.Entity<SiteConfig>()
                .HasData(
                new SiteConfig { Key = "SITEURL", Value = "https://bcp.amazecraft.net" },
                new SiteConfig { Key = "PERSONGROUP", Value = "bebc3187-603c-4f85-8e33-7f60b148458d" },
                new SiteConfig { Key = "NUM_PHOTO_PER_STUDENT", Value = "3" });
                //new SiteConfig { Key = "NUM_PHOTO_PER_CLASS", Value = "5" });
        }
    }
}
