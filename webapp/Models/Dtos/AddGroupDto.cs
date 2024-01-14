using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class AddGroupDto
{
    [UniqueGroup]
    public required string Name { get; set; }

    [ExistingStudent]
    public required string LeaderEmail { get; set; }
}

