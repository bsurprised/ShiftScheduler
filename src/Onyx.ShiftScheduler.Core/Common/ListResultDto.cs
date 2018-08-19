using System;
using System.Collections.Generic;

namespace Onyx.ShiftScheduler.Core.Common
{
    [Serializable]
    public class ListResultDto<T>
    {
        private IReadOnlyList<T> _items;

        public ListResultDto()
        {
        }

        public ListResultDto(IReadOnlyList<T> items)
        {
            Items = items;
        }

        public IReadOnlyList<T> Items
        {
            get => _items ?? (_items = new List<T>());
            set => _items = value;
        }
    }
}