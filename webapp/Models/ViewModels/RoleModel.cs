using webapp.Models.ViewModels.Validators;

namespace webapp.Models.ViewModels;

public class StaffRoleView
{
    [ExistingStaff]
    public required string Email { get; set; }

    [ExistingRole]
    public required List<string> Roles { get; set; }
}

public class StaffRolesView
{
    public required List<StaffRoleView> StaffRoles { get; set; }
}