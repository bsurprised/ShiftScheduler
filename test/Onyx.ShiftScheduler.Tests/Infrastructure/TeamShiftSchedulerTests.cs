using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Onyx.ShiftScheduler.Core.App;
using Onyx.ShiftScheduler.Core.Scheduler;
using Onyx.ShiftScheduler.Infrastructure.Scheduler;
using Xunit;

namespace Onyx.ShiftScheduler.Tests.Infrastructure
{
    public class TeamShiftSchedulerTests
    {
        [Fact]
        public async void CreateNewSchedule_Has_Result()
        {
            // Ref date, Monday August 20, 2018
            var date = new DateTime(2018, 8, 20);

            var teamShiftScheduler = new TeamShiftScheduler();

            var schedules = await teamShiftScheduler.CreateNewScheduleAsync(GetDefaultTransitionSet().RuleSet, GetEmployees(), date, 14, 2, 2, 7, 12, 1);

            // We have one schedule solution in the results
            Assert.Single(schedules);
        }

        [Fact]
        public async void CreateNewSchedule_Has_Statistics()
        {
            // Ref date, Monday August 20, 2018
            var date = new DateTime(2018, 8, 20);

            var teamShiftScheduler = new TeamShiftScheduler();

            var schedules = await teamShiftScheduler.CreateNewScheduleAsync(GetDefaultTransitionSet().RuleSet, GetEmployees(), date, 14, 2, 2, 7, 12, 1);

            // We have statistics for debugging
            Assert.NotEmpty(teamShiftScheduler.StatisticalResults);
        }

        [Fact]
        public async void CreateNewSchedule_Has_Valid_Params()
        {
            // Ref date, Monday August 20, 2018
            var date = new DateTime(2018, 8, 20);

            var teamShiftScheduler = new TeamShiftScheduler();

            var employees = GetEmployees();
            var schedules = await teamShiftScheduler.CreateNewScheduleAsync(GetDefaultTransitionSet().RuleSet, employees, date, 14, 2, 2, 7, 12, 3);

            // We have 3 solutions, dates match and we have 14 days * 10 employee shifts
            Assert.Equal(3, schedules.Count);
            foreach (var schedule in schedules)
            {
                Assert.Equal(14, schedule.Days);
                Assert.Equal(date, schedule.StartDate);
                Assert.Equal(date.AddDays(14), schedule.EndDate);
                Assert.Equal(140, schedule.Shifts.Count);

                foreach (var shift in schedule.Shifts)
                {
                    if (shift.Type == ShiftType.Day)
                        Assert.Equal(7, shift.StartDate.Hour);
                    if (shift.Type == ShiftType.Night)
                        Assert.Equal(19, shift.StartDate.Hour);
                    if (shift.Type == ShiftType.Off)
                        Assert.Equal(0, shift.StartDate.Hour);

                    if (shift.Type == ShiftType.Day)
                        Assert.Equal(19, shift.EndDate.Hour);
                    if (shift.Type == ShiftType.Night)
                        Assert.Equal(7, shift.EndDate.Hour);
                    if (shift.Type == ShiftType.Off)
                        Assert.Equal(0, shift.EndDate.Hour);

                    Assert.Contains(shift.Employee, employees);
                }
            }
        }

        [Fact]
        public async void CreateNewSchedule_Has_Valid_Result()
        {
            // Ref date, Monday August 20, 2018
            var date = new DateTime(2018, 8, 20);
            
            var teamShiftScheduler = new TeamShiftScheduler();

            var employees = GetEmployees();
            var schedules = await teamShiftScheduler.CreateNewScheduleAsync(GetDefaultTransitionSet().RuleSet, employees, date, 14, 2, 2, 7, 12, 3);

            var schedule = schedules.FirstOrDefault();

            Assert.NotNull(schedule);

            var shifts = schedule?.Shifts.ToList();

            Assert.IsType<Schedule>(schedule);
            Assert.IsType<List<Shift>>(shifts);

            // Check the result for rules
            for (int i = 0; i < 14; i++)
            {
                var currentDate = date.AddDays(i);
                var dateShifts = shifts.Where(w => w.StartDate.Date == currentDate).ToList();

                Assert.Equal(10, dateShifts.Count);

                Assert.Equal(2, dateShifts.Count(w => w.Type == ShiftType.Day));
                Assert.Equal(2, dateShifts.Count(w => w.Type == ShiftType.Night));
                Assert.Equal(6, dateShifts.Count(w => w.Type == ShiftType.Off));

                Assert.Equal(10, dateShifts.Select(s => s.Employee).Distinct().Count());
            }

            // And more...
        }

        #region Demo Data

        private List<Employee> GetEmployees()
        {
            // 10 demo employees
            return new List<Employee>()
            {
                new Employee(){ Name = "Wanda", FamilyName = "Morgan" },
                new Employee(){ Name = "Jonathan", FamilyName = "Hudson" },
                new Employee(){ Name = "Jessica", FamilyName = "Sharp" },
                new Employee(){ Name = "Colin", FamilyName = "Cornish" },
                new Employee(){ Name = "Sue", FamilyName = "Wilson" },
                new Employee(){ Name = "Richard", FamilyName = "Buckland" },
                new Employee(){ Name = "Joseph", FamilyName = "Lee" },
                new Employee(){ Name = "Alison", FamilyName = "Chapman" },
                new Employee(){ Name = "Nathan", FamilyName = "Carr" },
                new Employee(){ Name = "Jan", FamilyName = "Martin" }
            };
        }

        public static TransitionSet GetDefaultTransitionSet()
        {
            return
                TransitionSet.FromRuleSet("Two shifts, 2 days off; No 2 nights in a row",
                    new RuleSet()
                    {
                        InitialState = 1,
                        AcceptingStates = new int[] {1, 2, 3, 4, 5},
                        Tuples = new[,]
                        {
                            /* State 1 - either a day shift, a night shift or an off shift */
                            {1, 1, 2}, // d --> 2
                            {1, 2, 3}, // n --> 3
                            {1, 3, 1}, // o --> 1
                            /* State 2 - if day and night shift, go to 4 for two days off */
                            {2, 1, 4}, // d --> 4
                            {2, 2, 4}, // n --> 4
                            {2, 3, 1}, // o --> 1
                            /* State 3 - no night shift if had a night shift before, either day shift or off */
                            {3, 1, 4}, // d --> 4
                            {3, 3, 1}, // o --> 1
                            /* State 4 - only an off shift and continuing to another off */
                            {4, 3, 5}, // o --> 5
                            /* State 5 - another off shift and reset the cycle */
                            {5, 3, 1} // o --> 1
                        }
                    });
        }
        #endregion

    }
}
