using webapp.Data;
using webapp.Models.ViewModels;

namespace webapp.Models.EntityManagers;

public class StudentManager : IUserManager<Student>
{
    public bool DbContains(string id)
    {
        using ApplicationDbContext db = new();
        Student? student = db.Student.FirstOrDefault(e => e.Id == id);
        return student != null;
    }

    public void Add(Student user)
    {
        using ApplicationDbContext db = new();
        _ = db.Student.Add(user);
        _ = db.SaveChanges();
    }

    public Student? GetById(string id)
    {
        using ApplicationDbContext db = new();
        return db.Student.FirstOrDefault(e => e.Id == id);
    }

    public Student? GetByEmail(string email)
    {
        using ApplicationDbContext db = new();
        return db.Student.FirstOrDefault(e => e.Email == email);
    }

    public StudentListViewModel GenerateStudentListViewModel()
    {
        using ApplicationDbContext db = new();
        List<StudentViewModel> students = db.Student.Select(
            e =>
                new StudentViewModel
                {
                    GivenName = e.GivenName,
                    LastName = e.LastName,
                    Email = e.Email
                }
        )
            .ToList();

        return new StudentListViewModel { Students = students };
    }

    public StudentListViewModel GenerateStudentListViewModelFromGroupName(
        string groupName,
        bool invert = false
    )
    {
        using ApplicationDbContext db = new();

        // validator makes sure this isn't null
        Group group = db.Group.FirstOrDefault(e => e.Name == groupName)!;
        HashSet<string> members = db.StudentGroup.Where(e => e.GroupId == group.Id)
            .Select(e => e.StudentId)
            .ToHashSet();

        Func<Student, bool> predicate = invert
            ? (e => !members.Contains(e.Id))
            : (e => members.Contains(e.Id));

        List<StudentViewModel> students = db.Student.Where(predicate)
            .Select(
                e =>
                    new StudentViewModel
                    {
                        GivenName = e.GivenName,
                        LastName = e.LastName,
                        Email = e.Email
                    }
            )
            .ToList();

        return new StudentListViewModel { Students = students };
    }
}

