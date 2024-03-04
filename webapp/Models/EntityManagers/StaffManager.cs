using webapp.Data;
using webapp.Models.ViewModels;

namespace webapp.Models.EntityManagers;

/// <summary>
/// Handles database operations related to staff members. (Role management)
/// </summary>
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
        StaffRole defaultRole = new() { StaffId = user.Id, RoleId = (int)Roles.Unassigned };
        _ = db.StaffRole.Add(defaultRole);
        _ = db.SaveChanges();
    }

    public UsersViewModel GenerateUsersViewModel()
    {
        using ApplicationDbContext db = new();

        List<UserViewModel> list = db.Staff.Select(
            e =>
                new UserViewModel
                {
                    Id = e.Id,
                    GivenName = e.GivenName,
                    LastName = e.LastName,
                    Email = e.Email
                }
        )
            .ToList();

        return new() { Users = list };
    }

    public UsersViewModel GenerateUsersViewModel(int roleInt)
    {
        using ApplicationDbContext db = new();

        Func<Staff, UserViewModel> toViewModel =
            s =>
                new()
                {
                    Id = s.Id,
                    GivenName = s.GivenName,
                    LastName = s.LastName,
                    Email = s.Email
                };

        List<UserViewModel> list = (
            from staffRole in db.StaffRole
            join staff in db.Staff on staffRole.StaffId equals staff.Id
            where staffRole.RoleId == roleInt
            select toViewModel(staff)
        ).ToList();

        return new() { Users = list };
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

    public List<string> GetAvailableRoles(string staffId)
    {
        using ApplicationDbContext db = new();
        IQueryable<int> roleIds =
            from staffRole in db.StaffRole
            where staffRole.StaffId == staffId
            select staffRole.RoleId;

        return (
            from role in db.Role
            where (!roleIds.Any(id => id == role.Id)) && role.Id != (int)Roles.Unassigned
            select role.Name
       ).ToList();
    }

    public List<Role> GetRoles(IUser? user)
    {
        return user?.GetType() == typeof(Staff)
            ? GetRoles((Staff)user)
            : [];
    }

    public List<Role> GetRoles(Staff? user)
    {
        if (user == null)
        {
            return [];
        }

        using ApplicationDbContext db = new();
        IQueryable<int> roleIds =
            from staffRole in db.StaffRole
            where staffRole.StaffId == user.Id
            select staffRole.RoleId;

        return (
            from role in db.Role
            where roleIds.Any(id => id == role.Id)
            select role
       ).ToList();
    }

    public RolesListViewModel GenerateRolesListViewModel()
    {
        using ApplicationDbContext db = new();
        var staffInfo =
            from staffRole in db.StaffRole
            join staff in db.Staff on staffRole.StaffId equals staff.Id
            join role in db.Role on staffRole.RoleId equals role.Id
            select new
            {
                Id = staff.Id,
                GivenName = staff.GivenName,
                LastName = staff.LastName,
                Email = staff.Email,
                Role = role.Name
            };

        List<RolesViewModel> staffRoles = (
            from staff in db.Staff
            join role in staffInfo on staff.Id equals role.Id into roles
            select new RolesViewModel
            {
                User = new()
                {
                    Id = staff.Id,
                    GivenName = staff.GivenName,
                    LastName = staff.LastName,
                    Email = staff.Email,
                },
                Roles = roles.Select(e => e.Role).ToList()
            }
        ).ToList();

        return new() { StaffRoles = staffRoles };
    }

    public RolesViewModel GenerateRolesViewModelFromStaff(Staff user)
    {
        using ApplicationDbContext db = new();

        IQueryable<int> roleIds =
            from staffRole in db.StaffRole
            where staffRole.StaffId == user.Id
            select staffRole.RoleId;

        List<string> roles = (
            from role in db.Role
            where roleIds.Any(id => id == role.Id)
            select role.Name
        ).ToList();

        return new()
        {
            User = new()
            {
                Id = user.Id,
                GivenName = user.GivenName,
                LastName = user.LastName,
                Email = user.Email
            },
            Roles = roles,
        };
    }

    public void ToggleRoleByEmail(string email, string roleName)
    {
        using ApplicationDbContext db = new();

        // role validator should have made sure these exist
        Staff user = db.Staff.FirstOrDefault(e => e.Email == email)!;
        Role targetRole = db.Role.FirstOrDefault(e => e.Name == roleName)!;

        int count = db.StaffRole.Where(e => e.StaffId == user.Id).Count();

        StaffRole? unassigned = db.StaffRole.FirstOrDefault(
            e => e.StaffId == user.Id && e.RoleId == (int)Roles.Unassigned
        );

        StaffRole? existingStaffRole = db.StaffRole.FirstOrDefault(
            e => e.StaffId == user.Id && e.RoleId == targetRole.Id
        );

        if (existingStaffRole == null)
        {
            _ = db.StaffRole.Add(new() { StaffId = user.Id, RoleId = targetRole.Id });
            if (unassigned != null)
            {
                _ = db.StaffRole.Remove(unassigned);
            }
        }
        else
        {
            _ = db.StaffRole.Remove(existingStaffRole!);
            if (count == 1)
            {
                unassigned = new() { StaffId = user.Id, RoleId = (int)Roles.Unassigned };
                _ = db.StaffRole.Add(unassigned);
            }
        }

        _ = db.SaveChanges();
    }
}