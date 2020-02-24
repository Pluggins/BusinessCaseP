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
        public DbSet<IdentityUser> _AspNetUsers { get; set; }
        public DbSet<BCPUser> _BCPUsers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassAllocation> ClassAllocations { get; set; }
        public DbSet<Recognizer> Recognizers { get; set; }
        public DbSet<RecognizerTask> RecognizerTasks { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<AttendanceItem> AttendanceItems { get; set; }

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
        }
    }
}
