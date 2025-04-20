using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HabitTracker.Core.Models;
using HabitTracker.Core.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Core.Repository
{
    public interface IHabitTrackerRepository
    {
        Task<Habit?> GetByIdAsync(int id);
        Task<IEnumerable<Habit>> GetAllAsync();
        Task<Habit> CreateAsync(Habit habit);
        Task<Habit?> UpdateAsync(Habit habit);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Streak?>> GetAllStreaksAsync(int habitId);
        Task<bool> UpdateHabitStreakAsync(int habitId, DateTime completedDate);
    }

    public class HabitTrackerRepository : IHabitTrackerRepository
    {
        private readonly HabitTrackerDbContext _context;

        public HabitTrackerRepository(HabitTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Habit?> GetByIdAsync(int id)
        {
            var habit = await _context.Habits.FindAsync(id);
            if (habit == null)
            {
                return null;
            }
            return habit;
        }

        public async Task<IEnumerable<Habit>> GetAllAsync()
        {
            return await _context.Habits.ToListAsync();
        }

        public async Task<Habit> CreateAsync(Habit habit)
        {
            _context.Habits.Add(habit);
            await _context.SaveChangesAsync();
            return habit;
        }

        public async Task<Habit?> UpdateAsync(Habit updateHabit)
        {
            var habit = await _context.Habits.FindAsync(updateHabit.Id);
            if (habit == null)
            {
                return null;
            }

            if (updateHabit.Name != null)
                habit.Name = updateHabit.Name;

            if (updateHabit.Description != null)
                habit.Description = updateHabit.Description;

            habit.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return habit;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var habit = await _context.Habits.FindAsync(id);
            if (habit == null)
                return false;

            _context.Habits.Remove(habit);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Streak?>> GetAllStreaksAsync(int habitId)
        {
            var streaks = await _context.Streaks.Where(s => s.HabitId == habitId).ToListAsync();
            if (streaks == null)
            {
                return Enumerable.Empty<Streak?>();
            }
            return streaks;
        }

        public async Task<bool> UpdateHabitStreakAsync(int habitId, DateTime satisfiedDate)
        {
            var streak = new Streak
            {
                HabitId = habitId,
                HabitCompletedDate = satisfiedDate,
                CreatedDate = DateTime.UtcNow,
            };
            _context.Streaks.Add(streak);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
