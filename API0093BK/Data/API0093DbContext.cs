using API0093BK.Helpers;
using API0093BK.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace API0093BK.Data
{
    /// <summary>
    /// Контекст базы данных
    /// </summary>
    public class API0093DbContext : DbContext
    {
        public API0093DbContext(DbContextOptions<API0093DbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Wish> Wishes { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        /// <summary>
        /// Настройка модели базы данных
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка уникальных индексов для User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PortalEmployeeId)
                .IsUnique();

            // Настройка Wish
            modelBuilder.Entity<Wish>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishes)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Wish>()
                .HasIndex(w => new { w.UserId, w.WishDate })
                .IsUnique();

            // Настройка Schedule
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.User)
                .WithMany(u => u.Schedules)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Approver)
                .WithMany()
                .HasForeignKey(s => s.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasIndex(s => new { s.UserId, s.WorkDate })
                .IsUnique();
        }
    }
}