using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Role
{
    public int Id { get; set; }

    [Required]
    [MaxLength(64)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(128)]
    public required string Desc { get; set; }
}


