using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace webapp.Models;

[Index(nameof(Email))]
public class Staff : IUser
{
    [MinLength(36), MaxLength(36)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string GivenName { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string Email { get; set; } = string.Empty;
}
