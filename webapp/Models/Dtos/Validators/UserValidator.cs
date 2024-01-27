using System.ComponentModel.DataAnnotations;

using webapp.Models.EntityManagers;

namespace webapp.Models.Dtos.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ExistingUserAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string email = (string)value!;
        StudentManager studentManager = new();

        if (studentManager.GetByEmail(email) != null)
        {
            return true;
        }

        StaffManager staffManager = new();

        return staffManager.GetByEmail(email) != null;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} is not registered as a user.";
    }
}