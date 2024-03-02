using System.ComponentModel.DataAnnotations;
using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

[AtLeastOneProperty("Comment", "File")]
public class RejectDto
{
    [StringLength(2500)]
    public string? Comment { get; set; }

    [ValidFile(Extensions = ".docx")]
    public IFormFile? File { get; set; }
}