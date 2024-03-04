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
                    Id = (int)Roles.Unassigned,
                    Name = "Unassigned",
                    Desc = "Minimal access"
                },
                new Role
                {
                    Id = (int)Roles.Admin,
                    Name = "Admin",
                    Desc = "Manages roles"
                },
                new Role
                {
                    Id = (int)Roles.Instructor,
                    Name = "Instructor",
                    Desc = "Manages groups, endorses projects to Executive Director"
                },
                new Role
                {
                    Id = (int)Roles.ExecutiveDirector,
                    Name = "Executive Director",
                    Desc = "Approves project documents for proofreading"
                },
                new Role
                {
                    Id = (int)Roles.EcHead,
                    Name = "English Office Head",
                    Desc = "Assigns proofreaders"
                },
                new Role
                {
                    Id = (int)Roles.EcFaculty,
                    Name = "English Office Faculty",
                    Desc = "Proofreads"
                },
                new Role
                {
                    Id = (int)Roles.Librarian,
                    Name = "Librarian",
                    Desc = "Publishes"
                },
                new Role
                {
                    Id = (int)Roles.PblCoordinator,
                    Name = "PBL Coordinator",
                    Desc = "Has access to analytics"
                },
                new Role
                {
                    Id = (int)Roles.ProgramDirector,
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
                    RoleId = (int)Roles.Admin,
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
                    Id = (int)States.InitialReview,
                    Name = "Initial Review",
                    Desc = "Instructor is reviewing the project",
                    AcceptStateId = (int)States.PrfStart,
                    RejectStateId = (int)States.InitialRevisions
                },
                new State
                {
                    Id = (int)States.InitialRevisions,
                    Name = "Initial Revisions",
                    Desc = "Group is revising the document for initial review",
                    AcceptStateId = (int)States.InitialReview,
                },
                new State
                {
                    Id = (int)States.PrfStart,
                    Name = "PRF Start",
                    Desc = "Group is filling out the PRF template",
                    AcceptStateId = (int)States.PrfReview,
                },
                new State
                {
                    Id = (int)States.PrfReview,
                    Name = "PRF Review",
                    Desc = "Instructor is reviewing PRF for endorsing",
                    AcceptStateId = (int)States.ExdReview,
                    RejectStateId = (int)States.PrfStart,
                },
                new State
                {
                    Id = (int)States.ExdReview,
                    Name = "ExD Review",
                    Desc = "Executive director is reviewing the project",
                    AcceptStateId = (int)States.Assignment,
                    RejectStateId = (int)States.InitialRevisions,
                },
                new State
                {
                    Id = (int)States.Assignment,
                    Name = "Proofreader Assignment",
                    Desc = "English Office Head is assigning a proofreader",
                    AcceptStateId = (int)States.Proofreading,
                },
                new State
                {
                    Id = (int)States.Proofreading,
                    Name = "Proofreading",
                    Desc = "Proofreader is proofreading the document",
                    AcceptStateId = (int)States.PrfCompletion,
                    RejectStateId = (int)States.ProofreadingRevisions,
                },
                new State
                {
                    Id = (int)States.ProofreadingRevisions,
                    Name = "Proofreading Revisions",
                    Desc = "Group is revising the document for proofreading",
                    AcceptStateId = (int)States.Proofreading,
                },
                new State
                {
                    Id = (int)States.PrfCompletion,
                    Name = "PRF Completion",
                    Desc = "English Office Head is completing the PRF",
                    AcceptStateId = (int)States.PanelReview,
                },
                new State
                {
                    Id = (int)States.PanelReview,
                    Name = "Panel Review",
                    Desc = "Instructor is overseeing final revisions recommended by panelists",
                    AcceptStateId = (int)States.Finalizing,
                    RejectStateId = (int)States.PanelRevisions,
                },
                new State
                {
                    Id = (int)States.PanelRevisions,
                    Name = "Panel Revisions",
                    Desc = "Group is revising the document for panelist review",
                    AcceptStateId = (int)States.PanelReview,
                },
                new State
                {
                    Id = (int)States.Finalizing,
                    Name = "Finalizing",
                    Desc = "Project is being finalized (converting document to PDF)",
                    AcceptStateId = (int)States.Publishing,
                },
                new State
                {
                    Id = (int)States.Publishing,
                    Name = "Publishing",
                    Desc = "Librarian is reviewing project metadata for publishing",
                    AcceptStateId = (int)States.Published,
                    RejectStateId = (int)States.Finalizing,
                },
                new State
                {
                    Id = (int)States.Published,
                    Name = "Published",
                    Desc = "The project is complete",
                }
            );

        _ = modelBuilder
            .Entity<Category>()
            .HasData(
                new Category
                {
                    Id = (int)Categories.Hospitality,
                    Name = "Hospitality"
                },
                new Category
                {
                    Id = (int)Categories.FoodService,
                    Name = "Food Service"
                },
                new Category
                {
                    Id = (int)Categories.Retail,
                    Name = "Retail/Wholesale"
                },
                new Category
                {
                    Id = (int)Categories.Medical,
                    Name = "Medical"
                },
                new Category
                {
                    Id = (int)Categories.Education,
                    Name = "Education"
                },
                new Category
                {
                    Id = (int)Categories.Ecommerce,
                    Name = "E-Commerce"
                },
                new Category
                {
                    Id = (int)Categories.Agrigulture,
                    Name = "Agriculture"
                },
                new Category
                {
                    Id = (int)Categories.Government,
                    Name = "Govenrment/LGU"
                },
                new Category
                {
                    Id = (int)Categories.HumanResource,
                    Name = "Human Resource"
                },
                new Category
                {
                    Id = (int)Categories.Marketing,
                    Name = "Marketing and Distribution"
                },
                new Category
                {
                    Id = (int)Categories.Manufacturing,
                    Name = "Manufacturing"
                },
                new Category
                {
                    Id = (int)Categories.Others,
                    Name = "Others"
                }
            );

        _ = modelBuilder
            .Entity<Completion>()
            .HasData(
                new Completion
                {
                    Id = (int)Completions.Unfinished,
                    Name = "Unfinished",
                },
                new Completion
                {
                    Id = (int)Completions.Implemented,
                    Name = "Implemented",
                },
                new Completion
                {
                    Id = (int)Completions.Deployed,
                    Name = "Deployed",
                },
                new Completion
                {
                    Id = (int)Completions.Donated,
                    Name = "Donated",
                },
                new Completion
                {
                    Id = (int)Completions.Archived,
                    Name = "Archived",
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
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = (int)Roles.Instructor },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = (int)Roles.ExecutiveDirector },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = (int)Roles.EcHead },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = (int)Roles.EcFaculty },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = (int)Roles.Librarian },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = (int)Roles.PblCoordinator },
                new StaffRole { StaffId = defaultAdmin["Id"]!, RoleId = (int)Roles.ProgramDirector },
                new StaffRole { StaffId = "abcdefghijklmnopqrstuvwxyz0123486789", RoleId = (int)Roles.Unassigned },
                new StaffRole { StaffId = "9876543210zyxwvutsrqponmlkjihgfedcba", RoleId = (int)Roles.Unassigned },
                new StaffRole { StaffId = "abcdefghijklmnopqrstuvwxyz9876543210", RoleId = (int)Roles.Unassigned },
                new StaffRole { StaffId = "0123486789zyxwvutsrqponmlkjihgfedcba", RoleId = (int)Roles.Unassigned },
                new StaffRole { StaffId = "zyxwvutsrqponmlkjihgfedcba9876543210", RoleId = (int)Roles.Unassigned },
                new StaffRole { StaffId = "0123486789abcdefghijklmnopqrstuvwxyz", RoleId = (int)Roles.Unassigned }
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