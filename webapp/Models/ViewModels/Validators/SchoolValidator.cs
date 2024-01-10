using System.ComponentModel.DataAnnotations;
using webapp.Data;

namespace webapp.Models.ViewModels.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ExistingSchoolAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string code = (string)value!;

        using ApplicationDbContext db = new();
        return db.School.FirstOrDefault(e => e.Code == code) != null;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} was not found in the DB.";
    }
}