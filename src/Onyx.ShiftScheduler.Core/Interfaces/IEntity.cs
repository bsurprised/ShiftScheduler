namespace Onyx.ShiftScheduler.Core.Interfaces
{
    public interface IEntity : IEntity<int>
    {
        // Default to int
    }

    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }

        bool IsTransient();
    }
}