using System;
using HabitTracker.Core.Models.Domain;

namespace HabitTracker.Core.Models.Response;

public class HabitsResponse
{
    public required IEnumerable<HabitResponse> Items { get; init; } = Enumerable.Empty<HabitResponse>();
}
