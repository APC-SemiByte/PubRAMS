using webapp.Models.Dtos.Validators;

namespace webapp.Models.Dtos;

public class CompletePrfDto
{
    [ValidFile(Extensions = ".pdf")]
    public required IFormFile Prf { get; set; }
}