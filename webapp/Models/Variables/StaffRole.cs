using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace webapp.Models;

[PrimaryKey(nameof(StaffId), nameof(RoleId))]
public class StaffRole
{
    [MinLength(36), MaxLength(36)]
    public required string StaffId { get; set; }
    public virtual Staff? Staff { get; set; }

    public required int RoleId { get; set; }
    public virtual Role? Role { get; set; }
}

