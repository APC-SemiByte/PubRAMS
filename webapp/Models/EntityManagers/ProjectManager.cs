using webapp.Data;
using webapp.Helpers;
using webapp.Models.Dtos;
using webapp.Models.ViewModels;

using System.Text;

namespace webapp.Models.EntityManagers;

/// <remarks>
/// Only supply <see cref="Project" />s taken from <see cref="Get" />
/// </remarks>
public class ProjectManager
{
    public Project Add(SubmissionDto submission)
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
            BaseHandle = string.Empty,
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
        newProject.BaseHandle = $"{submission.Group}-{newProject.Id}";
        _ = db.SaveChanges();

        return newProject;
    }

    public Project? Get(int? id, IUser? user)
    {
        if (user == null)
        {
            return null;
        }

        return user.GetType() == typeof(Student)
            ? Get(id, (Student)user)
            : Get(id, (Staff)user);
    }

    public Project? Get(int? id, Student? student)
    {
        if (student == null)
        {
            return null;
        }

        using ApplicationDbContext db = new();
        return (
            from project in db.Project
            join studentGroup in db.StudentGroup on project.GroupId equals studentGroup.GroupId
            where project.Id == id && studentGroup.StudentId == student.Id
            select project
       ).FirstOrDefault();
    }

    public Project? Get(int? id, Staff? staff)
    {
        if (staff == null)
        {
            return null;
        }

        using ApplicationDbContext db = new();

        Project? project = db.Project.FirstOrDefault(e => e.Id == id);
        if (project == null)
        {
            return null;
        }

        IQueryable<int> roleIds =
            from staffRole in db.StaffRole
            where staffRole.StaffId == staff.Id
            select staffRole.RoleId;

        bool isInvolved = roleIds.Contains((int)Roles.EcHead)
            ? project.StateId <= (int)States.PrfCompletion
            : project.InstructorId == staff.Id
                || project.AdviserId == staff.Id
                || project.ProofreaderId == staff.Id
                || db.School.FirstOrDefault(e => e.Id == project.SchoolId)!.ExecDirId == staff.Id;

        return isInvolved ? project : null;
    }

    public string? GetSchoolName(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.School.FirstOrDefault(e => e.Id == project.SchoolId)!.Name;
    }

    public string? GetCourseCode(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Course.FirstOrDefault(e => e.Id == project.CourseId)!.Code;
    }

    public string? GetSubjectCode(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Subject.FirstOrDefault(e => e.Id == project.SubjectId)!.Code;
    }

    public string? GetAdviserEmail(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Staff.FirstOrDefault(e => e.Id == project.AdviserId)!.Email;
    }

    public string? GetInstructorEmail(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Staff.FirstOrDefault(e => e.Id == project.InstructorId)!.Email;
    }

    public string? GetGroupName(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Group.FirstOrDefault(e => e.Id == project.GroupId)!.Name;
    }

    public bool IsEditable(Project project)
    {
        using ApplicationDbContext db = new();
        int[] editable = [
            (int)States.InitialRevisions,
            (int)States.PrfStart,
            (int)States.ProofreadingRevisions,
            (int)States.PanelRevisions,
            (int)States.Finalizing,
        ];
        return editable.Contains(project.StateId);
    }

    public bool IsSubmittable(Project project)
    {
        using ApplicationDbContext db = new();

        int[] submittabledStates = [
            (int)States.InitialRevisions,
            (int)States.ProofreadingRevisions,
            (int)States.PanelRevisions,
            (int)States.Finalizing,
            (int)States.PrfStart
        ];

        if (!submittabledStates.Contains(project.StateId))
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

    public StateViewModel GenerateStateViewModel(Project project)
    {
        using ApplicationDbContext db = new();
        return db.State
            .Select(e => new StateViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Desc = e.Desc
            })
            .FirstOrDefault(e => e.Id == project.StateId)!;
    }

    public ProjectViewModel GenerateProjectViewModel(Project project, IUser user)
    {
        return user.GetType() == typeof(Student)
            ? GenerateProjectViewModel(project)
            : GenerateProjectViewModel(project, (Staff)user);
    }

    private ProjectViewModel GenerateProjectViewModel(Project project)
    {
        using ApplicationDbContext db = new();
        string? action = IsEditable(project) ? "Submit" : null;
        return ToViewModel(db, project, action);
    }

    private ProjectViewModel GenerateProjectViewModel(Project project, Staff staff)
    {
        using ApplicationDbContext db = new();

        IQueryable<int> roleIds =
            from staffRole in db.StaffRole
            where staffRole.StaffId == staff.Id
            select staffRole.RoleId;

        string? action = DetermineStaffAction(db, staff, roleIds, project);
        return ToViewModel(db, project, action);
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
            where groupIds.Contains(project.GroupId)
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

    private ProjectListViewModel GenerateProjectListViewModel(Staff staff)
    {
        using ApplicationDbContext db = new();

        IQueryable<int> roleIds =
            from staffRole in db.StaffRole
            where staffRole.StaffId == staff.Id
            select staffRole.RoleId;

        List<Project> projects = roleIds.Contains((int)Roles.EcHead)
            // EC head is involved with all projects until he has signed their PRF
            ? db.Project.Where(e => e.StateId <= (int)States.PrfCompletion).ToList()
            // Everyone else has a limited view, only what they're involved with
            : db.Project.Where(
                e =>
                    e.InstructorId == staff.Id
                    || e.AdviserId == staff.Id
                    || e.ProofreaderId == staff.Id
                    // Executive directors is involved in everything in their school
                    || db.School.FirstOrDefault(s => s.Id == e.SchoolId)!.ExecDirId == staff.Id
            ).ToList();

        List<ProjectViewModel> urgent = [];
        List<ProjectViewModel> notUrgent = [];

        foreach (Project project in projects)
        {
            string? action = DetermineStaffAction(db, staff, roleIds, project);

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

    public SubmissionDto GenerateEditSubmissionDto(Project project)
    {
        using ApplicationDbContext db = new();
        return (
            from project_ in db.Project
            join group_ in db.Group on project_.GroupId equals group_.Id
            where project_.Id == project.Id
            select new SubmissionDto
            {
                Title = project_.Title,
                Group = group_.Name,
                Abstract = project_.Abstract,
                Comment = project_.StudentComment,

                // will be ignored, we use a dynamically loaded dropdown for these
                School = "",
                Subject = "",
                Course = "",
                AdviserEmail = "",
                InstructorEmail = "",
            }
        ).FirstOrDefault()!;
    }

    public BiblioDto GenerateBiblioDto(Project project)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string url = config.GetSection("Paths")["ApplicationUrl"]!;

        using ApplicationDbContext db = new();

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

        return new()
        {
            Lead = $"{leader.GivenName} {leader.LastName}",
            Title = project.Title,
            Authors = memberStringBuilder.ToString(),
            PublishPlace = "Manila",
            Publisher = "Asia Pacific College",
            Date = "2024",
            Summary = project.Abstract,
            Uri = $"{url}/Projects/Download/{project.Id}",
            LinkText = "Click to download document",
            ItemType = "THESIS",

            // we want to trigger the validator
            Topic = string.Empty,
            Subdivision = string.Empty
        };
    }

    public BiblioItemDto GenerateBiblioItemDto(int recordId)
    {
        return new()
        {
            Id = recordId,
            HomeLibrary = "Asia Pacific College",
            CurrentLibrary = "Asia Pacific College",
            ShelvingLocation = "Research Section",
            CopyNumber = "c.1",
            KohaItemType = "THESES",

            // we want to trigger the validator
            CallNumber = string.Empty,
            AccessionNumber = string.Empty
        };
    }

    public static MarcxmlBuilder GenerateKohaRequest(BiblioDto dto)
    {
        MarcxmlBuilder builder = new();
        return builder
            .Add("100", [("a", dto.Lead)])
            .Add("245", [("a", dto.Title), ("c", dto.Authors)])
            .Add("264", [("a", dto.PublishPlace), ("b", dto.Publisher)], ind2: "1")
            .Add("264", [("c", dto.Date)], ind2: "4")
            .Add("520", [("a", dto.Summary)])
            .Add("650", [("a", dto.Topic), ("x", dto.Subdivision)])
            .Add("856", [("u", dto.Uri), ("y", dto.LinkText)])
            .Add("942", [("c", dto.ItemType)]);
    }

    public void Edit(Project project, SubmissionDto dto)
    {
        using ApplicationDbContext db = new();

        int schoolId = db.School.FirstOrDefault(e => e.Name == dto.School)!.Id;
        int subjectId = db.Subject.FirstOrDefault(e => e.Code == dto.Subject)!.Id;
        int courseId = db.Course.FirstOrDefault(e => e.Code == dto.Course)!.Id;

        int groupId = db.Group.FirstOrDefault(e => e.Name == dto.Group)!.Id;
        string instructorId = db.Staff.FirstOrDefault(
            e => e.Email == dto.InstructorEmail
        )!.Id;
        string adviserId = db.Staff.FirstOrDefault(e => e.Email == dto.InstructorEmail)!.Id;

        // needed to track changes
        Project project1 = db.Project.FirstOrDefault(e => e.Id == project.Id)!;
        project1.Title = dto.Title;
        project1.GroupId = groupId;
        project1.Abstract = dto.Abstract;
        project1.SchoolId = schoolId;
        project1.SubjectId = subjectId;
        project1.CourseId = courseId;
        project1.InstructorId = instructorId;
        project1.AdviserId = adviserId;
        project1.Edited = true;
        project1.StudentComment = dto.Comment;

        if (dto.Prf != null && project1.StateId >= (int)States.PrfStart)
        {
            project1.HasPrf = true;
        }

        if (dto.Pdf != null && project1.StateId == (int)States.Finalizing)
        {
            project1.HasPdf = true;
        }

        _ = db.SaveChanges();
    }

    public bool Accept(Project project, IUser user)
    {
        return AcceptOrReject(project, (Staff)user);
    }

    public bool Accept(Project project, Staff staff)
    {
        return AcceptOrReject(project, staff);
    }

    public bool Reject(Project project, IUser user, RejectDto dto)
    {
        return AcceptOrReject(project, (Staff)user, dto);
    }

    public bool Reject(Project project, Staff staff, RejectDto dto)
    {
        return AcceptOrReject(project, staff, dto);
    }

    private static bool AcceptOrReject(
        Project project,
        Staff staff,
        RejectDto? dto = null
    )
    {
        using ApplicationDbContext db = new();

        State currentState = db.State.FirstOrDefault(e => e.Id == project.StateId)!;
        int acceptId = currentState.AcceptStateId;
        int rejectId = currentState.RejectStateId;

        switch (project.StateId)
        {
            case (int)States.InitialReview:
                if (staff.Id != project.InstructorId)
                {
                    return false;
                }
                // TODO: state-specific operations
                break;

            case (int)States.PrfReview:
                if (staff.Id != project.InstructorId)
                {
                    return false;
                }
                // TODO: state-specific operations
                break;

            case (int)States.ExdReview:
                if (staff.Id != db.School.FirstOrDefault(e => e.Id == project.SchoolId)!.ExecDirId)
                {
                    return false;
                }
                // TODO: state-specific operations
                break;

            case (int)States.Proofreading:
                if (staff.Id != project.ProofreaderId)
                {
                    return false;
                }
                // TODO: state-specific operations
                break;

            case (int)States.PanelReview:
                if (staff.Id != project.InstructorId)
                {
                    return false;
                }
                // TODO: state-specific operations
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
                break;

            default:
                return false;
        }

        // needed to track changes
        Project project1 = db.Project.FirstOrDefault(e => e.Id == project.Id)!;
        project1.StudentComment = null;
        project1.StaffComment = null;
        project1.Edited = false;

        if (dto == null)
        {
            project1.StateId = acceptId;
            _ = db.SaveChanges();
            return true;
        }

        if (dto.Comment != null)
        {
            project1.StaffComment = dto.Comment;
        }

        project1.StateId = rejectId;
        _ = db.SaveChanges();

        return true;
    }

    public bool Submit(Project project, IUser user)
    {
        return Submit(project, (Student)user);
    }

    public bool Submit(Project project, Student student)
    {
        using ApplicationDbContext db = new();

        bool isInvolved = db.StudentGroup.Any(e =>
                e.StudentId == student.Id
                && e.GroupId == project.GroupId
        );
        if (!isInvolved)
        {
            return false;
        }

        int nextState = db.State
            .FirstOrDefault(e => e.Id == project.StateId)!
            .AcceptStateId;

        // needed to track changes
        Project project1 = db.Project.FirstOrDefault(e => e.Id == project.Id)!;
        project1.StateId = nextState;
        _ = db.SaveChanges();

        return true;
    }

    public bool Assign(Project project, AssignDto dto, IUser user)
    {
        return Assign(project, dto, (Staff)user);
    }

    public bool Assign(Project project, AssignDto dto, Staff staff)
    {
        using ApplicationDbContext db = new();

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

        int nextState = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;

        // needed to track changes
        Project project1 = db.Project.FirstOrDefault(e => e.Id == project.Id)!;
        project1.StateId = nextState;
        project1.ProofreaderId = db.Staff.FirstOrDefault(e => e.Email == dto.ProofreaderEmail)!.Id;
        project1.DeadlineDate = dto.Deadline;

        _ = db.SaveChanges();

        return true;
    }

    public void CompletePrf(Project project)
    {
        using ApplicationDbContext db = new();

        int nextState = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;

        // needed to track changes
        Project project1 = db.Project.FirstOrDefault(e => e.Id == project.Id)!;
        project1.StateId = nextState;
        _ = db.SaveChanges();
    }

    private static string? DetermineStaffAction(
        ApplicationDbContext db,
        Staff staff,
        IQueryable<int> roleIds,
        Project project
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
            (int)States.PrfCompletion => roleIds.Contains((int)Roles.EcHead) ? "CompletePrf" : null,
            (int)States.Publishing => roleIds.Contains((int)Roles.Librarian) ? "Publish" : null,

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
            StudentComment = project.StudentComment,
            Course = db.Course.FirstOrDefault(c => c.Id == project.CourseId)!.Name,
            Action = action
        };
    }
}