using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class FileActionDto
{
    [ExistingProject]
    public int ProjectId { get; set; }

    [ValidFile(Extensions = ".docx,.pdf")]
    public required IFormFile File { get; set; }
}