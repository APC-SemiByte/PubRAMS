using System.ComponentModel.DataAnnotations;
using webapp.Data;

namespace webapp.Models.Dtos.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ExistingProjectAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        int id = (int)value!;

        using ApplicationDbContext db = new();
        return db.Project.FirstOrDefault(e => e.Id == id) != null;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} was not found in the DB.";
    }
}
