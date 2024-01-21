using webapp.Data;
using webapp.Models.Dtos;
using webapp.Models.ViewModels;

namespace webapp.Models.EntityManagers;

public class ProjectManager
{
    public void Add(SubmissionDto submission, Student student)
    {
        using ApplicationDbContext db = new();
    }

    public ProjectListViewModel GenerateProjectListViewModel(IUser user)
    {
        return user.GetType() switch
        {
            Type type when type == typeof(Student) => GenerateProjectListViewModel((Student)user),
            _ => new ProjectListViewModel { Projects = [] },
        };
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
}