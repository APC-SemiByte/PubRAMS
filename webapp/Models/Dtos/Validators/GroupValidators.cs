using System.ComponentModel.DataAnnotations;
using webapp.Data;

namespace webapp.Models.Dtos.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ExistingGroupAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string name = (string)value!;

        using ApplicationDbContext db = new();
        return db.Group.FirstOrDefault(e => e.Name == name) != null;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} was not found in the DB.";
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class UniqueGroupAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string name = (string)value!;

        using ApplicationDbContext db = new();
        return db.Group.FirstOrDefault(e => e.Name == name) == null;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} was found in the DB.";
    }
}