using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class AssignDto
{
    [ExistingStaff(Roles = "English Office Faculty")]
    public required string ProofreaderEmail { get; set; }

    [ValidDeadline]
    public required DateTime Deadline { get; set; }
}