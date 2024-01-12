using webapp.Models.ViewModels.Validators;

namespace webapp.Models.ViewModels;

public class EditRoleViewModel
{
    [ExistingStaff]
    public required string Email { get; set; }

    [ExistingRole]
    public required string Role { get; set; }
}
