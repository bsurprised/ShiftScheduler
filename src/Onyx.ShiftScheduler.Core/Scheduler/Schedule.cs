using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Onyx.ShiftScheduler.Core.Common;

namespace Onyx.ShiftScheduler.Core.Scheduler
{
    public class Schedule : Entity
    {
        [Required]
        [StringLength(ObjectAttributeConsts.NameLength100, MinimumLength = ObjectAttributeConsts.StringMinLength3)]
        public string Name { get; set; }

        [Required] public DateTime StartDate { get; set; }

        [Required] public DateTime EndDate { get; set; }

        public IList<Shift> Shifts { get; set; }

        [NotMapped] public int Days => (EndDate - StartDate).Days;
    }
}