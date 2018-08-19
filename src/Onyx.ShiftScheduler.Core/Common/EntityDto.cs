using System;
using Onyx.ShiftScheduler.Core.Interfaces;

namespace Onyx.ShiftScheduler.Core.Common
{
    [Serializable]
    public class EntityDto : EntityDto<int?>, IEntityDto
    {
        public EntityDto()
        {
        }

        public EntityDto(int? id)
            : base(id)
        {
        }
    }

    [Serializable]
    public class EntityDto<TPrimaryKey> : IEntityDto<TPrimaryKey>
    {
        public EntityDto()
        {
        }

        public EntityDto(TPrimaryKey id)
        {
            Id = id;
        }

        public TPrimaryKey Id { get; set; }
    }
}