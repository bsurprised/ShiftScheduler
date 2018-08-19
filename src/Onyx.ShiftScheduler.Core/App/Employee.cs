using System.ComponentModel.DataAnnotations;
using Onyx.ShiftScheduler.Core.Common;
using Onyx.ShiftScheduler.Core.Interfaces;

namespace Onyx.ShiftScheduler.Core.App
{
    public class Employee : Entity, IPassivable
    {
        public Employee()
        {
            IsActive = true;
        }

        [Required]
        [StringLength(ObjectAttributeConsts.NameLength100, MinimumLength = ObjectAttributeConsts.StringMinLength3)]
        public string Name { get; set; }

        [Required]
        [StringLength(ObjectAttributeConsts.NameLength100, MinimumLength = ObjectAttributeConsts.StringMinLength3)]
        public string FamilyName { get; set; }

        public bool IsActive { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, FamilyName);
        }
    }
}