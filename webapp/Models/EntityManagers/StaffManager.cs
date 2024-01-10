using webapp.Data;

namespace webapp.Models.EntityManagers;

public class StaffManager : IUserManager<Staff>
{
    public bool DbContains(string id)
    {
        using ApplicationDbContext db = new();
        Staff? user = db.Staff.FirstOrDefault(e => e.Id == id);
        return user != null;
    }

    public void Add(Staff user)
    {
        using ApplicationDbContext db = new();
        _ = db.Staff.Add(user);
        _ = db.SaveChanges();
    }

    public Staff? GetById(string id)
    {
        using ApplicationDbContext db = new();
        return db.Staff.FirstOrDefault(e => e.Id == id);
    }

    public Staff? GetByEmail(string email)
    {
        using ApplicationDbContext db = new();
        return db.Staff.FirstOrDefault(e => e.Email == email);
    }

    public Role? GetRoleById(string id)
    {
        using ApplicationDbContext db = new();
        LookupRole? lookup = db.LookupRole.FirstOrDefault(e => e.StaffId == id);
        return lookup == null ? null : db.Role.FirstOrDefault(e => e.Id == lookup.RoleId);
    }

    public Role? GetRoleByEmail(string email)
    {
        using ApplicationDbContext db = new();
        string? id = db.Staff.FirstOrDefault(e => e.Email == email)?.Id;
        LookupRole? lookup = db.LookupRole.FirstOrDefault(e => e.StaffId == id);
        return lookup == null ? null : db.Role.FirstOrDefault(e => e.Id == lookup.RoleId);
    }
}


