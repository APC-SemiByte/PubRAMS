using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace webapp.Models;

[PrimaryKey(nameof(ProjectId), nameof(TagId))]
public class LookupTag
{
    [Required]
    public int ProjectId { get; set; }
    public virtual required ProjectInfo Project { get; set; }

    public int TagId { get; set; }
    public virtual required Tag Tag { get; set; }
}


