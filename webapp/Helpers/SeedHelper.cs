using Microsoft.EntityFrameworkCore;
using webapp.Models;

namespace webapp.Helpers;

/// <summary>
/// Static helper class for initializing the database
/// </summary>
public static class SeedHelper
{
    /// <summary>
    /// Initializes the database with constant values
    /// </summary>
    public static void Seed(ModelBuilder modelBuilder, bool developerMode = false)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        IConfigurationSection defaultAdmin = config.GetSection("DefaultAdmin");

        SeedProd(modelBuilder, defaultAdmin);
        if (!developerMode)
        {
            return;
        }

        SeedDev(modelBuilder, defaultAdmin);
    }

    private static void SeedProd(ModelBuilder modelBuilder, IConfigurationSection defaultAdmin)
    {
        _ = modelBuilder
            .Entity<Role>()
            .HasData(
                new Role
                {
                    Id = 1,
                    Name = "Unassigned",
                    Desc = "Minimal access"
                },
                new Role
                {
                    Id = 2,
                    Name = "Admin",
                    Desc = "Manages roles"
                },
                new Role
                {
                    Id = 3,
                    Name = "Instructor",
                    Desc = "Manages groups, endorses projects to Executive Director"
                },
                new Role
                {
                    Id = 4,
                    Name = "Executive Director",
                    Desc = "Approves project documents for proofreading"
                },
                new Role
                {
                    Id = 5,
                    Name = "English Office Head",
                    Desc = "Assigns proofreaders"
                },
                new Role
                {
                    Id = 6,
                    Name = "English Office Faculty",
                    Desc = "Proofreads"
                },
                new Role
                {
                    Id = 7,
                    Name = "Librarian",
                    Desc = "Publishes"
                },
                new Role
                {
                    Id = 8,
                    Name = "PBL Coordinator",
                    Desc = "Has access to analytics"
                },
                new Role
                {
                    Id = 9,
                    Name = "Program Director",
                    Desc = "Has access to analytics"
                }
            );

        _ = modelBuilder
            .Entity<Staff>()
            .HasData(
                new Staff
                {
                    Id = defaultAdmin["Id"]!,
                    GivenName = defaultAdmin["GivenName"]!,
                    LastName = defaultAdmin["LastName"]!,
                    Email = defaultAdmin["Email"]!
                }
            );

        _ = modelBuilder
            .Entity<StaffRole>()
            .HasData(
                new StaffRole
                {
                    StaffId = defaultAdmin["Id"]!,
                    RoleId = 2,
                }
            );

        _ = modelBuilder
            .Entity<School>()
            .HasData(
                new School
                {
                    Id = 1,
                    Code = "SoCIT",
                    Name = "School of Computing and Information Technologies",
                    ExecDirId = defaultAdmin["Id"]!
                }
            );

        _ = modelBuilder
            .Entity<Subject>()
            .HasData(
                new Subject
                {
                    Id = 1,
                    Code = "CSPROJ",
                    Name = "Applied Project for CS",
                    SchoolId = 1
                },
                new Subject
                {
                    Id = 2,
                    Code = "PROJMAN",
                    Name = "Project Management",
                    SchoolId = 1
                },
                new Subject
                {
                    Id = 3,
                    Code = "SOFTDEV",
                    Name = "Software Development",
                    SchoolId = 1
                }
            );

        _ = modelBuilder
            .Entity<Course>()
            .HasData(
                new Course
                {
                    Id = 1,
                    Code = "CS",
                    Name = "Computer Science",
                    SchoolId = 1
                },
                new Course
                {
                    Id = 2,
                    Code = "IT",
                    Name = "Information Technology",
                    SchoolId = 1
                }
            );

        _ = modelBuilder
            .Entity<State>()
            .HasData(
                new State
                {
                    Id = 1,
                    Name = "Submitted",
                    Desc = "To be reviewed by the instructor"
                }
            );
    }

    public static void SeedDev(ModelBuilder modelBuilder, IConfigurationSection defaultAdmin)
    {
        _ = modelBuilder
            .Entity<Staff>()
            .HasData(
                new Staff
                {
                    Id = "abcdefghijklmnopqrstuvwxyz0123486789",
                    GivenName = "John",
                    LastName = "Doe",
                    Email = "johnd@apc.edu.ph"
                },
                new Staff
                {
                    Id = "9876543210zyxwvutsrqponmlkjihgfedcba",
                    GivenName = "Jane",
                    LastName = "Doe",
                    Email = "janed@apc.edu.ph"
                },
                new Staff
                {
                    Id = "abcdefghijklmnopqrstuvwxyz9876543210",
                    GivenName = "John",
                    LastName = "Smith",
                    Email = "johns@apc.edu.ph"
                },
                new Staff
                {
                    Id = "0123486789zyxwvutsrqponmlkjihgfedcba",
                    GivenName = "Jane",
                    LastName = "Smith",
                    Email = "janes@apc.edu.ph"
                },
                new Staff
                {
                    Id = "zyxwvutsrqponmlkjihgfedcba9876543210",
                    GivenName = "John",
                    LastName = "Foobar",
                    Email = "johnf@apc.edu.ph"
                },
                new Staff
                {
                    Id = "0123486789abcdefghijklmnopqrstuvwxyz",
                    GivenName = "Jane",
                    LastName = "Foobar",
                    Email = "janef@apc.edu.ph"
                }
            );

        _ = modelBuilder
            .Entity<StaffRole>()
            .HasData(
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = 3 },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = 4 },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = 5 },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = 6 },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = 7 },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = 8 },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = 9 },
                new StaffRole { StaffId = "abcdefghijklmnopqrstuvwxyz0123486789", RoleId = 1 },
                new StaffRole { StaffId = "9876543210zyxwvutsrqponmlkjihgfedcba", RoleId = 1 },
                new StaffRole { StaffId = "abcdefghijklmnopqrstuvwxyz9876543210", RoleId = 1 },
                new StaffRole { StaffId = "0123486789zyxwvutsrqponmlkjihgfedcba", RoleId = 1 },
                new StaffRole { StaffId = "zyxwvutsrqponmlkjihgfedcba9876543210", RoleId = 1 },
                new StaffRole { StaffId = "0123486789abcdefghijklmnopqrstuvwxyz", RoleId = 1 }
            );

        _ = modelBuilder
            .Entity<Student>()
            .HasData(
                new Student()
                {
                    Id = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                    GivenName = "Chuse",
                    LastName = "Villareal",
                    Email = "cgvillareal@student.apc.edu.ph",
                },
                new Student()
                {
                    Id = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                    GivenName = "Cheese",
                    LastName = "Villarole",
                    Email = "cgvillarole@student.apc.edu.ph",
                },
                new Student()
                {
                    Id = "cccccccccccccccccccccccccccccccccccc",
                    GivenName = "Chess",
                    LastName = "Villaroel",
                    Email = "cgvillaroel@student.apc.edu.ph",
                }
            );

        _ = modelBuilder
            .Entity<Group>()
            .HasData(
                new Group()
                {
                    Id = 1,
                    Name = "The Villasomethings",
                    LeaderId = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                }
            );

        _ = modelBuilder
            .Entity<StudentGroup>()
            .HasData(
                new StudentGroup()
                {
                    StudentId = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                    GroupId = 1
                },
                new StudentGroup()
                {
                    StudentId = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                    GroupId = 1
                }
            );
    }
}