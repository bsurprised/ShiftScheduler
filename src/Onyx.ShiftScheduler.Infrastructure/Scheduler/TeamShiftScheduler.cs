﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.OrTools.ConstraintSolver;
using Onyx.ShiftScheduler.Core.App;
using Onyx.ShiftScheduler.Core.Scheduler;

namespace Onyx.ShiftScheduler.Infrastructure.Scheduler
{
    /// <summary>
    ///     The shift scheduler uses Google ConstraintSolver in OR tools package to find a solution
    ///     based on model constraints.
    ///     Remember that this demo model does not check for invalid input,
    ///     so if invalid number of teams or days is used, the solver may never
    ///     find a solution.
    /// </summary>
    public class TeamShiftScheduler
    {
        /// <summary>
        ///     A helper for solver statistics that can be used for debug purposes.
        /// </summary>
        public string StatisticalResults { get; private set; }

        /// <summary>
        ///     Get a new schedule based on number of teams and days.
        ///     This is a default 24/7 shift design based on 12 hour day/night shifts
        ///     Rules:
        ///     - One shift in a day for every employee
        ///     - Two days off after 2 sequential date on the job
        ///     - No two night shifts in a row
        ///     - At least two shifts in a cycle for every employee
        /// </summary>
        /// <param name="shiftEmployees">Participating employees for the shifts</param>
        /// <param name="startDate">Schedule start date</param>
        /// <param name="numberOfDays">Number of days in each cycle</param>
        /// <param name="teamSize">Number of employees in each team</param>
        /// <param name="minShiftsPerCycle">Required number of shifts per cycle</param>
        /// <param name="startHour">Starting hour of the day shift</param>
        /// <param name="shiftHours">Hours in every shift, up to 12</param>
        /// <param name="maxSolutions">Max returned results</param>
        /// <returns></returns>
        public Task<List<Schedule>> CreateNewScheduleAsync(
            IList<Employee> shiftEmployees,
            DateTime startDate,
            int numberOfDays,
            int teamSize = 2,
            int minShiftsPerCycle = 2,
            int startHour = 7,
            int shiftHours = 12,
            int maxSolutions = 1)
        {
            // Some sanity checks
            if (shiftEmployees == null || shiftEmployees.Count < 1)
                throw new ArgumentOutOfRangeException($"Employee collection is empty.");
            if (numberOfDays < 1)
                throw new ArgumentOutOfRangeException($"Invalid number for days in a cycle.");
            if (teamSize < 1)
                throw new ArgumentOutOfRangeException($"Invalid number for employees in each shift.");
            if (minShiftsPerCycle < 0)
                throw new ArgumentOutOfRangeException($"Invalid number for minimum shifts per cycle.");
            if (startHour > 12)
                throw new ArgumentOutOfRangeException(
                    $"Starting hour is bigger than expected. Please provide a number between 0-12");
            if (shiftHours > 12)
                throw new ArgumentOutOfRangeException(
                    $"Shift hours cannot be bigger than twelve. Please provide a number between 1-12");

            var numberOfEmployees = shiftEmployees.Count;

            AddDiagnostics("Starting to solve a new schedule\n\n");
            AddDiagnostics("This is a schedule for {0} employees in {1} days\n", numberOfEmployees, numberOfDays);
            AddDiagnostics("Shift team size: {0}, minimum shifts per employee: {1}\n\n", teamSize, minShiftsPerCycle);

            /*
             * Solver
             */

            // Initiate a new solver
            var solver = new Solver("Schedule");

            int[] shifts = {ShiftConsts.None, ShiftConsts.Day, ShiftConsts.Night, ShiftConsts.Off};
            int[] validShifts = {ShiftConsts.Day, ShiftConsts.Night, ShiftConsts.Off};

            /*
             * DFA and Transitions
             */
            var initialState = 1; // Everybody starts at state 1
            int[] acceptingStates = {1, 2, 3, 4, 5};

            // Transition tuples For TransitionConstraint
            var transitionTuples = new IntTupleSet(3);
            // Every tuple contains { state, input, next state }
            transitionTuples.InsertAll(new[,]
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
                /* State 4 - only and off shift and continuing to another off */
                {4, 3, 5}, // o --> 5
                /* State 5 - another off shift and reset the cycle */
                {5, 3, 1} // o --> 1
            });

            // Just for presentation in stats
            string[] days = {"d", "n", "o"};

            /*
             * Decision variables
             */

            // TransitionConstraint
            var x =
                solver.MakeIntVarMatrix(numberOfEmployees, numberOfDays, validShifts, "x");

            var flattenedX = x.Flatten();

            // Shift count
            var shiftCount = shifts.Length;

            // Shifts per day statistics
            var dayStats = new IntVar[numberOfDays, shiftCount];
            for (var i = 0; i < numberOfDays; i++)
            for (var j = 0; j < shiftCount; j++)
                dayStats[i, j] = solver.MakeIntVar(0, numberOfEmployees, "dayStats");

            // Team statistics
            var teamStats = new IntVar[numberOfEmployees];

            /*
             * Constraints
             */
            for (var i = 0; i < numberOfEmployees; i++)
            {
                var regInput = new IntVar[numberOfDays];
                for (var j = 0; j < numberOfDays; j++)
                    regInput[j] = x[i, j];

                solver.Add(regInput.Transition(transitionTuples, initialState, acceptingStates));
            }

            // Statistics and constraints for each team
            for (var team = 0; team < numberOfEmployees; team++)
            {
                // Number of worked days (either day or night shift)
                var teamDays = new IntVar[numberOfDays];
                for (var day = 0; day < numberOfDays; day++)
                    teamDays[day] = x[team, day].IsMember(new[] {ShiftConsts.Day, ShiftConsts.Night});

                teamStats[team] = teamDays.Sum().Var();

                // At least two shifts per cycle
                solver.Add(teamStats[team] >= minShiftsPerCycle);
            }

            // Statistics and constraints for each day
            for (var day = 0; day < numberOfDays; day++)
            {
                var teams = new IntVar[numberOfEmployees];
                for (var team = 0; team < numberOfEmployees; team++)
                    teams[team] = x[team, day];

                var stats = new IntVar[shiftCount];
                for (var shift = 0; shift < shiftCount; ++shift)
                    stats[shift] = dayStats[day, shift];

                solver.Add(teams.Distribute(stats));

                // Constraints for each day

                // - exactly 2 on day shift
                solver.Add(dayStats[day, ShiftConsts.Day] == teamSize);
                // - exactly 2 on night shift
                solver.Add(dayStats[day, ShiftConsts.Night] == teamSize);
                // - The rest of the employees are off duty
                solver.Add(dayStats[day, ShiftConsts.Off] == numberOfEmployees - teamSize * 2);

                /*
                 * Note: We can customize constraints even further
                 *
                 * For example, a special constraints for weekends (1 employee each shift as weekends are quiet):
                 * if (day % 7 == 5 || day % 7 == 6)
                 * {
                 *     solver.Add(dayStats[day, ShiftType.Day] == 1);
                 *     solver.Add(dayStats[day, ShiftType.Night] == 1);
                 *     solver.Add(dayStats[day, ShiftType.Off] == numberOfEmployees - 2);
                 * }
                 *
                */
            }

            /*
             * Decision Builder and Solution Search
             */

            // A simple random selection
            var db = solver.MakePhase(flattenedX, Solver.CHOOSE_DYNAMIC_GLOBAL_BEST, Solver.ASSIGN_RANDOM_VALUE);

            var log = solver.MakeSearchLog(1000000);

            // Start the search
            solver.NewSearch(db, log);

            // Return solutions as result
            var schedules = new List<Schedule>();

            var numSolutions = 0;
            while (solver.NextSolution())
            {
                numSolutions++;

                // A new schedule for the time period
                var schedule = new Schedule
                {
                    Id = numSolutions,
                    Name = string.Format("Schedule for {0}-{1} for {2} employees, team size {3}",
                        startDate.Date.ToShortDateString(), startDate.Date.AddDays(numberOfDays).ToShortDateString(),
                        numberOfEmployees, teamSize),
                    StartDate = startDate.Date,
                    EndDate = startDate.Date.AddDays(numberOfDays),
                    Shifts = new List<Shift>()
                };

                var idCounter = 1;
                for (var i = 0; i < numberOfEmployees; i++)
                {
                    AddDiagnostics("Employee #{0,-2}: ", i + 1, shiftEmployees[i].ToString());

                    var occ = new Dictionary<int, int>();
                    for (var j = 0; j < numberOfDays; j++)
                    {
                        var shiftVal = (int) x[i, j].Value() - 1;
                        if (!occ.ContainsKey(shiftVal)) occ[shiftVal] = 0;
                        occ[shiftVal]++;

                        // Add a shift
                        var shiftType = (ShiftType) shiftVal + 1;
                        var shiftStart = startDate.Date
                            .AddDays(j)
                            .AddHours(shiftType == ShiftType.Off
                                ? 0
                                : (shiftType == ShiftType.Day
                                    ? startHour
                                    : startHour + shiftHours)); // i.e Day shift starts at 07:00, night shift at 19:00

                        schedule.Shifts.Add(new Shift
                        {
                            Id = idCounter,
                            Employee = shiftEmployees[i],
                            Type = shiftType,
                            StartDate = shiftStart,
                            EndDate = shiftType == ShiftType.Off ? shiftStart : shiftStart.AddHours(shiftHours)
                        });
                        idCounter++;
                        AddDiagnostics(days[shiftVal] + " ");
                    }

                    AddDiagnostics(" #Total days: {0,2}", teamStats[i].Value());
                    foreach (var s in validShifts)
                    {
                        var v = 0;
                        if (occ.ContainsKey(s - 1)) v = occ[s - 1];
                        AddDiagnostics("  {0}:{1}", days[s - 1], v);
                    }

                    AddDiagnostics("\t- {0}\n", shiftEmployees[i].ToString());
                }

                AddDiagnostics("\n");

                AddDiagnostics("Daily Statistics\nDay\t\td n o\n");
                for (var j = 0; j < numberOfDays; j++)
                {
                    AddDiagnostics("Day #{0,2}: \t", j + 1);
                    foreach (var t in validShifts) AddDiagnostics(dayStats[j, t].Value() + " ");
                    AddDiagnostics("\n");
                }

                AddDiagnostics("\n");

                // Add this schedule to list
                schedules.Add(schedule);

                // defaults to just the first one
                if (numSolutions >= maxSolutions)
                    break;
            }

            AddDiagnostics("\nSolutions: {0}", solver.Solutions());
            AddDiagnostics("\nFailures: {0}", solver.Failures());
            AddDiagnostics("\nBranches: {0} ", solver.Branches());
            AddDiagnostics("\nWallTime: {0}ms", solver.WallTime());

            solver.EndSearch();

            AddDiagnostics("\n\nFinished solving the schedule.");

            return Task.FromResult(schedules);
        }

        private void AddDiagnostics(string format, params object[] arg)
        {
            StatisticalResults += string.Format(format, arg);
        }
    }
}