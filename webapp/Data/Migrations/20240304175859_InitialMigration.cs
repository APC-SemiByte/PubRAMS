using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace webapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            IConfigurationSection defaultAdmin = config.GetSection("DefaultAdmin");

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Completion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Completion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    AcceptStateId = table.Column<int>(type: "int", nullable: false),
                    RejectStateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Block = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "School",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ExecDirId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_School", x => x.Id);
                    table.ForeignKey(
                        name: "FK_School_Staff_ExecDirId",
                        column: x => x.ExecDirId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffRole",
                columns: table => new
                {
                    StaffId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffRole", x => new { x.StaffId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_StaffRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffRole_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LeaderId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Group_Student_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Course_School_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "School",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subject_School_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "School",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentGroup",
                columns: table => new
                {
                    StudentId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroup", x => new { x.StudentId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_StudentGroup_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGroup_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Abstract = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Continued = table.Column<bool>(type: "bit", nullable: false),
                    Archived = table.Column<bool>(type: "bit", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    CompletionId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    AdviserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    ProofreaderId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    StudentComment = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: true),
                    StaffComment = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: true),
                    BaseHandle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    HasPrf = table.Column<bool>(type: "bit", nullable: false),
                    HasPdf = table.Column<bool>(type: "bit", nullable: false),
                    Edited = table.Column<bool>(type: "bit", nullable: false),
                    KohaRecordId = table.Column<int>(type: "int", nullable: true),
                    DeadlineDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Term = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Completion_CompletionId",
                        column: x => x.CompletionId,
                        principalTable: "Completion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_School_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "School",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Staff_AdviserId",
                        column: x => x.AdviserId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Staff_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Staff_ProofreaderId",
                        column: x => x.ProofreaderId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_State_StateId",
                        column: x => x.StateId,
                        principalTable: "State",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Hospitality" },
                    { 2, "Food Service" },
                    { 3, "Retail/Wholesale" },
                    { 4, "Medical" },
                    { 5, "Education" },
                    { 6, "E-Commerce" },
                    { 7, "Agriculture" },
                    { 8, "Govenrment/LGU" },
                    { 9, "Human Resource" },
                    { 10, "Marketing and Distribution" },
                    { 11, "Manufacturing" },
                    { 12, "Others" }
                });

            migrationBuilder.InsertData(
                table: "Completion",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Unfinished" },
                    { 2, "Implemented" },
                    { 3, "Deployed" },
                    { 4, "Donated" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Desc", "Name" },
                values: new object[,]
                {
                    { 1, "Minimal access", "Unassigned" },
                    { 2, "Manages roles", "Admin" },
                    { 3, "Manages groups, endorses projects to Executive Director", "Instructor" },
                    { 4, "Approves project documents for proofreading", "Executive Director" },
                    { 5, "Assigns proofreaders", "English Office Head" },
                    { 6, "Proofreads", "English Office Faculty" },
                    { 7, "Publishes", "Librarian" },
                    { 8, "Has access to analytics", "PBL Coordinator" },
                    { 9, "Has access to analytics", "Program Director" }
                });

            migrationBuilder.InsertData(
                table: "Staff",
                columns: new[] { "Id", "Email", "GivenName", "LastName" },
                values: new object[,]
                {
                    { "0123486789abcdefghijklmnopqrstuvwxyz", "janef@apc.edu.ph", "Jane", "Foobar" },
                    { "0123486789zyxwvutsrqponmlkjihgfedcba", "janes@apc.edu.ph", "Jane", "Smith" },
                    { "9876543210zyxwvutsrqponmlkjihgfedcba", "janed@apc.edu.ph", "Jane", "Doe" },
                    { "abcdefghijklmnopqrstuvwxyz0123486789", "johnd@apc.edu.ph", "John", "Doe" },
                    { "abcdefghijklmnopqrstuvwxyz9876543210", "johns@apc.edu.ph", "John", "Smith" },
                    { defaultAdmin["Id"]!, defaultAdmin["Email"]!, defaultAdmin["GivenName"]!, defaultAdmin["LastName"]! },
                    { "zyxwvutsrqponmlkjihgfedcba9876543210", "johnf@apc.edu.ph", "John", "Foobar" }
                });

            migrationBuilder.InsertData(
                table: "State",
                columns: new[] { "Id", "AcceptStateId", "Desc", "Name", "RejectStateId" },
                values: new object[,]
                {
                    { 1, 3, "Instructor is reviewing the project", "Initial Review", 2 },
                    { 2, 1, "Group is revising the document for initial review", "Initial Revisions", 0 },
                    { 3, 4, "Group is filling out the PRF template", "PRF Start", 0 },
                    { 4, 5, "Instructor is reviewing PRF for endorsing", "PRF Review", 3 },
                    { 5, 6, "Executive director is reviewing the project", "ExD Review", 2 },
                    { 6, 7, "English Office Head is assigning a proofreader", "Proofreader Assignment", 0 },
                    { 7, 9, "Proofreader is proofreading the document", "Proofreading", 8 },
                    { 8, 7, "Group is revising the document for proofreading", "Proofreading Revisions", 0 },
                    { 9, 10, "English Office Head is completing the PRF", "PRF Completion", 0 },
                    { 10, 12, "Instructor is overseeing final revisions recommended by panelists", "Panel Review", 11 },
                    { 11, 10, "Group is revising the document for panelist review", "Panel Revisions", 0 },
                    { 12, 13, "Project is being finalized (converting document to PDF)", "Finalizing", 0 },
                    { 13, 14, "Librarian is reviewing project metadata for publishing", "Publishing", 12 },
                    { 14, 0, "The project is complete", "Published", 0 }
                });

            migrationBuilder.InsertData(
                table: "Student",
                columns: new[] { "Id", "Block", "Email", "GivenName", "LastName" },
                values: new object[,]
                {
                    { "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", null, "cgvillareal@student.apc.edu.ph", "Chuse", "Villareal" },
                    { "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", null, "cgvillarole@student.apc.edu.ph", "Cheese", "Villarole" },
                    { "cccccccccccccccccccccccccccccccccccc", null, "cgvillaroel@student.apc.edu.ph", "Chess", "Villaroel" }
                });

            migrationBuilder.InsertData(
                table: "Group",
                columns: new[] { "Id", "LeaderId", "Name" },
                values: new object[] { 1, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "The Villasomethings" });

            migrationBuilder.InsertData(
                table: "School",
                columns: new[] { "Id", "Code", "ExecDirId", "Name" },
                values: new object[] { 1, "SoCIT", defaultAdmin["Id"]!, "School of Computing and Information Technologies" });

            migrationBuilder.InsertData(
                table: "StaffRole",
                columns: new[] { "RoleId", "StaffId" },
                values: new object[,]
                {
                    { 1, "0123486789abcdefghijklmnopqrstuvwxyz" },
                    { 1, "0123486789zyxwvutsrqponmlkjihgfedcba" },
                    { 1, "9876543210zyxwvutsrqponmlkjihgfedcba" },
                    { 1, "abcdefghijklmnopqrstuvwxyz0123486789" },
                    { 1, "abcdefghijklmnopqrstuvwxyz9876543210" },
                    { 2, defaultAdmin["Id"]! },
                    { 3, defaultAdmin["Id"]! },
                    { 4, defaultAdmin["Id"]! },
                    { 5, defaultAdmin["Id"]! },
                    { 6, defaultAdmin["Id"]! },
                    { 7, defaultAdmin["Id"]! },
                    { 8, defaultAdmin["Id"]! },
                    { 9, defaultAdmin["Id"]! },
                    { 1, "zyxwvutsrqponmlkjihgfedcba9876543210" }
                });

            migrationBuilder.InsertData(
                table: "Course",
                columns: new[] { "Id", "Code", "Name", "SchoolId" },
                values: new object[,]
                {
                    { 1, "CS", "Computer Science", 1 },
                    { 2, "IT", "Information Technology", 1 }
                });

            migrationBuilder.InsertData(
                table: "StudentGroup",
                columns: new[] { "GroupId", "StudentId" },
                values: new object[,]
                {
                    { 1, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" },
                    { 1, "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb" }
                });

            migrationBuilder.InsertData(
                table: "Subject",
                columns: new[] { "Id", "Code", "Name", "SchoolId" },
                values: new object[,]
                {
                    { 1, "CSPROJ", "Applied Project for CS", 1 },
                    { 2, "PROJMAN", "Project Management", 1 },
                    { 3, "SOFTDEV", "Software Development", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                table: "Category",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Completion_Name",
                table: "Completion",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Code",
                table: "Course",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Course_SchoolId",
                table: "Course",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Group_LeaderId",
                table: "Group",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Group_Name",
                table: "Group",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_AdviserId",
                table: "Project",
                column: "AdviserId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CategoryId",
                table: "Project",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CompletionId",
                table: "Project",
                column: "CompletionId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CourseId",
                table: "Project",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_GroupId",
                table: "Project",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_InstructorId",
                table: "Project",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProofreaderId",
                table: "Project",
                column: "ProofreaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_SchoolId",
                table: "Project",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_StateId",
                table: "Project",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_SubjectId",
                table: "Project",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                table: "Role",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_School_ExecDirId",
                table: "School",
                column: "ExecDirId");

            migrationBuilder.CreateIndex(
                name: "IX_School_Name",
                table: "School",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_Email",
                table: "Staff",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_StaffRole_RoleId",
                table: "StaffRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_State_Name",
                table: "State",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Email",
                table: "Student",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroup_GroupId",
                table: "StudentGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_Code",
                table: "Subject",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_SchoolId",
                table: "Subject",
                column: "SchoolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "StaffRole");

            migrationBuilder.DropTable(
                name: "StudentGroup");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Completion");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "State");

            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "School");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "Staff");
        }
    }
}