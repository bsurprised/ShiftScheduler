using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Onyx.ShiftScheduler.Core.App;
using Onyx.ShiftScheduler.Core.App.Dto;
using Onyx.ShiftScheduler.Core.Interfaces;
using Onyx.ShiftScheduler.Core.Scheduler.Dto;
using Onyx.ShiftScheduler.Infrastructure.Scheduler;

namespace Onyx.ShiftScheduler.Infrastructure.Services
{
    public class SchedulerService
    {
        private readonly IRepository<Employee, int> _employeeRepository;
        private readonly IRepository<TransitionSet, int> _transitionSetRepository;
        private readonly TeamShiftScheduler _teamShiftScheduler;

        public SchedulerService(
            TeamShiftScheduler teamShiftScheduler,
            IRepository<Employee, int> employeeRepository,
            IRepository<TransitionSet, int> transitionSetRepository
        )
        {
            _teamShiftScheduler = teamShiftScheduler;
            _employeeRepository = employeeRepository;
            _transitionSetRepository = transitionSetRepository;
        }

        /// <summary>
        ///     Create a new schedule from a given day.
        ///     Some demo settings are in place here.
        /// </summary>
        /// <param name="scheduleRequest">Parameters for the search</param>        
        /// <returns>Scheduled shifts</returns>
        public async Task<ScheduleDto> GetNewScheduleAsync(ScheduleRequestDto scheduleRequest)
        {
            // Get team members from repository
            var teamMembers = await _employeeRepository.QueryNoTracking()
                .Where(w => w.IsActive) // Active ones for next schedule
                .OrderBy(c => new Random().Next()) // Shuffle staff order
                .Take(scheduleRequest.NumberOfEmployees) // we take a number from all employees in demo
                .ToListAsync();

            // Get transition set rules from repository
            var transitionSet = await _transitionSetRepository.QueryNoTracking()
                .Where(w => w.Id == scheduleRequest.TransitionSetId && w.IsActive) // active transition set
                .FirstOrDefaultAsync();

            // We only have that number of employees
            if (teamMembers.Count < scheduleRequest.NumberOfEmployees)
                throw new ArgumentOutOfRangeException($"Invalid number of employees for scheduling.");

            // Check we have the active transition set
            if (transitionSet == null)
                throw new ArgumentOutOfRangeException($"Invalid transition set defined.");

            var ruleSet = TransitionSet.FromRuleSetString(transitionSet.Name, transitionSet.RuleSetString).RuleSet;

            // Take 5 solutions as a demo result
            var schedules = await _teamShiftScheduler.CreateNewScheduleAsync(ruleSet, teamMembers, scheduleRequest.StartDate.Date, 
                scheduleRequest.Days, scheduleRequest.TeamSize, scheduleRequest.MinShiftsPerCycle, 
                scheduleRequest.StartHour, scheduleRequest.ShiftHours, 5);

            // Play with results for demo
            if (schedules.Count > 0)
                return ScheduleDto.FromEntity(schedules[new Random().Next(schedules.Count)], GetLatestStatistics());

            // Return default with statistics on model error
            return new ScheduleDto() { Statistics = GetLatestStatistics(), Error = GetLatestError() };

            // Or just return first.
            // return ScheduleToDto(schedules.FirstOrDefault());
        }

        /// <summary>
        /// Get all active transition sets
        /// </summary>
        /// <returns>List of transition sets</returns>
        public async Task<List<TransitionSetDto>> GetActiveTransitionSetsAsync()
        {
            // Get transition sets from repository
            var transitionSets = await _transitionSetRepository.GetAllListAsync(w => w.IsActive);
            
            return transitionSets.Select(TransitionSetDto.FromEntity).ToList();
        }

        /// <summary>
        ///     Just for the sake of debugging, a demo presentation of results
        /// </summary>
        /// <returns>string statistics</returns>
        public string GetLatestStatistics()
        {
            return _teamShiftScheduler.StatisticalResults;
        }

        /// <summary>
        ///     The last model error if solving the model was unsuccessful.
        /// </summary>
        /// <returns>string error</returns>
        public string GetLatestError()
        {
            return _teamShiftScheduler.LastError;
        }
    }
}