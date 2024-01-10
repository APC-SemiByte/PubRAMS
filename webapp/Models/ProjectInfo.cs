using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class ProjectInfo
{
    public int Id { get; set; }

    [Required]
    public int GroupId { get; set; }
    public virtual required Group Group { get; set; }

    [Required]
    [MaxLength(5000)]
    public required string DocumentUrl { get; set; }

    [Required]
    [MaxLength(5000)]
    public required string Abstract { get; set; }

    [Required]
    public int StateId { get; set; }
    public virtual required ProjectState State { get; set; }

    [MinLength(36), MaxLength(36)]
    public string? InstructorId { get; set; }
    public virtual Staff? Instructor { get; set; }

    [MinLength(36), MaxLength(36)]
    public string? ExecDirId { get; set; }
    public virtual Staff? ExecDir { get; set; }

    [MinLength(36), MaxLength(36)]
    public string? AdviserId { get; set; }
    public virtual Staff? Adviser { get; set; }

    [MinLength(36), MaxLength(36)]
    public string? ProofreaderId { get; set; }
    public virtual Staff? Proofreader { get; set; }

    [MaxLength(5000)]
    public string? PrfUrl { get; set; }
}


