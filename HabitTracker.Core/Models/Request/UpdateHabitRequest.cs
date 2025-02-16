using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Core.Models.Request
{
    public class UpdateHabitRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }       
    }
}
