using System;
using System.ComponentModel.DataAnnotations;
using Onyx.ShiftScheduler.Core.App;
using Onyx.ShiftScheduler.Core.Common;

namespace Onyx.ShiftScheduler.Core.Scheduler
{
    public class Shift : Entity
    {
        [Required] public Employee Employee { get; set; }

        [Required] public DateTime StartDate { get; set; }

        [Required] public DateTime EndDate { get; set; }

        [Required] public ShiftType Type { get; set; }
    }
}