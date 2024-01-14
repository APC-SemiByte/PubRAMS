using System.ComponentModel.DataAnnotations;
using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class SubmissionDto
{
    [StringLength(128, MinimumLength = 1)]
    public required string Title { get; set; }

    [StringLength(5000, MinimumLength = 1)]
    public required string Abstract { get; set; }

    [ExistingStaff(Roles = "Instructor")]
    public required string InstructorEmail { get; set; }
}

