using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Onyx.ShiftScheduler.Core.App;
using Onyx.ShiftScheduler.Core.Interfaces;
using Onyx.ShiftScheduler.Core.Scheduler.Dto;
using Onyx.ShiftScheduler.Infrastructure.Scheduler;

namespace Onyx.ShiftScheduler.Infrastructure.Services
{
    public class SchedulerService
    {
        private readonly IRepository<Employee, int> _repository;
        private readonly TeamShiftScheduler _teamShiftScheduler;

        public SchedulerService(
            TeamShiftScheduler teamShiftScheduler,
            IRepository<Employee, int> repository
        )
        {
            _teamShiftScheduler = teamShiftScheduler;
            _repository = repository;
        }

        /// <summary>
        ///     Create a new schedule from a given day.
        ///     Some demo settings are in place here.
        /// </summary>
        /// <param name="startDate">First day of the schedule</param>
        /// <param name="days">Number of days in a cycle</param>
        /// <param name="numberOfEmployees">Number of employees for the schedule</param>
        /// <param name="teamSize">Number of employees in a team for each shift</param>
        /// <param name="minShiftsPerCycle">Minimum number of shifts for each employee in a cycle</param>
        /// <param name="startHour">Starting hour of the day shift</param>
        /// <param name="shiftHours">Hours in every shift, up to 12</param>
        /// <returns>Scheduled shifts</returns>
        public async Task<ScheduleDto> GetNewScheduleAsync(
            DateTime startDate,
            int days,
            int numberOfEmployees,
            int teamSize,
            int minShiftsPerCycle,
            int startHour,
            int shiftHours)
        {
            // Get team members from repository
            var teamMembers = await _repository.QueryNoTracking<Employee>()
                .Where(w => w.IsActive) // Active ones for next schedule
                .OrderBy(c => new Random().Next()) // Shuffle staff order
                .Take(numberOfEmployees)
                .ToListAsync();

            // We only have certain number of employees in this demo
            if (teamMembers.Count < numberOfEmployees)
                throw new ArgumentOutOfRangeException($"Invalid number of employees for scheduling.");

            // Take 5 solutions as a demo result
            var schedules = await _teamShiftScheduler.CreateNewScheduleAsync(teamMembers, startDate.Date, days,
                teamSize, minShiftsPerCycle, startHour, shiftHours, 5);

            // Play with results for demo
            return ScheduleDto.FromEntity(schedules[new Random().Next(1, schedules.Count - 1)], GetLatestStatistics());

            // Or just return first.
            // return ScheduleToDto(schedules.FirstOrDefault());
        }

        /// <summary>
        ///     Just for the sake of debugging, a demo presentation of results
        /// </summary>
        /// <returns>string statistics</returns>
        public string GetLatestStatistics()
        {
            return _teamShiftScheduler.StatisticalResults;
        }
    }
}