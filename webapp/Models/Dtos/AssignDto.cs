using System.ComponentModel.DataAnnotations;
using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class AssignDto
{
    [ExistingProject]
    public int ProjectId { get; set; }

    [ExistingStaff(Roles = "English Office Faculty")]
    public required string ProofreaderEmail { get; set; }

    [RegularExpression(@"\d{4}-\d{2}-\d{2}")]
    public required string Deadline { get; set; }
}