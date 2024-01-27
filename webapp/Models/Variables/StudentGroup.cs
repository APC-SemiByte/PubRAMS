using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace webapp.Models;

[PrimaryKey(nameof(StudentId), nameof(GroupId))]
public class StudentGroup
{
    [MinLength(36), MaxLength(36)]
    public required string StudentId { get; set; }
    public virtual Student? Student { get; set; }

    public int GroupId { get; set; }
    public virtual Group? Group { get; set; }
}