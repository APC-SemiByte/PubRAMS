using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Student
{
    [MinLength(36), MaxLength(36)]
    public required string Id { get; set; }

    [Required]
    [MaxLength(64)]
    public required string FirstName { get; set; }

    [Required]
    [MaxLength(64)]
    public required string LastName { get; set; }

    [Required]
    [MaxLength(64)]
    public required string Email { get; set; }

    [MinLength(5), MaxLength(5)]
    public string? Block { get; set; }
}
