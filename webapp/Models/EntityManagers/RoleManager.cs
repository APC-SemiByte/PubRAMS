using webapp.Data;
using webapp.Models.ViewModels;

namespace webapp.Models.EntityManagers;

public class RoleManager
{
    public StaffRolesModel GetAllUserRoles()
    {
        using ApplicationDbContext db = new();
        List<StaffRole> staffRoles = [.. db.StaffRole];

        StaffRolesModel model = new() { StaffRoles = [] };
        foreach (StaffRole staffRole in staffRoles)
        {
            Staff staff = db.Staff.FirstOrDefault(e => e.Id == staffRole.StaffId)!;
            Role role = db.Role.FirstOrDefault(e => e.Id == staffRole.RoleId)!;
            model.StaffRoles.Add(new() { Email = staff.Email, Role = role.Name });
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

