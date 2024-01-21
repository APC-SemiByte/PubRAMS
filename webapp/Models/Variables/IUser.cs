using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public interface IUser
{
    [MinLength(36), MaxLength(36)]
    public string Id { get; set; }

    [MaxLength(64)]
    public string GivenName { get; set; }

    [MaxLength(64)]
    public string LastName { get; set; }

    [MaxLength(64)]
    public string Email { get; set; }
}
