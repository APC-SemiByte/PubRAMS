using System.Text;

using webapp.Data;
using webapp.Helpers;
using webapp.Models.Dtos;
using webapp.Models.ViewModels;

namespace webapp.Models.EntityManagers;

/// <remarks>
/// Only supply <see cref="Project" />s taken from <see cref="Get" />
/// </remarks>
public class ProjectManager
{
    public Project Add(SubmissionDto dto)
    {
        using ApplicationDbContext db = new();

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string term = config.GetSection("Variables")["Term"]!;

        Project newProject = new()
        {
            Title = dto.Title,
            Abstract = dto.Abstract,

            Tags = dto.Tags,
            CategoryId = db.Category.FirstOrDefault(e => e.Name == dto.Category)!.Id,

            Continued = dto.Continued,
            StateId = (int)States.InitialReview,
            CompletionId = (int)Completions.Unfinished,

            GroupId = db.Group.FirstOrDefault(e => e.Name == dto.Group)!.Id,

            BaseHandle = string.Empty,
            HasPrf = false,
            HasPdf = false,
            Edited = false,

            SchoolId = db.School.FirstOrDefault(e => e.Name == dto.School)!.Id,
            SubjectId = db.Subject.FirstOrDefault(e => e.Code == dto.Subject)!.Id,
            CourseId = db.Course.FirstOrDefault(e => e.Code == dto.Course)!.Id,

            InstructorId = db.Staff.FirstOrDefault(e => e.Email == dto.InstructorEmail)!.Id,
            AdviserId = db.Staff.FirstOrDefault(e => e.Email == dto.InstructorEmail)!.Id,

            Term = term
        };

        _ = db.Project.Add(newProject);
        _ = db.SaveChanges();
        newProject.BaseHandle = $"{dto.Group}-{newProject.Id}";
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

        if (project.InstructorId == staff.Id
            || project.AdviserId == staff.Id
            || project.ProofreaderId == staff.Id)
        {
            return project;
        }

        if (roleIds.Contains((int)Roles.ExecutiveDirector))
        {
            return staff.Id == db.School.FirstOrDefault(e => e.Id == project.SchoolId)!.ExecDirId
                ? project : null;
        }

        if (roleIds.Contains((int)Roles.Librarian))
        {
            return project.StateId <= (int)States.Publishing ? project : null;
        }

        if (roleIds.Contains((int)Roles.EcHead))
        {
            return project.StateId <= (int)States.PrfCompletion ? project : null;
        }

        return null;
    }

    public string? GetCategory(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Category.FirstOrDefault(e => e.Id == project.CategoryId)?.Name;
    }

    public string? GetCompletion(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Completion.FirstOrDefault(e => e.Id == project.CompletionId)?.Name;
    }

    public string? GetSchoolName(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.School.FirstOrDefault(e => e.Id == project.SchoolId)?.Name;
    }

    public string? GetCourseCode(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Course.FirstOrDefault(e => e.Id == project.CourseId)?.Code;
    }

    public string? GetSubjectCode(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Subject.FirstOrDefault(e => e.Id == project.SubjectId)?.Code;
    }

    public string? GetAdviserEmail(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Staff.FirstOrDefault(e => e.Id == project.AdviserId)?.Email;
    }

    public string? GetInstructorEmail(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Staff.FirstOrDefault(e => e.Id == project.InstructorId)?.Email;
    }

    public string? GetGroupName(Project? project)
    {
        if (project == null)
        {
            return null;
        }
        using ApplicationDbContext db = new();
        return db.Group.FirstOrDefault(e => e.Id == project.GroupId)?.Name;
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

    private static ProjectViewModel GenerateProjectViewModel(Project project, Staff staff)
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

    private static ProjectListViewModel GenerateProjectListViewModel(Staff staff)
    {
        using ApplicationDbContext db = new();

        IQueryable<int> roleIds =
            from staffRole in db.StaffRole
            where staffRole.StaffId == staff.Id
            select staffRole.RoleId;

        List<Project> projects;

        if (roleIds.Contains((int)Roles.ExecutiveDirector))
        {
            projects = (
                from project in db.Project
                where project.InstructorId == staff.Id
                    || project.AdviserId == staff.Id
                    || project.ProofreaderId == staff.Id
                    || db.School.FirstOrDefault(e => e.Id == project.SchoolId)!
                        .ExecDirId == staff.Id
                select project
            ).ToList();
        }
        else if (roleIds.Contains((int)Roles.Librarian))
        {
            projects = (
                from project in db.Project
                where project.InstructorId == staff.Id
                    || project.AdviserId == staff.Id
                    || project.ProofreaderId == staff.Id
                    || project.StateId <= (int)States.Publishing
                select project
            ).ToList();
        }
        else if (roleIds.Contains((int)Roles.EcHead))
        {
            projects = (
                from project in db.Project
                where project.InstructorId == staff.Id
                    || project.AdviserId == staff.Id
                    || project.ProofreaderId == staff.Id
                    || project.StateId <= (int)States.PrfCompletion
                select project
            ).ToList();
        }
        else
        {
            projects = (
                from project in db.Project
                where project.InstructorId == staff.Id
                    || project.AdviserId == staff.Id
                    || project.ProofreaderId == staff.Id
                select project
            ).ToList();
        }

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
            join category in db.Category on project_.CategoryId equals category.Id
            where project_.Id == project.Id
            select new SubmissionDto
            {
                Title = project_.Title,
                Abstract = project_.Abstract,
                Tags = project_.Tags,
                Continued = project_.Continued,
                Group = group_.Name,
                Comment = project_.StudentComment,

                // will be ignored, we use a dynamically loaded dropdown for these
                Category = string.Empty,
                School = string.Empty,
                Subject = string.Empty,
                Course = string.Empty,
                AdviserEmail = string.Empty,
                InstructorEmail = string.Empty,
            }
        ).FirstOrDefault()!;
    }

    public BiblioDto? GenerateBiblioDto(Project project)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string url = config.GetSection("Paths")["ApplicationUrl"]!;

        using ApplicationDbContext db = new();

        if (project.StateId != (int)States.Publishing)
        {
            return null;
        }

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

        // needed to track changes
        Project project1 = db.Project.FirstOrDefault(e => e.Id == project.Id)!;

        project1.Title = dto.Title;
        project1.Abstract = dto.Abstract;
        project1.Tags = dto.Tags;
        project1.CategoryId = db.Category.FirstOrDefault(e => e.Id == project.CategoryId)!.Id;
        project1.Continued = dto.Continued;
        project1.GroupId = db.Group.FirstOrDefault(e => e.Name == dto.Group)!.Id;
        project1.SchoolId = db.School.FirstOrDefault(e => e.Name == dto.School)!.Id;
        project1.SubjectId = db.Subject.FirstOrDefault(e => e.Code == dto.Subject)!.Id;
        project1.CourseId = db.Course.FirstOrDefault(e => e.Code == dto.Course)!.Id;
        project1.InstructorId = db.Staff.FirstOrDefault(e => e.Email == dto.InstructorEmail)!.Id;
        project1.AdviserId = db.Staff.FirstOrDefault(e => e.Email == dto.InstructorEmail)!.Id;
        project1.StudentComment = dto.Comment;
        project1.Edited = true;

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
        project1.Edited = false;

        if (dto == null)
        {
            project1.StudentComment = null;
            project1.StaffComment = null;

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

        if (project.StateId != (int)States.Assignment
            && project.StateId != (int)States.Proofreading
            && project.StateId != (int)States.ProofreadingRevisions)
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

    public bool CompletePrf(Project project, IUser user)
    {
        return CompletePrf(project, (Staff)user);
    }

    public bool CompletePrf(Project project, Staff staff)
    {
        using ApplicationDbContext db = new();

        bool isEcHead =
            db.StaffRole.FirstOrDefault(e => e.StaffId == staff.Id && e.RoleId == (int)Roles.EcHead)
            != null;

        if (!isEcHead)
        {
            return false;
        }

        if (project.StateId != (int)States.PrfCompletion)
        {
            return false;
        }

        int nextState = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;

        // needed to track changes
        Project project1 = db.Project.FirstOrDefault(e => e.Id == project.Id)!;
        project1.StateId = nextState;
        _ = db.SaveChanges();

        return true;
    }

    public void SaveKohaRecordId(Project project, int id)
    {
        using ApplicationDbContext db = new();
        Project project1 = db.Project.FirstOrDefault(e => e.Id == project.Id)!;
        project1.KohaRecordId = id;
        _ = db.SaveChanges();
    }

    public void MarkPublished(Project project)
    {
        using ApplicationDbContext db = new();
        Project project1 = db.Project.FirstOrDefault(e => e.Id == project.Id)!;
        project1.StateId = (int)States.Published;
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
        Group group = db.Group.FirstOrDefault(e => e.Id == project.GroupId)!;
        return new ProjectViewModel
        {
            Id = project.Id,
            Title = project.Title,
            Tags = project.Tags,
            Category = db.Category.FirstOrDefault(e => e.Id == project.CategoryId)!.Name,
            HasPrf = project.HasPrf,
            HasPdf = project.HasPdf,
            Abstract = project.Abstract,
            Group = GroupManager.GenerateGroupViewModel(db, group),
            School = db.School.FirstOrDefault(s => s.Id == project.SchoolId)!.Name,
            Subject = db.Subject.FirstOrDefault(s => s.Id == project.SubjectId)!.Name,
            StaffComment = project.StaffComment,
            StudentComment = project.StudentComment,
            Course = db.Course.FirstOrDefault(c => c.Id == project.CourseId)!.Name,
            Action = action,
            State = db.State.Select(e => new StateViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Desc = e.Desc
            }).FirstOrDefault(e => e.Id == project.StateId)!,
        };
    }
}