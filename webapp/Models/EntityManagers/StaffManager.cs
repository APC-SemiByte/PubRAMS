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

    public List<Role> GetRolesById(string id)
    {
        using ApplicationDbContext db = new();
        List<StaffRole> lookups = [.. db.StaffRole.Where(e => e.StaffId == id)];
        List<Role> roles = [];

        foreach (StaffRole lookup in lookups)
        {
            Role role = db.Role.FirstOrDefault(e => e.Id == lookup.RoleId)!;
            roles.Add(role);
        }

        return roles;
    }

    public List<Role> GetRoles(Staff user)
    {
        using ApplicationDbContext db = new();
        List<StaffRole> lookups = [.. db.StaffRole.Where(e => e.StaffId == user.Id)];
        List<Role> roles = [];

        foreach (StaffRole lookup in lookups)
        {
            Role role = db.Role.FirstOrDefault(e => e.Id == lookup.RoleId)!;
            roles.Add(role);
        }

        return roles;
    }

    public List<Role> GetRolesByEmail(string email)
    {
        using ApplicationDbContext db = new();
        List<StaffRole> lookups = [.. db.StaffRole.Where(e => e.StaffId == email)];
        List<Role> roles = [];

        foreach (StaffRole lookup in lookups)
        {
            Role role = db.Role.FirstOrDefault(e => e.Id == lookup.RoleId)!;
            roles.Add(role);
        }

        return roles;
    }
}