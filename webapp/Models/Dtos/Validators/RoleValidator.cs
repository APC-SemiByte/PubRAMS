using System.ComponentModel.DataAnnotations;
using webapp.Data;

namespace webapp.Models.Dtos.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ExistingRoleAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string name = (string)value!;
        if (name == "Unassigned")
        {
            return false;
        }

        using ApplicationDbContext db = new();
        return db.Role.FirstOrDefault(e => e.Name == name) != null;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} was not found in the DB.";
    }
}