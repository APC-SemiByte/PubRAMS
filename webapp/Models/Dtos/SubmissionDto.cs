using System.ComponentModel.DataAnnotations;

using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class SubmissionDto
{
    [StringLength(128)]
    public required string Title { get; set; }

    [ExistingGroup]
    public required string Group { get; set; }

    [StringLength(5000)]
    public required string Abstract { get; set; }

    [ExistingSchool]
    public required string School { get; set; }

    [ExistingSubject]
    public required string Subject { get; set; }

    [ExistingCourse]
    public required string Course { get; set; }

    [ExistingStaff]
    public required string AdviserEmail { get; set; }

    [ExistingStaff(Roles = "Instructor")]
    public required string InstructorEmail { get; set; }

    [StringLength(2500)]
    public string? Comment { get; set; }

    [ValidFile(Extensions = ".docx")]
    public IFormFile? File { get; set; }

    [ValidFile(Extensions = ".pdf")]
    public IFormFile? Prf { get; set; }

    [ValidFile(Extensions = ".pdf")]
    public IFormFile? Pdf { get; set; }

    public bool? SubmitFlag { get; set; }
}