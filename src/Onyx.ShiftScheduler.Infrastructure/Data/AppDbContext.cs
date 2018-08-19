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

        #region Db Demo Initializer

        public void Initialize()
        {
            // Init DB
            Database.EnsureCreated();

            // Seed data
            if (!Employees.Any())
            {
                Employees.AddRange(GetEmployees());
                SaveChanges();
            }
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
                // Inactive, play at your discretion
                new Employee {Name = "Paulene", FamilyName = "Berube", IsActive = false},
                new Employee {Name = "Rich", FamilyName = "Vallecillo", IsActive = false},
                new Employee {Name = "Shanell", FamilyName = "Lasala", IsActive = false},
                new Employee {Name = "Hassan", FamilyName = "Mcclendon", IsActive = false},
                new Employee {Name = "Roselle", FamilyName = "Larose", IsActive = false},
                new Employee {Name = "Marylouise", FamilyName = "Santerre", IsActive = false},
                new Employee {Name = "Milan", FamilyName = "Wimmer", IsActive = false},
                new Employee {Name = "Christy", FamilyName = "Lindgren", IsActive = false},
                new Employee {Name = "Lanie", FamilyName = "Stegner", IsActive = false},
                new Employee {Name = "Katina", FamilyName = "Hammers", IsActive = false},
                new Employee {Name = "Lekisha", FamilyName = "Janelle", IsActive = false},
                new Employee {Name = "Shane", FamilyName = "Schatz", IsActive = false},
                new Employee {Name = "Lorena", FamilyName = "Hanneman", IsActive = false},
                new Employee {Name = "Chad", FamilyName = "Westerlund", IsActive = false},
                new Employee {Name = "Elmira", FamilyName = "Vanbuskirk", IsActive = false},
                new Employee {Name = "Nerissa", FamilyName = "Montville", IsActive = false},
                new Employee {Name = "Tomasa", FamilyName = "Holst", IsActive = false},
                new Employee {Name = "Zachery", FamilyName = "Brinkmann", IsActive = false},
                new Employee {Name = "Jere", FamilyName = "Salo", IsActive = false},
                new Employee {Name = "Erna", FamilyName = "Hom", IsActive = false}
            };
        }

        #endregion
    }
}