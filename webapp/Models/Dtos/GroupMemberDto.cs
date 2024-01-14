using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class GroupMemberDto
{
    [ExistingGroup]
    public required string GroupName { get; set; }

    [ExistingStudent]
    public required string StudentEmail { get; set; }
}