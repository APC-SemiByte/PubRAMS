using System.ComponentModel.DataAnnotations;

namespace webapp.Models.Dtos.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ValidFileAttribute : ValidationAttribute
{
    private readonly Dictionary<string, List<byte[]>> validSignaturesPerType =
        new()
        {
            {
                ".docx",
                new() { new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 } }
            },
            {
                ".pdf",
                new() { new byte[] { 0x25, 0x50, 0x44, 0x46 } }
            }
        };

    public required string Extensions { get; set; }

    public override bool IsValid(object? value)
    {
        IFormFile file = (IFormFile)value!;

        string[] validExtensions = Extensions.Split(",");
        string ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !validExtensions.Contains(ext))
        {
            return false;
        }

        using BinaryReader reader = new(file.OpenReadStream());
        List<byte[]> validSignatures = validSignaturesPerType[ext];
        byte[] headerBytes = reader.ReadBytes(validSignatures.Max(m => m.Length));

        return validSignatures.Any(
            signature => headerBytes.Take(signature.Length).SequenceEqual(signature)
        );
    }

    public override string FormatErrorMessage(string name)
    {
        return $"Only {Extensions} documents may be uploaded";
    }
}