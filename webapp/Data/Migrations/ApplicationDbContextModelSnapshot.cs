﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using webapp.Data;

#nullable disable

namespace webapp.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            IConfigurationSection defaultAdmin = config.GetSection("DefaultAdmin");

            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("webapp.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("SchoolId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SchoolId");

                    b.ToTable("Course");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Code = "CS",
                            Name = "Computer Science",
                            SchoolId = 1
                        },
                        new
                        {
                            Id = 2,
                            Code = "IT",
                            Name = "Information Technology",
                            SchoolId = 1
                        });
                });

            modelBuilder.Entity("webapp.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("LeaderId")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.HasIndex("LeaderId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Group");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            LeaderId = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                            Name = "The Villasomethings"
                        });
                });

            modelBuilder.Entity("webapp.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Abstract")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AdviserId")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<string>("DeadlineDate")
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.Property<string>("DocumentUrl")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("InstructorId")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("PrfUrl")
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProofreaderId")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("PublishDate")
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.Property<int>("SchoolId")
                        .HasColumnType("int");

                    b.Property<int>("StateId")
                        .HasColumnType("int");

                    b.Property<int>("SubjectId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("AdviserId");

                    b.HasIndex("CourseId");

                    b.HasIndex("GroupId");

                    b.HasIndex("InstructorId");

                    b.HasIndex("ProofreaderId");

                    b.HasIndex("SchoolId");

                    b.HasIndex("StateId");

                    b.HasIndex("SubjectId");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("webapp.Models.ProjectTag", b =>
                {
                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("ProjectId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("ProjectTag");
                });

            modelBuilder.Entity("webapp.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Desc")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.ToTable("Role");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Desc = "Minimal access",
                            Name = "Unassigned"
                        },
                        new
                        {
                            Id = 2,
                            Desc = "Manages roles",
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 3,
                            Desc = "Manages groups, endorses projects to Executive Director",
                            Name = "Instructor"
                        },
                        new
                        {
                            Id = 4,
                            Desc = "Approves project documents for proofreading",
                            Name = "Executive Director"
                        },
                        new
                        {
                            Id = 5,
                            Desc = "Assigns proofreaders",
                            Name = "English Office Head"
                        },
                        new
                        {
                            Id = 6,
                            Desc = "Proofreads",
                            Name = "English Office Faculty"
                        },
                        new
                        {
                            Id = 7,
                            Desc = "Publishes",
                            Name = "Librarian"
                        },
                        new
                        {
                            Id = 8,
                            Desc = "Has access to analytics",
                            Name = "PBL Coordinator"
                        },
                        new
                        {
                            Id = 9,
                            Desc = "Has access to analytics",
                            Name = "Program Director"
                        });
                });

            modelBuilder.Entity("webapp.Models.School", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("ExecDirId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.HasIndex("ExecDirId");

                    b.ToTable("School");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Code = "SoCIT",
                            ExecDirId = defaultAdmin["Id"]!,
                            Name = "School of Computing and Information Technologies"
                        });
                });

            modelBuilder.Entity("webapp.Models.Staff", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("GivenName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.ToTable("Staff");

                    b.HasData(
                        new
                        {
                            Id = defaultAdmin["Id"]!,
                            Email = defaultAdmin["Email"]!,
                            GivenName = defaultAdmin["GivenName"]!,
                            LastName = defaultAdmin["LastName"]!
                        },
                        new
                        {
                            Id = "abcdefghijklmnopqrstuvwxyz0123486789",
                            Email = "johnd@apc.edu.ph",
                            GivenName = "John",
                            LastName = "Doe"
                        },
                        new
                        {
                            Id = "9876543210zyxwvutsrqponmlkjihgfedcba",
                            Email = "janed@apc.edu.ph",
                            GivenName = "Jane",
                            LastName = "Doe"
                        },
                        new
                        {
                            Id = "abcdefghijklmnopqrstuvwxyz9876543210",
                            Email = "johns@apc.edu.ph",
                            GivenName = "John",
                            LastName = "Smith"
                        },
                        new
                        {
                            Id = "0123486789zyxwvutsrqponmlkjihgfedcba",
                            Email = "janes@apc.edu.ph",
                            GivenName = "Jane",
                            LastName = "Smith"
                        },
                        new
                        {
                            Id = "zyxwvutsrqponmlkjihgfedcba9876543210",
                            Email = "johnf@apc.edu.ph",
                            GivenName = "John",
                            LastName = "Foobar"
                        },
                        new
                        {
                            Id = "0123486789abcdefghijklmnopqrstuvwxyz",
                            Email = "janef@apc.edu.ph",
                            GivenName = "Jane",
                            LastName = "Foobar"
                        });
                });

            modelBuilder.Entity("webapp.Models.StaffRole", b =>
                {
                    b.Property<string>("StaffId")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("StaffId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("StaffRole");

                    b.HasData(
                        new
                        {
                            StaffId = defaultAdmin["Id"]!,
                            RoleId = 2
                        },
                        new
                        {
                            StaffId = defaultAdmin["Id"]!,
                            RoleId = 3
                        },
                        new
                        {
                            StaffId = defaultAdmin["Id"]!,
                            RoleId = 4
                        },
                        new
                        {
                            StaffId = defaultAdmin["Id"]!,
                            RoleId = 5
                        },
                        new
                        {
                            StaffId = defaultAdmin["Id"]!,
                            RoleId = 6
                        },
                        new
                        {
                            StaffId = defaultAdmin["Id"]!,
                            RoleId = 7
                        },
                        new
                        {
                            StaffId = defaultAdmin["Id"]!,
                            RoleId = 8
                        },
                        new
                        {
                            StaffId = defaultAdmin["Id"]!,
                            RoleId = 9
                        },
                        new
                        {
                            StaffId = "abcdefghijklmnopqrstuvwxyz0123486789",
                            RoleId = 1
                        },
                        new
                        {
                            StaffId = "9876543210zyxwvutsrqponmlkjihgfedcba",
                            RoleId = 1
                        },
                        new
                        {
                            StaffId = "abcdefghijklmnopqrstuvwxyz9876543210",
                            RoleId = 1
                        },
                        new
                        {
                            StaffId = "0123486789zyxwvutsrqponmlkjihgfedcba",
                            RoleId = 1
                        },
                        new
                        {
                            StaffId = "zyxwvutsrqponmlkjihgfedcba9876543210",
                            RoleId = 1
                        },
                        new
                        {
                            StaffId = "0123486789abcdefghijklmnopqrstuvwxyz",
                            RoleId = 1
                        });
                });

            modelBuilder.Entity("webapp.Models.State", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ApproveStateId")
                        .HasColumnType("int");

                    b.Property<string>("Desc")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<int>("RejectStateId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("State");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ApproveStateId = 3,
                            Desc = "Project is being reviewed by the instructor",
                            Name = "Initial Review",
                            RejectStateId = 2
                        },
                        new
                        {
                            Id = 2,
                            ApproveStateId = 3,
                            Desc = "Project is being revised for initial review",
                            Name = "Initial Revisions",
                            RejectStateId = 0
                        },
                        new
                        {
                            Id = 3,
                            ApproveStateId = 4,
                            Desc = "Group is filling out the PRF template",
                            Name = "PRF Start",
                            RejectStateId = 0
                        },
                        new
                        {
                            Id = 4,
                            ApproveStateId = 5,
                            Desc = "Instructor is reviewing PRF for endorsing",
                            Name = "PRF Review",
                            RejectStateId = 3
                        },
                        new
                        {
                            Id = 5,
                            ApproveStateId = 5,
                            Desc = "Project is being reviewed by the Executive Director",
                            Name = "ExD Review",
                            RejectStateId = 3
                        },
                        new
                        {
                            Id = 6,
                            ApproveStateId = 7,
                            Desc = "English Office Head is assigning a proofreader",
                            Name = "Proofreader Assignment",
                            RejectStateId = 0
                        },
                        new
                        {
                            Id = 7,
                            ApproveStateId = 9,
                            Desc = "The project document is being proofread",
                            Name = "Proofreading",
                            RejectStateId = 8
                        },
                        new
                        {
                            Id = 8,
                            ApproveStateId = 7,
                            Desc = "To be revised",
                            Name = "Proofreading Revisions",
                            RejectStateId = 0
                        },
                        new
                        {
                            Id = 9,
                            ApproveStateId = 10,
                            Desc = "English Office Head is completing the PRF",
                            Name = "PRF Completion",
                            RejectStateId = 0
                        },
                        new
                        {
                            Id = 10,
                            ApproveStateId = 12,
                            Desc = "Instructor is overseeing final revisions recommended by panelists",
                            Name = "Panel Review",
                            RejectStateId = 11
                        },
                        new
                        {
                            Id = 11,
                            ApproveStateId = 10,
                            Desc = "Instructor is overseeing final revisions recommended by panelists",
                            Name = "Panel Revisions",
                            RejectStateId = 0
                        },
                        new
                        {
                            Id = 12,
                            ApproveStateId = 13,
                            Desc = "Librarian is reviewing project metadata",
                            Name = "Publishing",
                            RejectStateId = 0
                        },
                        new
                        {
                            Id = 13,
                            ApproveStateId = 0,
                            Desc = "The project is complete!",
                            Name = "Published",
                            RejectStateId = 0
                        });
                });

            modelBuilder.Entity("webapp.Models.Student", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Block")
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("GivenName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.ToTable("Student");

                    b.HasData(
                        new
                        {
                            Id = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                            Email = "cgvillareal@student.apc.edu.ph",
                            GivenName = "Chuse",
                            LastName = "Villareal"
                        },
                        new
                        {
                            Id = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                            Email = "cgvillarole@student.apc.edu.ph",
                            GivenName = "Cheese",
                            LastName = "Villarole"
                        },
                        new
                        {
                            Id = "cccccccccccccccccccccccccccccccccccc",
                            Email = "cgvillaroel@student.apc.edu.ph",
                            GivenName = "Chess",
                            LastName = "Villaroel"
                        });
                });

            modelBuilder.Entity("webapp.Models.StudentGroup", b =>
                {
                    b.Property<string>("StudentId")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.HasKey("StudentId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("StudentGroup");

                    b.HasData(
                        new
                        {
                            StudentId = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                            GroupId = 1
                        },
                        new
                        {
                            StudentId = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                            GroupId = 1
                        });
                });

            modelBuilder.Entity("webapp.Models.Subject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("SchoolId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SchoolId");

                    b.ToTable("Subject");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Code = "CSPROJ",
                            Name = "Applied Project for CS",
                            SchoolId = 1
                        },
                        new
                        {
                            Id = 2,
                            Code = "PROJMAN",
                            Name = "Project Management",
                            SchoolId = 1
                        },
                        new
                        {
                            Id = 3,
                            Code = "SOFTDEV",
                            Name = "Software Development",
                            SchoolId = 1
                        });
                });

            modelBuilder.Entity("webapp.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("webapp.Models.Course", b =>
                {
                    b.HasOne("webapp.Models.School", "School")
                        .WithMany()
                        .HasForeignKey("SchoolId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("School");
                });

            modelBuilder.Entity("webapp.Models.Group", b =>
                {
                    b.HasOne("webapp.Models.Student", "Leader")
                        .WithMany()
                        .HasForeignKey("LeaderId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Leader");
                });

            modelBuilder.Entity("webapp.Models.Project", b =>
                {
                    b.HasOne("webapp.Models.Staff", "Adviser")
                        .WithMany()
                        .HasForeignKey("AdviserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("webapp.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("webapp.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("webapp.Models.Staff", "Instructor")
                        .WithMany()
                        .HasForeignKey("InstructorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("webapp.Models.Staff", "Proofreader")
                        .WithMany()
                        .HasForeignKey("ProofreaderId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("webapp.Models.School", "School")
                        .WithMany()
                        .HasForeignKey("SchoolId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("webapp.Models.State", "State")
                        .WithMany()
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("webapp.Models.Subject", "Subject")
                        .WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Adviser");

                    b.Navigation("Course");

                    b.Navigation("Group");

                    b.Navigation("Instructor");

                    b.Navigation("Proofreader");

                    b.Navigation("School");

                    b.Navigation("State");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("webapp.Models.ProjectTag", b =>
                {
                    b.HasOne("webapp.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("webapp.Models.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("webapp.Models.School", b =>
                {
                    b.HasOne("webapp.Models.Staff", "ExecDir")
                        .WithMany()
                        .HasForeignKey("ExecDirId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ExecDir");
                });

            modelBuilder.Entity("webapp.Models.StaffRole", b =>
                {
                    b.HasOne("webapp.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("webapp.Models.Staff", "Staff")
                        .WithMany()
                        .HasForeignKey("StaffId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("Staff");
                });

            modelBuilder.Entity("webapp.Models.StudentGroup", b =>
                {
                    b.HasOne("webapp.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("webapp.Models.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("webapp.Models.Subject", b =>
                {
                    b.HasOne("webapp.Models.School", "School")
                        .WithMany()
                        .HasForeignKey("SchoolId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("School");
                });
#pragma warning restore 612, 618
        }
    }
}