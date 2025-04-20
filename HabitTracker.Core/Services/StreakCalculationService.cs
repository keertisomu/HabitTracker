using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Core.Services
{
    public class StreakCalculationService : IStreakCalculationService
    {
        public int CalculateCurrentStreak(
            IEnumerable<DateTime> satisfiedDates,
            DateTime currentDate
        )
        {
            if (satisfiedDates is null || !satisfiedDates.Any())
                return 0;

            var orderedDates = satisfiedDates.OrderByDescending(d => d.Date).ToList();

            // If no entry for today, streak is 0
            if (orderedDates.First().Date != currentDate.Date)
                return 0;

            int streak = 1;
            var previousDate = orderedDates[0].Date;

            for (int i = 1; i < orderedDates.Count; i++)
            {
                var dt = orderedDates[i].Date;

                // If there's a gap larger than 1 day, break the streak
                if ((previousDate - dt).Days != 1)
                    break;

                streak++;
                previousDate = dt;
            }

            return streak;
        }
    }

    // create an inteface for the service
    public interface IStreakCalculationService
    {
        int CalculateCurrentStreak(IEnumerable<DateTime> satisfiedDates, DateTime currentDate);
    }
}
