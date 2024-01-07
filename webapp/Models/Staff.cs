using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Staff
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
}
