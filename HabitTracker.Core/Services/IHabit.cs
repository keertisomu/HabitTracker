using HabitTracker.Core.Models.Request;
using HabitTracker.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Core.Services
{
    public interface IHabit
    {
        Task<IEnumerable<HabitResponse>> GetHabits();
        Task<HabitResponse> CreateHabit(CreateHabitRequest request);
        Task<HabitResponse> UpdateHabit(int id, UpdateHabitRequest request);
    }
}
