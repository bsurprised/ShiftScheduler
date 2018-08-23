using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Onyx.ShiftScheduler.Core.App;

namespace Onyx.ShiftScheduler.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<TransitionSet> TransitionSets { get; set; }

        #region Db Demo Initializer

        public void Initialize()
        {
            // Init DB
            Database.EnsureCreated();

            // Seed data
            if (!Employees.Any())
                Employees.AddRange(GetEmployees());

            if (!TransitionSets.Any())
                TransitionSets.AddRange(GetDefaultTransitionSets());

            SaveChanges();
        }

        /* You can build any transitions based on your business and add them to database,
           Then use them later in your schedules as rules */
        public static List<TransitionSet> GetDefaultTransitionSets()
        {
            return new List<TransitionSet> {
                TransitionSet.FromRuleSet("Two shifts, 2 days off; No 2 nights in a row",
                new RuleSet()
                {
                    InitialState = 1,
                    AcceptingStates = new int[] { 1, 2, 3, 4, 5 },
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
                }),
                TransitionSet.FromRuleSet("Three shifts, 1 day off; No 3 nights in a row",
                    new RuleSet()
                    {
                        InitialState = 1,
                        AcceptingStates = new int[] { 1, 2, 3, 4, 5, 6 },
                        Tuples = new[,]
                        {
                            /* State 1 - either a day shift, a night shift or an off shift */
                            {1, 1, 2}, // d --> 2
                            {1, 2, 3}, // n --> 3
                            {1, 3, 1}, // o --> 1
                            /* State 2 - if day and night shift, go to 4 */
                            {2, 1, 4}, // d --> 4
                            {2, 2, 4}, // n --> 4
                            {2, 3, 1}, // o --> 1
                            /* State 3 - either a day shift, a night shift or an off shift */
                            {3, 1, 4}, // d --> 4
                            {3, 2, 5}, // n --> 5
                            {3, 3, 1}, // o --> 1
                            /* State 4 - any shift will go for an off day in 6 */
                            {4, 1, 6}, // d --> 6
                            {4, 2, 6}, // n --> 6
                            {4, 3, 1}, // o --> 1
                            /* State 5 - no night shift if had two night shift before */
                            {5, 1, 6}, // d --> 6
                            {5, 3, 1}, // o --> 1
                            /* State 6 - only an off shift and reset the cycle */
                            {6, 3, 1} // o --> 1
                        }
                    })
            };
        }

        public static List<Employee> GetEmployees()
        {
            // 30 demo employees
            return new List<Employee>
            {
                new Employee {Name = "Wanda", FamilyName = "Morgan", IsActive = true},
                new Employee {Name = "Jonathan", FamilyName = "Hudson", IsActive = true},
                new Employee {Name = "Jessica", FamilyName = "Sharp", IsActive = true},
                new Employee {Name = "Colin", FamilyName = "Cornish", IsActive = true},
                new Employee {Name = "Sue", FamilyName = "Wilson", IsActive = true},
                new Employee {Name = "Richard", FamilyName = "Buckland", IsActive = true},
                new Employee {Name = "Joseph", FamilyName = "Lee", IsActive = true},
                new Employee {Name = "Alison", FamilyName = "Chapman", IsActive = true},
                new Employee {Name = "Nathan", FamilyName = "Carr", IsActive = true},
                new Employee {Name = "Jan", FamilyName = "Martin", IsActive = true},
                new Employee {Name = "Paulene", FamilyName = "Berube", IsActive = true},
                new Employee {Name = "Rich", FamilyName = "Vallecillo", IsActive = true},
                new Employee {Name = "Shanell", FamilyName = "Lasala", IsActive = true},
                new Employee {Name = "Hassan", FamilyName = "Mcclendon", IsActive = true},
                new Employee {Name = "Roselle", FamilyName = "Larose", IsActive = true},
                new Employee {Name = "Marylouise", FamilyName = "Santerre", IsActive = true},
                new Employee {Name = "Milan", FamilyName = "Wimmer", IsActive = true},
                new Employee {Name = "Christy", FamilyName = "Lindgren", IsActive = true},
                new Employee {Name = "Lanie", FamilyName = "Stegner", IsActive = true},
                new Employee {Name = "Katina", FamilyName = "Hammers", IsActive = true},
                new Employee {Name = "Lekisha", FamilyName = "Janelle", IsActive = true},
                new Employee {Name = "Shane", FamilyName = "Schatz", IsActive = true},
                new Employee {Name = "Lorena", FamilyName = "Hanneman", IsActive = true},
                new Employee {Name = "Chad", FamilyName = "Westerlund", IsActive = true},
                new Employee {Name = "Elmira", FamilyName = "Vanbuskirk", IsActive = true},
                new Employee {Name = "Nerissa", FamilyName = "Montville", IsActive = true},
                new Employee {Name = "Tomasa", FamilyName = "Holst", IsActive = true},
                new Employee {Name = "Zachery", FamilyName = "Brinkmann", IsActive = true},
                new Employee {Name = "Jere", FamilyName = "Salo", IsActive = true},
                new Employee {Name = "Erna", FamilyName = "Hom", IsActive = true}
            };
        }

        #endregion
    }
}