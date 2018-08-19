using System;

namespace Onyx.ShiftScheduler.Core.Helpers
{
    public static class Utils
    {
        /// <summary>
        ///     Get the next weekday date
        /// </summary>
        /// <param name="startDay">Date to start from</param>
        /// <param name="day">What day</param>
        /// <returns></returns>
        public static DateTime GetNextWeekday(DateTime startDay, DayOfWeek day)
        {
            return startDay.AddDays(((int) day - (int) startDay.DayOfWeek + 7) % 7);
        }
    }
}