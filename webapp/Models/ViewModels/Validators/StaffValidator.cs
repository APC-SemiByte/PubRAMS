using System.ComponentModel.DataAnnotations;
using webapp.Models.EntityManagers;

namespace webapp.Models.ViewModels.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ExistingStaffAttribute : ValidationAttribute
{
    public string? Role { get; set; }

    public override bool IsValid(object? value)
    {
        string email = (string)value!;
        StaffManager staffManager = new();

        Models.Staff? staff = staffManager.GetByEmail(email);
        return staff != null && (Role == null || staffManager.GetRoleById(staff.Id)?.Name == Role);
    }

    public override string FormatErrorMessage(string name)
    {
        return Role == null
            ? $"{name} is not registered as a staff member."
            : $"{name} is not registered as a staff member with role {Role}";
    }
}