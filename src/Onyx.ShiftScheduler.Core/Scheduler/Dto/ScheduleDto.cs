using System;
using System.Collections.Generic;
using System.Linq;
using Onyx.ShiftScheduler.Core.Common;

namespace Onyx.ShiftScheduler.Core.Scheduler.Dto
{
    public class ScheduleDto : EntityDto
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IList<ShiftDto> Shifts { get; set; }

        public string Statistics { get; set; }

        public int Days => (EndDate - StartDate).Days;

        public static ScheduleDto FromEntity(Schedule input, string statistics)
        {
            if (input == null)
                return null;

            return new ScheduleDto
            {
                Id = input.Id,
                Name = input.Name,
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                Shifts = input.Shifts.Select(ShiftDto.FromEntity).ToList(),
                Statistics = statistics
            };
        }
    }
}