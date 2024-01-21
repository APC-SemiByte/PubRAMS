using webapp.Data;

namespace webapp.Models.EntityManagers;

public class ConstManager
{
    public List<string> GetCourses()
    {
        using ApplicationDbContext db = new();
        return db.Course.Select(e => e.Code).ToList();
    }

    public bool CourseExists(string code)
    {
        using ApplicationDbContext db = new();
        return db.Course.FirstOrDefault(e => e.Code == code) != null;
    }

    public List<string> GetRoles()
    {
        using ApplicationDbContext db = new();
        return db.Course.Where(e => e.Id != 1).Select(e => e.Code).ToList();
    }

    public bool RoleExists(string name)
    {
        using ApplicationDbContext db = new();
        return db.Role.FirstOrDefault(e => e.Name == name) != null;
    }

    public List<string> GetSchools()
    {
        using ApplicationDbContext db = new();
        return db.School.Select(e => e.Code).ToList();
    }

    public bool SchoolExists(string code)
    {
        using ApplicationDbContext db = new();
        return db.School.FirstOrDefault(e => e.Code == code) != null;
    }

    public bool StateExists(string name)
    {
        using ApplicationDbContext db = new();
        return db.State.FirstOrDefault(e => e.Name == name) != null;
    }

    public List<string> GetSubjects()
    {
        using ApplicationDbContext db = new();
        return db.Subject.Select(e => e.Code).ToList();
    }

    public bool SubjectExists(string code)
    {
        using ApplicationDbContext db = new();
        return db.Subject.FirstOrDefault(e => e.Code == code) != null;
    }
}