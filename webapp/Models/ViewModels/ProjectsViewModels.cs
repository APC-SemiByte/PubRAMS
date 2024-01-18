namespace webapp.Models.ViewModels;

public class ProjectViewModel
{
    public required string Title { get; set; }

    public required string Group { get; set; }

    public required string DocumentUrl { get; set; }

    public required string Abstract { get; set; }

    public required string State { get; set; }

    public required string School { get; set; }

    public required string Subject { get; set; }

    public required string Course { get; set; }

    /* [MinLength(36), MaxLength(36)] */
    /* public string? InstructorId { get; set; } */
    /* public virtual Staff? Instructor { get; set; } */
    /**/
    /* [MinLength(36), MaxLength(36)] */
    /* public string? ExecDirId { get; set; } */
    /* public virtual Staff? ExecDir { get; set; } */
    /**/
    /* [MinLength(36), MaxLength(36)] */
    /* public string? AdviserId { get; set; } */
    /* public virtual Staff? Adviser { get; set; } */
    /**/
    /* [MinLength(36), MaxLength(36)] */
    /* public string? ProofreaderId { get; set; } */
    /* public virtual Staff? Proofreader { get; set; } */
    /**/
    /* [MaxLength(5000)] */
    /* public string? PrfUrl { get; set; } */
}

public class ProjectListViewModel
{
    public required List<ProjectViewModel> Projects { get; set; }
}