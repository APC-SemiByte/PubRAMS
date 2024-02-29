using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Project
{
    public int Id { get; set; }

    [MaxLength(128)]
    public required string Title { get; set; }

    public int GroupId { get; set; }
    public virtual Group? Group { get; set; }

    [MaxLength(5000)]
    public required string Abstract { get; set; }

    public required int StateId { get; set; }
    public virtual State? State { get; set; }

    public required int SchoolId { get; set; }
    public virtual School? School { get; set; }

    public required int SubjectId { get; set; }
    public virtual Subject? Subject { get; set; }

    public int CourseId { get; set; }
    public virtual Course? Course { get; set; }

    [MinLength(36), MaxLength(36)]
    public string? InstructorId { get; set; }
    public virtual Staff? Instructor { get; set; }

    [MinLength(36), MaxLength(36)]
    public string? AdviserId { get; set; }
    public virtual Staff? Adviser { get; set; }

    [MinLength(36), MaxLength(36)]
    public string? ProofreaderId { get; set; }
    public virtual Staff? Proofreader { get; set; }

    [MaxLength(250)]
    public string? DocumentHandle { get; set; }

    [MaxLength(250)]
    public string? PrfHandle { get; set; }

    [MaxLength(5000)]
    public string? Comment { get; set; }

    // we can figure out how to use proper types later, stick with JS date string
    [MinLength(12), MaxLength(12)]
    public string? DeadlineDate { get; set; }

    [MinLength(12), MaxLength(12)]
    public string? PublishDate { get; set; }
}