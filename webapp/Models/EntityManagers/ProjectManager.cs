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
            HasPrf = false,
            HasPdf = false,
            Edited = false,
            SchoolId = schoolId,
            SubjectId = subjectId,
            CourseId = courseId,
            InstructorId = instructorId,
            AdviserId = adviserId
        };

        _ = db.Project.Add(newProject);
        _ = db.SaveChanges();
        string handle = $"{submission.Group}-{newProject.Id}";
        newProject.BaseHandle = handle;
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
        project.Edited = true;

        if (dto.Prf != null && project.StateId >= (int)States.PrfStart)
        {
            project.HasPrf = true;
        }

        if (dto.Pdf != null && project.StateId == (int)States.Finalizing)
        {
            project.HasPdf = true;
        }

        _ = db.SaveChanges();

        return project.BaseHandle;
    }

    public string? GetSchool(int? id)
    {
        using ApplicationDbContext db = new();
        return (
            from project in db.Project
            join school in db.School on project.SchoolId equals school.Id
            where project.Id == id
            select school.Name
        ).FirstOrDefault();
    }

    public string? GetCourse(int? id)
    {
        using ApplicationDbContext db = new();
        return (
            from project in db.Project
            join course in db.Course on project.CourseId equals course.Id
            where project.Id == id
            select course.Code
        ).FirstOrDefault();
    }

    public string? GetSubject(int? id)
    {
        using ApplicationDbContext db = new();
        return (
            from project in db.Project
            join subject in db.Subject on project.SubjectId equals subject.Id
            where project.Id == id
            select subject.Code
        ).FirstOrDefault();
    }

    public string? GetAdviser(int? id)
    {
        using ApplicationDbContext db = new();
        return (
            from project in db.Project
            join adviser in db.Staff on project.AdviserId equals adviser.Id
            where project.Id == id
            select adviser.Email
        ).FirstOrDefault();
    }

    public string? GetInstructor(int? id)
    {
        using ApplicationDbContext db = new();
        return (
            from project in db.Project
            join instructor in db.Staff on project.InstructorId equals instructor.Id
            where project.Id == id
            select instructor.Email
        ).FirstOrDefault();
    }

    public string? GetGroup(int? id)
    {
        using ApplicationDbContext db = new();
        return (
            from project in db.Project
            join group_ in db.Group on project.GroupId equals group_.Id
            where project.Id == id
            select group_.Name
        ).FirstOrDefault();
    }

    public bool IsEditable(int id, string student)
    {
        using ApplicationDbContext db = new();
        int[] editable = [
            (int)States.InitialRevisions,
            (int)States.PrfStart,
            (int)States.ProofreadingRevisions,
            (int)States.PanelRevisions,
            (int)States.Finalizing,
        ];
        return (
            from project in db.Project
            join studentGroup in db.StudentGroup on project.GroupId equals studentGroup.GroupId
            where studentGroup.StudentId == student
                && project.Id == id
                && editable.Contains(project.StateId)
            select studentGroup
       ).Any();
    }

    public string GetBaseHandle(int projectId)
    {
        using ApplicationDbContext db = new();
        // validator guarantees this isn't null
        return db.Project.FirstOrDefault(e => e.Id == projectId)!.BaseHandle!;
    }

    public int GetStateId(int projectId)
    {
        using ApplicationDbContext db = new();
        // validator guarantees this isn't null
        return db.Project.FirstOrDefault(e => e.Id == projectId)!.StateId;
    }

    public ProjectViewModel? GenerateProjectViewModel(int id, IUser user)
    {
        return user.GetType() == typeof(Student)
            ? GenerateProjectViewModel(id, (Student)user)
            : GenerateProjectViewModel(id, (Staff)user);
    }

    private ProjectViewModel? GenerateProjectViewModel(int id, Student student)
    {
        using ApplicationDbContext db = new();
        Project? project = (
            from project_ in db.Project
            join studentGroup in db.StudentGroup on project_.GroupId equals studentGroup.GroupId
            where project_.Id == id && studentGroup.StudentId == student.Id
            select project_
        ).FirstOrDefault();

        if (project == null)
        {
            return null;
        }

        string? action = IsEditable(id, student.Id) ? "Submit" : null;
        return ToViewModel(db, project, action);
    }

    private ProjectViewModel? GenerateProjectViewModel(int id, Staff staff)
    {
        using ApplicationDbContext db = new();

        IQueryable<int> roleIds =
            from staffRole in db.StaffRole
            where staffRole.StaffId == staff.Id
            select staffRole.RoleId;

        Project? project = roleIds.Any(e => e == (int)Roles.EcHead)
            // EC head is involved with all projects. They should have a heads up
            ? db.Project.Where(e => e.Id == id && e.StateId <= (int)States.PrfCompletion).FirstOrDefault()
            // Everyone else has a limited view, only what they're involved with
            : db.Project.Where(
                e =>
                    e.Id == id &&
                    (
                        e.InstructorId == staff.Id
                        || e.AdviserId == staff.Id
                        || e.ProofreaderId == staff.Id
                        // Executive directors see everything in their school
                        || db.School.FirstOrDefault(s => s.Id == e.SchoolId)!.ExecDirId == staff.Id
                    )
            ).FirstOrDefault();

        if (project == null)
        {
            return null;
        }

        string? action = DetermineStaffAction(staff, roleIds, project, db);
        return ToViewModel(db, project, action);
    }

    public EditSubmissionDto? GenerateEditSubmissionDto(int id, string student)
    {
        using ApplicationDbContext db = new();
        return (
            from project_ in db.Project
            join group_ in db.Group on project_.GroupId equals group_.Id
            join studentGroup in db.StudentGroup on project_.GroupId equals studentGroup.GroupId
            where project_.Id == id && studentGroup.StudentId == student
            select new EditSubmissionDto
            {
                Id = project_.Id,
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

        IQueryable<int> roleIds =
            from staffRole in db.StaffRole
            where staffRole.StaffId == staff.Id
            select staffRole.RoleId;

        List<Project> projects = roleIds.Any(e => e == (int)Roles.EcHead)
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
            ).ToList();

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
                notUrgent.Add(ToViewModel(db, project));
            }
        }

        return new() { UrgentProjects = urgent, Projects = notUrgent };
    }

    public bool Accept(int id, string staffId)
    {
        return AcceptOrReject(id, staffId) != null;
    }

    public string? Reject(int id, string staffId, RejectDto dto)
    {
        return AcceptOrReject(id, staffId, dto);
    }

    public bool IsSubmittable(int id, string studentId)
    {
        using ApplicationDbContext db = new();
        Project? project = (
            from project1 in db.Project
            join studentGroup in db.StudentGroup on project1.GroupId equals studentGroup.GroupId
            where project1.Id == id && studentGroup.StudentId == studentId
            select project1
        ).FirstOrDefault();

        if (project == null)
        {
            return false;
        }

        int[] submittabledStates = [
            (int)States.InitialRevisions,
            (int)States.ProofreadingRevisions,
            (int)States.PanelRevisions,
            (int)States.Finalizing,
            (int)States.PrfStart
        ];

        if (!submittabledStates.Any(e => e == project.StateId))
        {
            return false;
        }

        if (project.StateId >= (int)States.PrfStart
            && !project.HasPrf)
        {
            return false;
        }

        return project.Edited;
    }

    public bool Submit(int id, string studentId)
    {
        using ApplicationDbContext db = new();
        Project? project = (
            from project1 in db.Project
            join studentGroup in db.StudentGroup on project1.GroupId equals studentGroup.GroupId
            where project1.Id == id && studentGroup.StudentId == studentId
            select project1
        ).FirstOrDefault();

        if (project == null)
        {
            return false;
        }

        int nextState = (
            from state in db.State
            where state.Id == project.StateId
            select state.AcceptStateId
        ).FirstOrDefault();

        project.StateId = nextState;
        _ = db.SaveChanges();

        return true;
    }

    private static string? AcceptOrReject(
        int id,
        string staffId,
        RejectDto? dto = null
    )
    {
        using ApplicationDbContext db = new();

        // validator guarantees this is not null
        Project project = db.Project.FirstOrDefault(e => e.Id == id)!;

        int acceptId = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;
        int rejectId = db.State.FirstOrDefault(e => e.Id == project.StateId)!.RejectStateId;

        string handle;

        switch (project.StateId)
        {
            case (int)States.InitialReview:
                if (staffId != project.InstructorId)
                {
                    return null;
                }
                // TODO: state-specific operations
                handle = project.BaseHandle!;
                break;

            case (int)States.PrfReview:
                if (staffId != project.InstructorId)
                {
                    return null;
                }
                // TODO: state-specific operations
                handle = $"{project.BaseHandle}-Prf.pdf";
                break;

            case (int)States.ExdReview:
                if (staffId != db.School.FirstOrDefault(e => e.Id == project.SchoolId)!.ExecDirId)
                {
                    return null;
                }
                // TODO: state-specific operations
                handle = project.BaseHandle!;
                break;

            case (int)States.Proofreading:
                if (staffId != project.ProofreaderId)
                {
                    return null;
                }
                // TODO: state-specific operations
                handle = project.BaseHandle!;
                break;

            case (int)States.PanelReview:
                if (staffId != project.InstructorId)
                {
                    return null;
                }
                // TODO: state-specific operations
                handle = project.BaseHandle!;
                break;

            case (int)States.PrfCompletion:
                if (
                    db.StaffRole.FirstOrDefault(
                        e => e.StaffId == staffId && e.RoleId == (int)Roles.EcHead
                    ) == null
                )
                {
                    return null;
                }
                // TODO: state-specific operations
                handle = project.BaseHandle!;
                break;

            case (int)States.Publishing:
                if (
                    db.StaffRole.FirstOrDefault(
                        e => e.StaffId == staffId && e.RoleId == (int)Roles.Librarian
                    ) == null
                )
                {
                    return null;
                }
                // TODO: state-specific operations
                handle = project.BaseHandle!;
                break;

            default:
                return null;
        }

        project.StudentComment = null;
        project.StaffComment = null;
        project.Edited = false;

        if (dto == null)
        {
            project.StateId = acceptId;
            _ = db.SaveChanges();
            return project.BaseHandle;
        }

        if (dto.Comment != null)
        {
            project.StaffComment = dto.Comment;
        }

        project.StateId = rejectId;
        _ = db.SaveChanges();

        return project.BaseHandle;
    }

    public string CompletePrf(CompletePrfDto dto)
    {
        using ApplicationDbContext db = new();
        // dont't need to check authorization, only EcHead can access the endpoint
        // validator guarantees this isn't null
        Project project = db.Project.FirstOrDefault(e => e.Id == dto.ProjectId)!;
        int nextState = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;

        project.StateId = nextState;
        _ = db.SaveChanges();

        // PrfStart should've generated one already
        return project.BaseHandle!;
    }

    public bool Assign(int id, AssignDto dto, Staff staff)
    {
        using ApplicationDbContext db = new();
        // validator guarantees this isn't null
        Project project = db.Project.FirstOrDefault(e => e.Id == id)!;
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
        IQueryable<int> roleIds,
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

            (int)States.Assignment => roleIds.Any(e => e == (int)Roles.EcHead) ? "Assign" : null,
            (int)States.Proofreading => staff.Id == project.ProofreaderId ? "Approve" : null,
            (int)States.PrfCompletion => roleIds.Any(e => e == (int)Roles.EcHead) ? "CompletePrf" : null,
            (int)States.Publishing => roleIds.Any(e => e == (int)Roles.Librarian) ? "Publish" : null,

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
            HasPrf = project.HasPrf,
            HasPdf = project.HasPdf,
            Abstract = project.Abstract,
            StateId = project.StateId,
            State = db.State.FirstOrDefault(s => s.Id == project.StateId)!.Name,
            StateDescription = db.State.FirstOrDefault(s => s.Id == project.StateId)!.Desc,
            School = db.School.FirstOrDefault(s => s.Id == project.SchoolId)!.Name,
            Subject = db.Subject.FirstOrDefault(s => s.Id == project.SubjectId)!.Name,
            StaffComment = project.StaffComment,
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
            };

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