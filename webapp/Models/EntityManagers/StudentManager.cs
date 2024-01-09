using webapp.Data;

namespace webapp.Models.EntityManagers;

public class StudentManager : IUserManager<Student>
{
    public bool DbContains(string id)
    {
        using ApplicationDbContext db = new();
        var student = db.Student.FirstOrDefault(e => e.Id.Equals(id));
        return student != null;
    }

    public void Add(Student user)
    {
        using ApplicationDbContext db = new();
        _ = db.Student.Add(user);
        db.SaveChanges();
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
}
