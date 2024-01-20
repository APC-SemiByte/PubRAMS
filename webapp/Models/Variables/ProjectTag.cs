using Microsoft.EntityFrameworkCore;

namespace webapp.Models;

[PrimaryKey(nameof(ProjectId), nameof(TagId))]
public class ProjectTag
{
    public int ProjectId { get; set; }
    public virtual Project? Project { get; set; }

    public int TagId { get; set; }
    public virtual Tag? Tag { get; set; }
}