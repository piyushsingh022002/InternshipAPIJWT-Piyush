using InternshipAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace InternshipAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Intern> Interns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed some initial data
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    // Pre-computed hash for 'admin123' to avoid dynamic hash generation
                    Password = "$2a$11$ij4DH2hPRFANYwFDwkwQo.qP/6jJ8dMwkH3hXQJfqqZMGFJyM3YpW",
                    Email = "admin@example.com",
                    Role = "HR",
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Fixed date to avoid dynamic value
                }
            );
        }
    }
}
