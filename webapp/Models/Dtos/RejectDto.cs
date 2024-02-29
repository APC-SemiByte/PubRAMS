using System.ComponentModel.DataAnnotations;
using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

[AtLeastOneProperty("Comment", "File")]
public class RejectDto
{
    [StringLength(5000)]
    public string? Comment { get; set; }

    [ValidFile(Extensions = ".docx", Nullable = true)]
    public IFormFile? File { get; set; }
}