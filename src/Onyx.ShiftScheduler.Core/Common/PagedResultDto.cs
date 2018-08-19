using System;
using System.Collections.Generic;

namespace Onyx.ShiftScheduler.Core.Common
{
    [Serializable]
    public class PagedResultDto<T> : ListResultDto<T>
    {
        public PagedResultDto()
        {
        }

        public PagedResultDto(int totalCount, IReadOnlyList<T> items)
            : base(items)
        {
            TotalCount = totalCount;
        }

        public int TotalCount { get; set; }
    }
}