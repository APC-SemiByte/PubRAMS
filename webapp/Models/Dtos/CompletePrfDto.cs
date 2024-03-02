using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class CompletePrfDto
{
    [ExistingProject]
    public int ProjectId { get; set; }

    [ValidFile(Extensions = ".pdf")]
    public required IFormFile Prf { get; set; }
}