using HabitTracker.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;

namespace HabitTracker
{
    public class HabitTrackerDbContext : DbContext
    {
        public HabitTrackerDbContext(DbContextOptions<HabitTrackerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Habit> Habits { get; set; }
        public DbSet<Streak> Streaks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Habit>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.UpdatedDate)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Streak>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .ValueGeneratedOnAdd();

                entity.HasOne(d => d.Habit)
                      .WithMany(p => p.Streaks)
                      .HasForeignKey(d => d.HabitId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

}
