using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class SchoolDto
{
    [ExistingSchool]
    public required string School { get; set; }
}