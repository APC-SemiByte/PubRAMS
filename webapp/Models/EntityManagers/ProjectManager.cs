using webapp.Data;
using webapp.Models.Dtos;
using webapp.Models.ViewModels;

namespace webapp.Models.EntityManagers;

public class ProjectManager
{
    public void Add(SubmissionDto submission)
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

        _ = db.Project.Add(
            new()
            {
                Title = submission.Title,
                GroupId = groupId,
                DocumentUrl = submission.DocumentUrl,
                Abstract = submission.Abstract,
                StateId = 1,
                SchoolId = schoolId,
                SubjectId = subjectId,
                CourseId = courseId,
                InstructorId = instructorId,
                AdviserId = adviserId
            }
        );

        _ = db.SaveChanges();
    }

    public ProjectListViewModel GenerateProjectListViewModel(IUser user)
    {
        return user.GetType() == typeof(Student)
            ? GenerateProjectListViewModel((Student)user)
            : GenerateProjectListViewModel((Staff)user);
    }

    public ProjectListViewModel GenerateProjectListViewModel(Student student)
    {
        using ApplicationDbContext db = new();
        HashSet<int> groupIds = db.StudentGroup.Where(e => e.StudentId == student.Id)
            .Select(e => e.GroupId)
            .ToHashSet();

        List<ProjectViewModel> projects = db.Project.Where(e => groupIds.Contains(e.GroupId))
            .Select(
                e =>
                    new ProjectViewModel
                    {
                        Title = e.Title,
                        Group = db.Group.FirstOrDefault(g => g.Id == e.GroupId)!.Name,
                        DocumentUrl = e.DocumentUrl,
                        Abstract = e.Abstract,
                        State = db.State.FirstOrDefault(s => s.Id == e.StateId)!.Name,
                        School = db.School.FirstOrDefault(s => s.Id == e.SchoolId)!.Name,
                        Subject = db.Subject.FirstOrDefault(s => s.Id == e.SubjectId)!.Name,
                        Course = db.Course.FirstOrDefault(c => c.Id == e.CourseId)!.Name
                    }
            )
            .ToList();

        return new() { Projects = projects };
    }

    public ProjectListViewModel GenerateProjectListViewModel(Staff staff)
    {
        using ApplicationDbContext db = new();
        List<ProjectViewModel> projects = db.Project.Where(
            e =>
                e.InstructorId == staff.Id || e.AdviserId == staff.Id || e.ProofreaderId == staff.Id
        )
            .Select(
                e =>
                    new ProjectViewModel
                    {
                        Title = e.Title,
                        Group = db.Group.FirstOrDefault(g => g.Id == e.GroupId)!.Name,
                        DocumentUrl = e.DocumentUrl,
                        Abstract = e.Abstract,
                        State = db.State.FirstOrDefault(s => s.Id == e.StateId)!.Name,
                        School = db.School.FirstOrDefault(s => s.Id == e.SchoolId)!.Name,
                        Subject = db.Subject.FirstOrDefault(s => s.Id == e.SubjectId)!.Name,
                        Course = db.Course.FirstOrDefault(c => c.Id == e.CourseId)!.Name
                    }
            )
            .ToList();

        return new() { Projects = projects };
    }
}

