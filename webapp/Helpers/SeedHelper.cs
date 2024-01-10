using Microsoft.EntityFrameworkCore;
using webapp.Models;

namespace webapp.Helpers;

/// <summary>
/// Static helper class for initializing the database
/// </summary>
public static class SeedHelper
{
    /// <summary>
    /// Initializes the database with static values
    /// </summary>
    public static void Seed(ModelBuilder modelBuilder)
    {
        _ = modelBuilder
            .Entity<Role>()
            .HasData(
                new Role
                {
                    Id = 1,
                    Name = "Unassigned",
                    Desc = "Doesn't do stuff"
                },
                new Role
                {
                    Id = 2,
                    Name = "Admin",
                    Desc = "Does stuff"
                },
                new Role
                {
                    Id = 3,
                    Name = "Instructor",
                    Desc = "Does stuff"
                },
                new Role
                {
                    Id = 4,
                    Name = "Executive Director",
                    Desc = "Does stuff"
                },
                new Role
                {
                    Id = 5,
                    Name = "English Office Head",
                    Desc = "Does stuff"
                },
                new Role
                {
                    Id = 6,
                    Name = "English Office Faculty",
                    Desc = "Does stuff"
                },
                new Role
                {
                    Id = 7,
                    Name = "Librarian",
                    Desc = "Does stuff"
                },
                new Role
                {
                    Id = 8,
                    Name = "PBL Coordinator",
                    Desc = "Does stuff"
                },
                new Role
                {
                    Id = 9,
                    Name = "Program Director",
                    Desc = "Does stuff"
                }
            );

        _ = modelBuilder
            .Entity<School>()
            .HasData(
                new School
                {
                    Id = 1,
                    Code = "SoCIT",
                    Name = "School of Computing and Information Technologies"
                },
                new School
                {
                    Id = 2,
                    Code = "SoMA",
                    Name = "School of Multimedia and Arts"
                },
                new School
                {
                    Id = 3,
                    Code = "SoM",
                    Name = "School of Management"
                },
                new School
                {
                    Id = 4,
                    Code = "SoE",
                    Name = "School of Engineering"
                },
                new School
                {
                    Id = 5,
                    Code = "SHS",
                    Name = "Senior High School"
                },
                new School
                {
                    Id = 6,
                    Code = "",
                    Name = "Graduate School"
                }
            );

        _ = modelBuilder
            .Entity<Subject>()
            .HasData(
                new Subject
                {
                    Id = 1,
                    Code = "CSPROJ",
                    Name = ""
                },
                new Subject
                {
                    Id = 2,
                    Code = "PROJMAN",
                    Name = "Project Management"
                },
                new Subject
                {
                    Id = 3,
                    Code = "SOFTDEV",
                    Name = "Software Development"
                }
            );

        _ = modelBuilder
            .Entity<Course>()
            .HasData(
                new Course
                {
                    Id = 1,
                    Code = "CS",
                    Name = "Computer Science"
                },
                new Course
                {
                    Id = 2,
                    Code = "IT",
                    Name = "Information Technology"
                }
            );
    }
}