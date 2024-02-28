using webapp.Data;
using webapp.Helpers;
using webapp.Models.Dtos;
using webapp.Models.ViewModels;

using System.Text;

namespace webapp.Models.EntityManagers;

public class ProjectManager
{
    public string Add(SubmissionDto submission)
    {
        using ApplicationDbContext db = new();
        int schoolId = db.School.FirstOrDefault(e => e.Name == submission.School)!.Id;
        int subjectId = db.Subject.FirstOrDefault(e => e.Code == submission.Subject)!.Id;
        int courseId = db.Course.FirstOrDefault(e => e.Code == submission.Course)!.Id;

        int groupId = db.Group.FirstOrDefault(e => e.Name == submission.Group)!.Id;
        string instructorId = db.Staff.FirstOrDefault(
            e => e.Email == submission.InstructorEmail
        )!.Id;
        string adviserId = db.Staff.FirstOrDefault(e => e.Email == submission.InstructorEmail)!.Id;

        Project newProject = new()
        {
            Title = submission.Title,
            GroupId = groupId,
            Abstract = submission.Abstract,
            StateId = 1,
            SchoolId = schoolId,
            SubjectId = subjectId,
            CourseId = courseId,
            InstructorId = instructorId,
            AdviserId = adviserId
        };

        _ = db.Project.Add(newProject);
        _ = db.SaveChanges();
        string handle = $"{submission.Group}-{newProject.Id}.docx";
        newProject.DocumentHandle = handle;
        _ = db.SaveChanges();

        return handle;
    }

    public string? Edit(int id, EditSubmissionDto dto)
    {
        using ApplicationDbContext db = new();

        Project? project = (
            from project_ in db.Project
            join studentGroup in db.StudentGroup on project_.GroupId equals studentGroup.GroupId
            where project_.Id == id
            select project_
        ).FirstOrDefault();

        if (project == null)
        {
            return null;
        }

        int schoolId = db.School.FirstOrDefault(e => e.Name == dto.School)!.Id;
        int subjectId = db.Subject.FirstOrDefault(e => e.Code == dto.Subject)!.Id;
        int courseId = db.Course.FirstOrDefault(e => e.Code == dto.Course)!.Id;

        int groupId = db.Group.FirstOrDefault(e => e.Name == dto.Group)!.Id;
        string instructorId = db.Staff.FirstOrDefault(
            e => e.Email == dto.InstructorEmail
        )!.Id;
        string adviserId = db.Staff.FirstOrDefault(e => e.Email == dto.InstructorEmail)!.Id;

        project.Title = dto.Title;
        project.GroupId = groupId;
        project.Abstract = dto.Abstract;
        project.SchoolId = schoolId;
        project.SubjectId = subjectId;
        project.CourseId = courseId;
        project.InstructorId = instructorId;
        project.AdviserId = adviserId;

        _ = db.SaveChanges();

        return project.DocumentHandle;
    }

    public bool InvolvesStudent(int id, string student)
    {
        using ApplicationDbContext db = new();
        return (
            from project in db.Project
            join studentGroup in db.StudentGroup on project.GroupId equals studentGroup.GroupId
            where studentGroup.StudentId == student
            select studentGroup
       ).Any();
    }

    public string GetDocumentHandle(int projectId)
    {
        using ApplicationDbContext db = new();
        // validator guarantees this isn't null
        return db.Project.FirstOrDefault(e => e.Id == projectId)!.DocumentHandle!;
    }

    public string? GetPrfHandle(int projectId)
    {
        using ApplicationDbContext db = new();
        return db.Project.FirstOrDefault(e => e.Id == projectId)!.PrfHandle;
    }

    public EditSubmissionDto? GenerateProjectViewModel(int id, string student)
    {
        using ApplicationDbContext db = new();
        return (
            from project_ in db.Project
            join group_ in db.Group on project_.GroupId equals group_.Id
            join studentGroup in db.StudentGroup on project_.GroupId equals studentGroup.GroupId
            where project_.Id == id && studentGroup.StudentId == student
            select new EditSubmissionDto
            {
                Title = project_.Title,
                Group = group_.Name,
                Abstract = project_.Abstract,

                // will be ignored, we use a dynamically loaded dropdown for these
                School = "",
                Subject = "",
                Course = "",
                AdviserEmail = "",
                InstructorEmail = "",
            }
        ).FirstOrDefault();
    }

    public ProjectListViewModel GenerateProjectListViewModel(IUser user)
    {
        return user.GetType() == typeof(Student)
            ? GenerateProjectListViewModel((Student)user)
            : GenerateProjectListViewModel((Staff)user);
    }

    private static ProjectListViewModel GenerateProjectListViewModel(Student student)
    {
        using ApplicationDbContext db = new();
        IQueryable<int> groupIds =
            from studentGroup in db.StudentGroup
            where studentGroup.StudentId == student.Id
            select studentGroup.GroupId;

        List<int> urgentStates =
        [
            (int)States.InitialRevisions,
            (int)States.PrfStart,
            (int)States.ProofreadingRevisions,
            (int)States.PanelRevisions,
            (int)States.Finalizing,
        ];

        List<ProjectViewModel> urgent = [];
        List<ProjectViewModel> notUrgent = [];

        List<Project> projects = (
            from project in db.Project
            where groupIds.Any(id => id == project.GroupId)
            select project
        ).ToList();

        foreach (Project project in projects)
        {
            if (urgentStates.Contains(project.StateId))
            {
                urgent.Add(ToViewModel(db, project, "Submit"));
            }
            else
            {
                notUrgent.Add(ToViewModel(db, project));
            }
        }

        return new() { UrgentProjects = urgent, Projects = notUrgent };
    }

    private static ProjectListViewModel GenerateProjectListViewModel(Staff staff)
    {
        using ApplicationDbContext db = new();

        List<int> roleIds = (
            from staffRole in db.StaffRole
            where staffRole.StaffId == staff.Id
            select staffRole.RoleId
        ).ToList();

        List<Project> projects = roleIds.Contains((int)Roles.EcHead)
            // EC head is involved with all projects. They should have a heads up
            ? db.Project.Where(e => e.StateId <= (int)States.PrfCompletion).ToList()
            // Everyone else has a limited view, only what they're involved with
            : db.Project.Where(
                e =>
                    e.InstructorId == staff.Id
                    || e.AdviserId == staff.Id
                    || e.ProofreaderId == staff.Id
                    // Executive directors see everything in their school
                    || db.School.FirstOrDefault(s => s.Id == e.SchoolId)!.ExecDirId == staff.Id
            )
                .ToList();

        List<ProjectViewModel> urgent = [];
        List<ProjectViewModel> notUrgent = [];

        foreach (Project project in projects)
        {
            string? action = DetermineStaffAction(staff, roleIds, project, db);

            if (action != null)
            {
                urgent.Add(ToViewModel(db, project, action));
            }
            else
            {
                notUrgent.Add(ToViewModel(db, project, null));
            }
        }

        return new() { UrgentProjects = urgent, Projects = notUrgent };
    }

    public async Task<bool> Accept(ActionDto dto, Staff staff)
    {
        return await AcceptOrRejectAsync(new() { ProjectId = dto.ProjectId, File = null! }, staff, true);
    }

    public async Task<bool> Reject(FileActionDto dto, Staff staff, string filesPath)
    {
        return await AcceptOrRejectAsync(dto, staff, false, filesPath);
    }

    private static async Task<bool> AcceptOrRejectAsync(
        FileActionDto dto,
        Staff staff,
        bool accept,
        string? filesPath = null
    )
    {
        using ApplicationDbContext db = new();

        // validator guarantees this is not null
        Project project = db.Project.FirstOrDefault(e => e.Id == dto.ProjectId)!;

        int acceptId = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;
        int rejectId = db.State.FirstOrDefault(e => e.Id == project.StateId)!.RejectStateId;

        string handle;

        switch (project.StateId)
        {
            case (int)States.InitialReview:
                if (staff.Id != project.InstructorId)
                {
                    return false;
                }
                // TODO: state-specific operations
                handle = project.DocumentHandle!;
                break;

            case (int)States.PrfReview:
                if (staff.Id != project.InstructorId)
                {
                    return false;
                }
                // TODO: state-specific operations
                handle = project.PrfHandle!;
                break;

            case (int)States.ExdReview:
                if (staff.Id != db.School.FirstOrDefault(e => e.Id == project.SchoolId)!.ExecDirId)
                {
                    return false;
                }
                // TODO: state-specific operations
                handle = project.DocumentHandle!;
                break;

            case (int)States.Proofreading:
                if (staff.Id != project.ProofreaderId)
                {
                    return false;
                }
                // TODO: state-specific operations
                handle = project.DocumentHandle!;
                break;

            case (int)States.PanelReview:
                if (staff.Id != project.InstructorId)
                {
                    return false;
                }
                // TODO: state-specific operations
                handle = project.DocumentHandle!;
                break;

            case (int)States.PrfCompletion:
                if (
                    db.StaffRole.FirstOrDefault(
                        e => e.StaffId == staff.Id && e.RoleId == (int)Roles.EcHead
                    ) == null
                )
                {
                    return false;
                }
                // TODO: state-specific operations
                handle = project.DocumentHandle!;
                break;

            case (int)States.Publishing:
                if (
                    db.StaffRole.FirstOrDefault(
                        e => e.StaffId == staff.Id && e.RoleId == (int)Roles.Librarian
                    ) == null
                )
                {
                    return false;
                }
                // TODO: state-specific operations
                handle = project.DocumentHandle!;
                break;

            default:
                return false;
        }

        if (accept)
        {
            project.StateId = acceptId;
            _ = db.SaveChanges();
            return true;
        }

        string path = Path.Combine(filesPath!, handle);
        using Stream file = File.Create(path);
        await dto.File.CopyToAsync(file);

        project.StateId = rejectId;
        _ = db.SaveChanges();

        return true;
    }

    public async Task<bool> Submit(FileActionDto dto, IUser user, string filesPath)
    {
        return user.GetType() == typeof(Student)
            ? await Submit(dto, (Student)user, filesPath)
            : await Submit(dto, (Staff)user, filesPath);
    }

    private static async Task<bool> Submit(FileActionDto dto, Student student, string filesPath)
    {
        using ApplicationDbContext db = new();
        // validator guarantees this isn't null
        Project project = db.Project.FirstOrDefault(e => e.Id == dto.ProjectId)!;
        int nextState = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;

        bool studentInGroup =
            db.StudentGroup.FirstOrDefault(
                e => e.StudentId == student.Id && e.GroupId == project.GroupId
            ) != null;
        if (!studentInGroup)
        {
            return false;
        }

        string handle;

        switch (project.StateId)
        {
            case (int)States.InitialRevisions:
            case (int)States.ProofreadingRevisions:
            case (int)States.PanelRevisions:
            case (int)States.Finalizing:
                // TODO: state-specific operations
                handle = project.DocumentHandle!;
                break;

            case (int)States.PrfStart:
                // TODO: state-specific operations
                string groupName = db.Group.FirstOrDefault(e => e.Id == project.GroupId)!.Name;
                project.PrfHandle = $"{groupName}-{project.Id}-Prf.pdf";
                handle = project.PrfHandle!;
                break;

            default:
                return false;
        }

        string path = Path.Combine(filesPath!, handle);
        using Stream file = File.Create(path);
        await dto.File.CopyToAsync(file);

        project.StateId = nextState;
        _ = db.SaveChanges();

        return true;
    }

    private static async Task<bool> Submit(FileActionDto dto, Staff staff, string filesPath)
    {
        using ApplicationDbContext db = new();
        // validator guarantees this isn't null
        Project project = db.Project.FirstOrDefault(e => e.Id == dto.ProjectId)!;
        List<int> roles = (
            from staffRole in db.StaffRole
            where staffRole.StaffId == staff.Id
            select staffRole.RoleId
        ).ToList();

        int nextState = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;

        if (!roles.Contains((int)Roles.EcHead))
        {
            return false;
        }

        string handle = project.PrfHandle!;
        string path = Path.Combine(filesPath!, handle);
        using (Stream file = File.Create(path))
        {
            await dto.File.CopyToAsync(file);
        }

        project.StateId = nextState;
        _ = db.SaveChanges();

        return true;
    }

    public bool Assign(AssignDto dto, Staff staff)
    {
        using ApplicationDbContext db = new();
        // validator guarantees this isn't null
        Project project = db.Project.FirstOrDefault(e => e.Id == dto.ProjectId)!;
        int nextState = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;

        bool isEcHead =
            db.StaffRole.FirstOrDefault(e => e.StaffId == staff.Id && e.RoleId == (int)Roles.EcHead)
            != null;
        if (!isEcHead)
        {
            return false;
        }

        if (project.StateId != (int)States.Assignment)
        {
            return false;
        }

        project.ProofreaderId = db.Staff.FirstOrDefault(e => e.Email == dto.ProofreaderEmail)!.Id;
        project.DeadlineDate = dto.Deadline;
        project.StateId = nextState;
        _ = db.SaveChanges();

        return true;
    }

    private static string? DetermineStaffAction(
        Staff staff,
        List<int> roleIds,
        Project project,
        ApplicationDbContext db
    )
    {
        return project.StateId switch
        {
            (int)States.InitialReview
            or (int)States.PrfReview
            or (int)States.PanelReview
                => staff.Id == project.InstructorId ? "Approve" : null,

            (int)States.ExdReview
                => db.School.FirstOrDefault(e => e.ExecDirId == staff.Id) != null
                    ? "Approve"
                    : null,

            (int)States.Assignment => roleIds.Contains((int)Roles.EcHead) ? "Assign" : null,
            (int)States.Proofreading => staff.Id == project.ProofreaderId ? "Approve" : null,
            (int)States.PrfCompletion => roleIds.Contains((int)Roles.EcHead) ? "Submit" : null,
            (int)States.Publishing => roleIds.Contains((int)Roles.Librarian) ? "Approve" : null,

            _ => null,
        };
    }

    private static ProjectViewModel ToViewModel(
        ApplicationDbContext db,
        Project project,
        string? action = null
    )
    {
        return new ProjectViewModel
        {
            Id = project.Id,
            Title = project.Title,
            Group = db.Group.FirstOrDefault(g => g.Id == project.GroupId)!.Name,
            HasPrf = project.PrfHandle != null,
            Abstract = project.Abstract,
            State = db.State.FirstOrDefault(s => s.Id == project.StateId)!.Name,
            StateDescription = db.State.FirstOrDefault(s => s.Id == project.StateId)!.Desc,
            School = db.School.FirstOrDefault(s => s.Id == project.SchoolId)!.Name,
            Subject = db.Subject.FirstOrDefault(s => s.Id == project.SubjectId)!.Name,
            Course = db.Course.FirstOrDefault(c => c.Id == project.CourseId)!.Name,
            Action = action
        };
    }

    public MarcxmlBuilder Publish(int id)
    {
        using ApplicationDbContext db = new();
        Project project = db.Project.FirstOrDefault(e => e.Id == id)!;
        return GenerateKohaRequest(db, project);
    }

    private static MarcxmlBuilder GenerateKohaRequest(ApplicationDbContext db, Project project)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string url = config.GetSection("Paths")["ApplicationUrl"]!;

        var allMembers =
            from project_ in db.Project
            join studentGroup in db.StudentGroup on project.GroupId equals studentGroup.GroupId
            join student in db.Student on studentGroup.StudentId equals student.Id
            where project_.Id == project.Id
            select new
            {
                GroupId = project_.GroupId,
                StudentId = student.Id,
                GivenName = student.GivenName,
                LastName = student.LastName
            } ;

        // 110: a: first author (group leader)
        var leader = (
            from member in allMembers
            join group_ in db.Group on member.GroupId equals group_.Id
            where member.StudentId == group_.LeaderId
            select new
            {
                Id = member.StudentId,
                GivenName = member.GivenName,
                LastName = member.LastName
            }
        ).FirstOrDefault()!;

        // 245: a) title, c) all authors
        var members = (
            from member in allMembers
            where member.StudentId != leader.Id
            select new
            {
                GivenName = member.GivenName,
                LastName = member.LastName
            }
        ).ToList();

        StringBuilder memberStringBuilder = new($"{leader.GivenName} {leader.LastName}");
        foreach (var member in members)
        {
            _ = memberStringBuilder.Append($", {member.GivenName} {member.LastName}");
        }

        MarcxmlBuilder builder = new();
        _ = builder
            .Add("110", ("a", $"{leader.GivenName} {leader.LastName}")) // first author
            .Add(
                "245",
                ("a", project.Title), // title
                ("c", memberStringBuilder.ToString()) // authors
            )
            .Add("520", ("a", project.Abstract)) // abstract
            .Add(
                "856",
                ("u", $"{url}/Projects/Download/{project.Id}"), // document url
                ("y", "Click to download document") // label (not standard)
            );

        return builder;
    }
}