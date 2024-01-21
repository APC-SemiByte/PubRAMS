using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Subject
{
    public required int Id { get; set; }

    [MinLength(6), MaxLength(7)]
    public required string Code { get; set; }

    [MaxLength(128)]
    public required string Name { get; set; }

    public required int SchoolId { get; set; }
    public virtual School? School { get; set; }
}

