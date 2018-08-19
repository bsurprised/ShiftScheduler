using System;

namespace Onyx.ShiftScheduler.Core.Interfaces
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletionTime { get; set; }
    }
}