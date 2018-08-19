using Onyx.ShiftScheduler.Core.Common;

namespace Onyx.ShiftScheduler.Core.App.Dto
{
    public class EmployeeDto : EntityDto
    {
        public string Name { get; set; }

        public string FamilyName { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, FamilyName);
        }
    }
}