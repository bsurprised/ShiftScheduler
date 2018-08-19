namespace Onyx.ShiftScheduler.Core.Interfaces
{
    public interface IEntityDto : IEntityDto<int?>
    {
        // Default to Guid
    }

    public interface IEntityDto<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}