using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public interface IUser
{
    [MinLength(36), MaxLength(36)]
    public string Id { get; set; }

    [Required]
    [MaxLength(64)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(64)]
    public string LastName { get; set; }

    [Required]
    [MaxLength(64)]
    public string Email { get; set; }
}
