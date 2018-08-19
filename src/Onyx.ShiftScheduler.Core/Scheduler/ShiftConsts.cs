namespace Onyx.ShiftScheduler.Core.Scheduler
{
    public class ShiftConsts
    {
        public const int None = 0; // A failing state, nothing happens
        public const int Day = 1; // A day shift
        public const int Night = 2; // A night shift
        public const int Off = 3; // An off shift
    }
}