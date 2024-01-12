using webapp.Data;
using webapp.Models.ViewModels;

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

    public bool RoleExists(string name)
    {
        using ApplicationDbContext db = new();
        return db.Role.FirstOrDefault(e => e.Name == name) != null;
    }

    public List<Role> GetRoles(Staff user)
    {
        using ApplicationDbContext db = new();
        List<StaffRole> lookups = [.. db.StaffRole.Where(e => e.StaffId == user.Id)];
        List<Role> roles = [];

        foreach (StaffRole lookup in lookups)
        {
            // ignore null bc if it's in the db, it conforms to the foreign key contraint
            Role role = db.Role.FirstOrDefault(e => e.Id == lookup.RoleId)!;
            roles.Add(role);
        }

        return roles;
    }

    public RolesListViewModel GenerateRolesListViewModel()
    {
        using ApplicationDbContext db = new();
        List<StaffRole> staffRoles = [.. db.StaffRole];

        RolesListViewModel model = new() { StaffRoles = [] };
        HashSet<string> emails = [];
        foreach (StaffRole staffRole in staffRoles)
        {
            // ignore null bc if it's in the db, it conforms to the foreign key contraint
            Staff staff = db.Staff.FirstOrDefault(e => e.Id == staffRole.StaffId)!;
            Role role = db.Role.FirstOrDefault(e => e.Id == staffRole.RoleId)!;
            if (emails.Add(staff.Email))
            {
                model.StaffRoles.Add(new() { Email = staff.Email, Roles = [role.Name] });
                continue;
            }

            model.StaffRoles.FirstOrDefault(e => e.Email == staff.Email)?.Roles.Add(role.Name);
        }

        return model;
    }

    public RolesListViewModel? GenerateRolesListViewModel(Func<StaffRole, bool> predicate)
    {
        using ApplicationDbContext db = new();
        List<StaffRole> staffRoles = [.. db.StaffRole.Where(predicate)];

        RolesListViewModel model = new() { StaffRoles = [] };
        HashSet<string> emails = [];
        foreach (StaffRole staffRole in staffRoles)
        {
            // ignore null bc if it's in the db, it conforms to the foreign key contraint
            Staff staff = db.Staff.FirstOrDefault(e => e.Id == staffRole.StaffId)!;
            Role role = db.Role.FirstOrDefault(e => e.Id == staffRole.RoleId)!;
            if (emails.Add(staff.Email))
            {
                model.StaffRoles.Add(new() { Email = staff.Email, Roles = [role.Name] });
                continue;
            }

            model.StaffRoles.FirstOrDefault(e => e.Email == staff.Email)?.Roles.Add(role.Name);
        }

        return model;
    }

    public RolesViewModel GenerateRolesViewModel(Staff user)
    {
        using ApplicationDbContext db = new();
        List<StaffRole> staffRoles = [.. db.StaffRole.Where(e => e.StaffId == user.Id)];

        RolesViewModel model = new() { Email = user.Email, Roles = [] };
        foreach (StaffRole staffRole in staffRoles)
        {
            // ignore null bc if it's in the db, it conforms to the foreign key contraint
            Role role = db.Role.FirstOrDefault(e => e.Id == staffRole.RoleId)!;
            model.Roles.Add(role.Name);
        }

        return model;
    }

    public bool ToggleRoleByEmail(string email, string roleName)
    {
        using ApplicationDbContext db = new();
        Staff? user = db.Staff.FirstOrDefault(e => e.Email == email);
        if (user == null)
        {
            return false;
        }

        Role? targetRole = db.Role.FirstOrDefault(e => e.Name == roleName);
        if (targetRole == null)
        {
            return false;
        }

        StaffRole? existingStaffRole = db.StaffRole.FirstOrDefault(
            e => e.StaffId == user.Id && e.RoleId == targetRole.Id
        );

        _ =
            existingStaffRole == null
                ? db.StaffRole.Add(new StaffRole { StaffId = user.Id, RoleId = targetRole.Id })
                : db.StaffRole.Remove(existingStaffRole!);

        _ = db.SaveChanges();
        return true;
    }

    public void AssignDefaultRole(Staff user)
    {
        using ApplicationDbContext db = new();
        StaffRole staffRole = new() { RoleId = 1, StaffId = user.Id };

        _ = db.StaffRole.Add(staffRole);
        _ = db.SaveChanges();
    }
}
