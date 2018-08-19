using System;
using Onyx.ShiftScheduler.Core.Common;

namespace Onyx.ShiftScheduler.Core.Scheduler.Dto
{
    public class ShiftDto : EntityDto
    {
        public string Employee { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ShiftType Type { get; set; }

        public static ShiftDto FromEntity(Shift input)
        {
            if (input == null)
                return null;

            return new ShiftDto
            {
                Id = input.Id,
                Employee = input.Employee.ToString(),
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                Type = input.Type
            };
        }
    }
}