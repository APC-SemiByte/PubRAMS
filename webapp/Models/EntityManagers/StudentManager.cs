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

        return new() { Students = students };
    }

    public StudentListViewModel GenerateStudentListViewModelFromGroupName(
        string groupName,
        bool invert = false
    )
    {
        using ApplicationDbContext db = new();

        // validator makes sure this isn't null
        Group group_ = db.Group.FirstOrDefault(e => e.Name == groupName)!;
        IQueryable<string> members =
            from studentGroup in db.StudentGroup
            where studentGroup.GroupId == group_.Id
            select studentGroup.StudentId;

        List<StudentViewModel> students = invert
            ?
            (
                 from student in db.Student
                 where !members.Any(id => id == student.Id)
                 select new StudentViewModel
                 {
                     GivenName = student.GivenName,
                     LastName = student.LastName,
                     Email = student.Email
                 }
            ).ToList()
            :
            (
                 from student in db.Student
                 where members.Any(id => id == student.Id)
                 select new StudentViewModel
                 {
                     GivenName = student.GivenName,
                     LastName = student.LastName,
                     Email = student.Email
                 }
            ).ToList();

        return new() { Students = students };
    }
}