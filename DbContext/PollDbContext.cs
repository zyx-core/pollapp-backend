using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PollApp.Backend.Models;
using System;

namespace PollApp.Backend.DbContexts
{
    public class PollDbContext : DbContext
    {
        public PollDbContext(DbContextOptions<PollDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Poll> Polls { get; set; } = null!;
        public DbSet<PollOption> PollOptions { get; set; } = null!;
        public DbSet<Vote> Votes { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enforce Unique Vote constraint: One user can vote only once per poll
            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.UserId, v.PollId })
                .IsUnique();

            // Configure Delete Behavior
            modelBuilder.Entity<PollOption>()
                .HasOne(po => po.Poll)
                .WithMany(p => p.Options)
                .HasForeignKey(po => po.PollId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Poll)
                .WithMany(p => p.Votes)
                .HasForeignKey(v => v.PollId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.PollOption)
                .WithMany()
                .HasForeignKey(v => v.PollOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Poll)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PollId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Poll>()
                .HasOne(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Admin User
            var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var adminUser = new User
            {
                Id = adminId,
                Name = "Admin",
                Email = "admin@pollapp.com",
                IsActive = true,
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            var passwordHasher = new PasswordHasher<User>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

            modelBuilder.Entity<User>().HasData(adminUser);
        }
    }
}
