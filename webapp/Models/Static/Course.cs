using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Course
{
    public required int Id { get; set; }

    [MinLength(3), MaxLength(7)]
    public required string Code { get; set; }

    [MaxLength(128)]
    public required string Name { get; set; }
}