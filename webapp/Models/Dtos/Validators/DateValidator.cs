using System.ComponentModel.DataAnnotations;

namespace webapp.Models.Dtos.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ValidDeadlineAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        DateTime date = (DateTime)value!;

        return date > DateTime.Now.Date.AddDays(1);
    }

    public override string FormatErrorMessage(string name)
    {
        return "Deadline must be in at least 1 day.";
    }
}
