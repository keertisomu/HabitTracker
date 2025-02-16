using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Core.Models.Domain
{
    public class Streak
    {
        public int Id { get; set; }
        public int HabitId { get; set; }
        public DateTime HabitCompletedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation property
        public Habit Habit { get; set; }
    }
}
