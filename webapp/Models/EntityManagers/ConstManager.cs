using webapp.Data;
using webapp.Models.ViewModels;

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
        return (
            from course in db.Course
            where course.Id != (int)Roles.Unassigned
            select course.Code
       ).ToList();
    }

    public bool RoleExists(string name)
    {
        using ApplicationDbContext db = new();
        return db.Role.FirstOrDefault(e => e.Name == name) != null;
    }

    public List<string> GetSchools()
    {
        using ApplicationDbContext db = new();
        return db.School.Select(e => e.Name).ToList();
    }

    public SchoolRelatedOptionsViewModel GenerateSchoolRelatedOptionsViewModel(string schoolName)
    {
        using ApplicationDbContext db = new();
        // validator guarantees this exists
        School school = db.School.FirstOrDefault(e => e.Name == schoolName)!;

        List<string> courses = (
            from course in db.Course
            where course.SchoolId == school.Id
            select course.Code
        ).ToList();

        List<string> subjects = (
            from subject in db.Subject
            where subject.SchoolId == school.Id
            select subject.Code
        ).ToList();

        return new() { Courses = courses, Subjects = subjects };
    }

    public bool SchoolExists(string name)
    {
        using ApplicationDbContext db = new();
        return db.School.FirstOrDefault(e => e.Name == name) != null;
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