using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace webapp.Models;

[PrimaryKey(nameof(StaffId), nameof(RoleId))]
public class LookupRole
{
    [MinLength(36), MaxLength(36)]
    public required string StaffId { get; set; }
    public virtual required Staff Staff { get; set; }

    public int RoleId { get; set; }
    public virtual required Role Role { get; set; }
}


