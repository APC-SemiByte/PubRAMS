using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Tag
{
    public int Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }
}
