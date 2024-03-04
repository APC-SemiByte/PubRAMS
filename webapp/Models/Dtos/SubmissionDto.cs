using System.ComponentModel.DataAnnotations;

using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class SubmissionDto
{
    /* GENERAL INFO */
    [StringLength(128)]
    public required string Title { get; set; }

    [StringLength(5000)]
    public required string Abstract { get; set; }

    /* CLASSIFICATION */
    [StringLength(256)]
    public required string Tags { get; set; }

    [ExistingCategory]
    public required string Category { get; set; }

    /* STATE INFO */
    public required bool Continued { get; set; }

    public string? Completion { get; set; }

    /* ASSOCIATIONS */
    [ExistingGroup]
    public required string Group { get; set; }

    [ExistingSchool]
    public required string School { get; set; }

    [ExistingSubject]
    public required string Subject { get; set; }

    [ExistingCourse]
    public required string Course { get; set; }

    [ExistingStaff(Roles = "Instructor")]
    public required string InstructorEmail { get; set; }

    [ExistingStaff]
    public required string AdviserEmail { get; set; }

    /* OTHERS */
    [StringLength(2500)]
    public string? Comment { get; set; }

    /* FILES */
    [ValidFile(Extensions = ".docx")]
    public IFormFile? File { get; set; }

    [ValidFile(Extensions = ".pdf")]
    public IFormFile? Prf { get; set; }

    [ValidFile(Extensions = ".pdf")]
    public IFormFile? Pdf { get; set; }

    public bool? SubmitFlag { get; set; }
}