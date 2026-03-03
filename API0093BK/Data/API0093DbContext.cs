using API0093BK.Models;
using API0093BK.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API0093BK.Data
{
    public class API0093DbContext : DbContext
    {
        public API0093DbContext(DbContextOptions<API0093DbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Wish> Wishes { get; set; }                    // Вместо Requests
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<EmployeeCourse> EmployeeCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Users ===
            modelBuilder.Entity<User>()
                .HasIndex(u => u.EmployeeNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // === Wishes ===
            modelBuilder.Entity<Wish>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishes)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Wish>()
                .HasIndex(w => new { w.UserId, w.RequestedDate })
                .IsUnique();

            // === Schedules ===
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

            // === Courses ===
            modelBuilder.Entity<Course>()
                .HasIndex(c => c.ExternalId)
                .IsUnique();

            // === EmployeeCourses ===
            modelBuilder.Entity<EmployeeCourse>()
                .HasOne(ec => ec.User)
                .WithMany(u => u.EmployeeCourses)
                .HasForeignKey(ec => ec.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeCourse>()
                .HasOne(ec => ec.Course)
                .WithMany(c => c.EmployeeCourses)
                .HasForeignKey(ec => ec.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeCourse>()
                .HasIndex(ec => new { ec.UserId, ec.CourseId })
                .IsUnique();

            // Seed initial admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    EmployeeNumber = "ADMIN001",
                    FullName = "System Administrator",
                    Email = "admin@burgerking.ru",
                    PasswordHash = PasswordHelper.HashPassword("Admin123!"),
                    Role = UserRoles.Administrator,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                }
            );
        }
    }
}