using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HabitTracker.Core.Models;
using HabitTracker.Core.Models.Domain;
using HabitTracker.Core.Repository;

namespace HabitTracker.Core.Services
{
    public interface IHabitTrackerService
    {
        Task<Habit?> GetHabitAsync(int id);
        Task<IEnumerable<Habit>> GetAllHabitsAsync();
        Task<Habit> CreateHabitAsync(Habit habit);
        Task<Habit?> UpdateHabitAsync(int id, Habit habit);
        Task<bool> UpdateHabitStreakAsync(int habitId, DateTime completedDate);
        Task<bool> DeleteHabitAsync(int id);
    }

    public class HabitTrackerService : IHabitTrackerService
    {
        private readonly IHabitTrackerRepository _habitRepository;
        private readonly IStreakCalculationService _streakCalculationService;

        public HabitTrackerService(
            IHabitTrackerRepository habitRepository,
            IStreakCalculationService streakCalculationService
        )
        {
            _habitRepository =
                habitRepository ?? throw new ArgumentNullException(nameof(habitRepository));
            _streakCalculationService =
                streakCalculationService
                ?? throw new ArgumentNullException(nameof(streakCalculationService));
        }

        public async Task<Habit?> GetHabitAsync(int id)
        {
            return await _habitRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Habit>> GetAllHabitsAsync()
        {
            var habits = await _habitRepository.GetAllAsync();
            // get the habits updated dates from the streaks table
            // use streak calculation service to calculate streaks for each habit
            foreach (var habit in habits)
            {
                var allStreaks = await _habitRepository.GetAllStreaksAsync(habit.Id);
                // get all the habitcompleteddates from allStreaks.
                var satisfiedDates = allStreaks.Select(streak => streak.HabitCompletedDate);
                habit.CurrentStreak = _streakCalculationService.CalculateCurrentStreak(
                    satisfiedDates,
                    DateTime.UtcNow
                );
            }
            return habits;
        }

        public async Task<Habit> CreateHabitAsync(Habit habit)
        {
            if (habit == null)
                throw new ArgumentNullException(nameof(habit));

            return await _habitRepository.CreateAsync(habit);
        }

        public async Task<Habit?> UpdateHabitAsync(int id, Habit habit)
        {
            if (habit == null)
                throw new ArgumentNullException(nameof(habit));

            var existingHabit = await _habitRepository.GetByIdAsync(id);
            if (existingHabit == null)
                return null;

            return await _habitRepository.UpdateAsync(habit);
        }

        public async Task<bool> UpdateHabitStreakAsync(int habitId, DateTime completedDate)
        {
            var habit = await _habitRepository.GetByIdAsync(habitId);
            if (habit == null)
                return false;

            return await _habitRepository.UpdateHabitStreakAsync(habitId, completedDate);
        }

        public Task<bool> DeleteHabitAsync(int id)
        {
            var habit = _habitRepository.GetByIdAsync(id);
            if (habit == null)
                return Task.FromResult(false);

            return _habitRepository.DeleteAsync(id);
        }
    }
}
