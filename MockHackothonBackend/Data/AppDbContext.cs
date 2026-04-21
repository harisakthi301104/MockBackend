using Microsoft.EntityFrameworkCore;
using MockHackothonBackend.Model;
using MockHackothonBackend.Models;
using System;

namespace MockHackothonBackend.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Need> Needs { get; set; }
        public DbSet<Commitment> Commitments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            });

            // Need configuration
            modelBuilder.Entity<Need>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(300);
                entity.Property(e => e.RequiredVolunteers).IsRequired();
                entity.Property(e => e.CurrentVolunteersCount).HasDefaultValue(0);
                entity.Property(e => e.Status).HasDefaultValue(Enums.NeedStatus.Open);

                entity.HasOne(e => e.Organization)
                    .WithMany(u => u.CreatedNeeds)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Commitment configuration
            modelBuilder.Entity<Commitment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Commitments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Need)
                    .WithMany(n => n.Commitments)
                    .HasForeignKey(e => e.NeedId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint: user can commit to a need only once
                entity.HasIndex(e => new { e.UserId, e.NeedId }).IsUnique();
            });
        }
    }
}
