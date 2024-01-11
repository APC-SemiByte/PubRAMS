using webapp.Data;
using webapp.Models.ViewModels;

namespace webapp.Models.EntityManagers;

public class RoleManager
{
    public StaffRolesView GetAllUserRoles()
    {
        using ApplicationDbContext db = new();
        List<StaffRole> staffRoles = [.. db.StaffRole];

        StaffRolesView model = new() { StaffRoles = [] };
        HashSet<string> emails = [];
        foreach (StaffRole staffRole in staffRoles)
        {
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

    public void AssignDefaultRole(Staff user)
    {
        using ApplicationDbContext db = new();
        StaffRole staffRole = new() { RoleId = 1, StaffId = user.Id };

        _ = db.StaffRole.Add(staffRole);
        _ = db.SaveChanges();
    }
}

