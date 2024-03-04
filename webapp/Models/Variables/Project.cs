using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Project
{
    /* GENERAL INFO */
    public int Id { get; set; }

    [MaxLength(128)]
    public required string Title { get; set; }

    [MaxLength(5000)]
    public required string Abstract { get; set; }

    /* CLASSIFICATION */
    [MaxLength(256)]
    public required string Tags { get; set; }

    public required int CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    /* STATE INFO */
    public required bool Continued { get; set; }

    public required bool Archived { get; set; }

    /// <summary>
    /// State of the documentation
    /// </summary>
    public required int StateId { get; set; }
    public virtual State? State { get; set; }

    /// <summary>
    /// State of the software
    /// </summary>
    public required int CompletionId { get; set; }
    public virtual Completion? Completion { get; set; }

    /* ASSOCIATIONS */
    public int GroupId { get; set; }
    public virtual Group? Group { get; set; }

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

    /* OTHERS */
    [MaxLength(2500)]
    public string? StudentComment { get; set; }

    [MaxLength(2500)]
    public string? StaffComment { get; set; }

    /* FILES */
    [MaxLength(256)]
    public required string BaseHandle { get; set; }

    public required bool HasPrf { get; set; }

    public required bool HasPdf { get; set; }

    public required bool Edited { get; set; }

    public int? KohaRecordId { get; set; }

    /* DATES */
    public DateTime? DeadlineDate { get; set; }

    public DateTime? PublishDate { get; set; }

    [MinLength(1), MaxLength(1)]
    public required string Term { get; set; }
}