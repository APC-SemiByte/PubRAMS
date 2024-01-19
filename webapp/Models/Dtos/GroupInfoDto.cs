using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class GroupInfoDto
{
    [ExistingGroup]
    public required string GroupName { get; set; }
}