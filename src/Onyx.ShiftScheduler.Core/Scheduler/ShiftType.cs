namespace Onyx.ShiftScheduler.Core.Scheduler
{
    /// <summary>
    ///     The type of the shift, can be day, night, or off
    /// </summary>
    public enum ShiftType : byte
    {
        Day = 1,
        Night = 2,
        Off = 3
    }
}