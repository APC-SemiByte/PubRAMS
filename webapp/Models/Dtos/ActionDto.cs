using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class ActionDto
{
    [ExistingProject]
    public int ProjectId { get; set; }
}