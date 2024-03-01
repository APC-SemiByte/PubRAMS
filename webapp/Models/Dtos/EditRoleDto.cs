using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class EditRoleDto
{
    [ExistingStaff]
    public required string Email { get; set; }

    [ExistingRole]
    public required string Role { get; set; }
}