using HabitTracker.Core.Models.Domain;
using HabitTracker.Core.Models.Request;
using HabitTracker.Core.Models.Response;
using HabitTracker.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HabitTrackerController : ControllerBase
    {

        private readonly HabitTrackerDbContext _context;
        private readonly StreakCalculationService _streakService;

        public HabitTrackerController(
            HabitTrackerDbContext context,
            StreakCalculationService streakService)
        {
            _context = context;
            _streakService = streakService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HabitResponse>>> GetHabits()
        {
            var currentDate = DateTime.UtcNow.Date;
            var habits = await _context.Habits
                .Include(h => h.Streaks)
                .Select(h => new HabitResponse
                {
                    Id = h.Id,
                    Name = h.Name,
                    Description = h.Description,
                    Streak = _streakService.CalculateCurrentStreak(
                        h.Streaks.Select(s => s.HabitCompletedDate),
                        currentDate
                    )
                })
                .ToListAsync();

            return Ok(habits);
        }

        [HttpPost]
        public async Task<ActionResult<HabitResponse>> CreateHabit(CreateHabitRequest request)
        {
            var habit = new Habit
            {
                Name = request.Name,
                Description = request.Description,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _context.Habits.Add(habit);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetHabits),
                new HabitResponse
                {
                    Id = habit.Id,
                    Name = habit.Name,
                    Description = habit.Description,
                    Streak = 0
                });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateHabitStreak(int id, UpdateHabitStreakRequest request)
        {
            var habit = await _context.Habits
                .Include(h => h.Streaks)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (habit == null)
                return NotFound();

            if (request.StreakCompletedDate.HasValue)
            {
                var streak = new Streak
                {
                    HabitId = habit.Id,
                    HabitCompletedDate = request.StreakCompletedDate.Value.Date,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Streaks.Add(streak);
            }

            habit.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHabit(int id, UpdateHabitRequest request)
        {
            var habit = await _context.Habits.FindAsync(id);

            if (habit == null)
                return NotFound();

            if (request.Name != null)
                habit.Name = request.Name;

            if (request.Description != null)
                habit.Description = request.Description;

            habit.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHabit(int id)
        {
            var habit = await _context.Habits.FindAsync(id);

            if (habit == null)
                return NotFound();

            _context.Habits.Remove(habit);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
