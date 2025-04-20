using HabitTracker.Core;
using HabitTracker.Core.Models.Domain;
using HabitTracker.Core.Models.Request;
using HabitTracker.Core.Models.Response;
using HabitTracker.Core.Services;
using HabitTracker.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HabitTrackerController : ControllerBase
    {
        private readonly IHabitTrackerService _habitService;

        public HabitTrackerController(IHabitTrackerService habitService)
        {
            _habitService = habitService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HabitResponse>>> GetHabits()
        {
            var habits = await _habitService.GetAllHabitsAsync();
            return Ok(habits.MapToResponse());
        }

        [HttpPost]
        public async Task<ActionResult<HabitResponse>> CreateHabit(CreateHabitRequest request)
        {
            var habit = request.MapToHabit();

            var createdHabit = await _habitService.CreateHabitAsync(habit);
            var habitResponse = createdHabit.MapToResponse();

            return CreatedAtAction(nameof(GetHabits), habitResponse);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateHabitStreak(int id, UpdateHabitStreakRequest request)
        {
            var updated = await _habitService.UpdateHabitStreakAsync(
                id,
                request.StreakCompletedDate
            );

            return updated ? NoContent() : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHabit(int id, UpdateHabitRequest request)
        {
            var habit = request.MapToHabit(id);
            var updated = await _habitService.UpdateHabitAsync(id, habit);

            return updated != null ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHabit(int id)
        {
            var deleted = await _habitService.DeleteHabitAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
