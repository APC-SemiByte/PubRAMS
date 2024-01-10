using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace webapp.Models;

[PrimaryKey(nameof(StudentId), nameof(GroupId))]
public class LookupGroup
{
    [MinLength(36), MaxLength(36)]
    public required string StudentId { get; set; }
    public virtual required Student Student { get; set; }

    public int GroupId { get; set; }
    public virtual required Group Group { get; set; }
}


