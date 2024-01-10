using webapp.Models.ViewModels.Validators;

namespace webapp.Models.ViewModels;

public class StaffRoleModel
{
    [ExistingStaff]
    public required string Email { get; set; }

    [ExistingRole]
    public string? Role { get; set; }
}

public class StaffRolesModel
{
    public required List<StaffRoleModel> StaffRoles { get; set; }
}