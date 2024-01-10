using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Role
{
    public required int Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(128)]
    public required string Desc { get; set; }
}

