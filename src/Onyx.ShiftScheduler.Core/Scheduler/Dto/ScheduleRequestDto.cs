using System;
using System.Collections.Generic;
using System.Text;

namespace Onyx.ShiftScheduler.Core.Scheduler.Dto
{
    public class ScheduleRequestDto
    {
        /// <summary>
        /// The id for transition set of rules
        /// </summary>
        public int TransitionSetId { get; set; }

        /// <summary>
        /// First day of the schedule
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Number of days in a cycle
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Number of employees for the schedule
        /// </summary>
        public int NumberOfEmployees { get; set; }

        /// <summary>
        /// Number of employees in a team for each shift
        /// </summary>
        public int TeamSize { get; set; }

        /// <summary>
        /// Minimum number of shifts for each employee in a cycle
        /// </summary>
        public int MinShiftsPerCycle { get; set; }

        /// <summary>
        /// Starting hour of the day shift
        /// </summary>
        public int StartHour { get; set; }

        /// <summary>
        /// Hours in every shift, up to 12
        /// </summary>
        public int ShiftHours { get; set; }

        public ScheduleRequestDto()
        {
            StartDate = DateTime.Now.Date;
        }
    }
}
