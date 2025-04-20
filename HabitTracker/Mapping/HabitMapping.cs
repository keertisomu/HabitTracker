using System;
using HabitTracker.Core.Models.Domain;
using HabitTracker.Core.Models.Request;
using HabitTracker.Core.Models.Response;

namespace HabitTracker.Mapping;

public static class HabitMapping
{
    public static HabitResponse MapToResponse(this Habit habit)
    {
        return new HabitResponse
        {
            Id = habit.Id,
            Name = habit.Name,
            Description = habit.Description,
            Streak = habit.CurrentStreak,
        };
    }

    public static IEnumerable<HabitResponse> MapToResponse(this IEnumerable<Habit> habits)
    {
        return [.. habits.Select(h => h.MapToResponse())];
    }

    public static Habit MapToHabit(this CreateHabitRequest request)
    {
        return new Habit
        {
            Name = request.Name,
            Description = request.Description,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };
    }

    public static Habit MapToHabit(this UpdateHabitRequest request, int id)
    {
        return new Habit
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            UpdatedDate = DateTime.UtcNow,
        };
    }
}
