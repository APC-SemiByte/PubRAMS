using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class School
{
    public required int Id { get; set; }

    [MaxLength(5)]
    public required string Code { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    [MinLength(36), MaxLength(36)]
    public required string ExecDirId { get; set; }
    public virtual Staff? ExecDir { get; set; }
}

