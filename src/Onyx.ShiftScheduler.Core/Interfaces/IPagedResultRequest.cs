namespace Onyx.ShiftScheduler.Core.Interfaces
{
    public interface IPagedResultRequest
    {
        int SkipCount { get; set; }
        int MaxResultCount { get; set; }
    }
}