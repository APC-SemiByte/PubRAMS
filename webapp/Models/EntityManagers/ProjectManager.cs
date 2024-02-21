using webapp.Data;
using webapp.Models.Dtos;
using webapp.Models.ViewModels;

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
        _ = newProject.DocumentHandle = handle;
        _ = db.SaveChanges();

        return handle;
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
        HashSet<int> groupIds = db.StudentGroup.Where(e => e.StudentId == student.Id)
            .Select(e => e.GroupId)
            .ToHashSet();

        HashSet<int> urgentStates =
        [
            (int)States.InitialRevisions,
            (int)States.PrfStart,
            (int)States.ProofreadingRevisions,
            (int)States.PanelRevisions,
        ];

        List<ProjectViewModel> urgent = [];
        List<ProjectViewModel> notUrgent = [];

        List<Project> projects = db.Project.Where(e => groupIds.Contains(e.GroupId)).ToList();

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
        HashSet<int> roleIds = db.StaffRole.Where(e => e.StaffId == staff.Id)
            .Select(e => e.RoleId)
            .ToHashSet();

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
            string? action = DetermineAction(staff, roleIds, project, db);

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

    public bool Accept(int projectId, Staff staff)
    {
        return AcceptOrReject(projectId, staff, true);
    }

    public bool Reject(int projectId, Staff staff)
    {
        return AcceptOrReject(projectId, staff, false);
    }

    private static bool AcceptOrReject(int projectId, Staff staff, bool accept)
    {
        using ApplicationDbContext db = new();

        // validator guarantees this is not null
        Project project = db.Project.FirstOrDefault(e => e.Id == projectId)!;

        int acceptId = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;
        int rejectId = db.State.FirstOrDefault(e => e.Id == project.StateId)!.RejectStateId;

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
                if (db.StaffRole.FirstOrDefault(e => e.StaffId == staff.Id && e.RoleId == (int)Roles.EcHead) == null)
                {
                    return false;
                }
                // TODO: state-specific operations
                break;

            default:
                return false;
        }

        project.StateId = accept ? acceptId : rejectId;
        _ = db.SaveChanges();

        return true;
    }

    public bool Submit(int projectId, IUser user)
    {
        return user.GetType() == typeof(Student)
            ? Submit(projectId, (Student)user)
            : Submit(projectId, (Staff)user);
    }

    private static bool Submit(int projectId, Student student)
    {
        using ApplicationDbContext db = new();
        // validator guarantees this isn't null
        Project project = db.Project.FirstOrDefault(e => e.Id == projectId)!;
        int nextState = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;

        bool studentInGroup = db.StudentGroup.FirstOrDefault(e => e.StudentId == student.Id && e.GroupId == project.GroupId) != null;
        if (!studentInGroup)
        {
            return false;
        }

        switch (project.StateId)
        {
            case (int)States.InitialRevisions:
            case (int)States.PrfStart:
            case (int)States.ProofreadingRevisions:
            case (int)States.PanelRevisions:
                // split these up when we're doing state-specific operations
                // TODO: state-specific operations
                break;

            default:
                return false;
        }

        project.StateId = nextState;
        _ = db.SaveChanges();

        return true;
    }

    private static bool Submit(int projectId, Staff staff)
    {
        using ApplicationDbContext db = new();
        // validator guarantees this isn't null
        Project project = db.Project.FirstOrDefault(e => e.Id == projectId)!;
        HashSet<int> roles = db.StaffRole.Where(e => e.StaffId == staff.Id).Select(e => e.RoleId).ToHashSet();
        int nextState = db.State.FirstOrDefault(e => e.Id == project.StateId)!.AcceptStateId;

        switch (project.StateId)
        {
            case (int)States.PrfCompletion:
                if (!roles.Contains((int)Roles.EcHead))
                {
                    return false;
                }
                // TODO: state-specific operations
                break;

            case (int)States.Publishing:
                if (!roles.Contains((int)Roles.Librarian))
                {
                    return false;
                }
                // TODO: state-specific operations
                break;

            default:
                return false;
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

        bool isEcHead = db.StaffRole.FirstOrDefault(e => e.StaffId == staff.Id && e.RoleId == (int)Roles.EcHead) != null;
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

    private static string? DetermineAction(
        Staff staff,
        HashSet<int> roleIds,
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
            (int)States.Publishing => roleIds.Contains((int)Roles.Librarian) ? "Submit" : null,

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
            DocumentHandle = project.DocumentHandle!,
            Abstract = project.Abstract,
            State = db.State.FirstOrDefault(s => s.Id == project.StateId)!.Name,
            School = db.School.FirstOrDefault(s => s.Id == project.SchoolId)!.Name,
            Subject = db.Subject.FirstOrDefault(s => s.Id == project.SubjectId)!.Name,
            Course = db.Course.FirstOrDefault(c => c.Id == project.CourseId)!.Name,
            Action = action
        };
    }
}