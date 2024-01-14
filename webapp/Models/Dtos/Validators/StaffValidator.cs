using System.ComponentModel.DataAnnotations;
using webapp.Models.EntityManagers;

namespace webapp.Models.Dtos.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ExistingStaffAttribute : ValidationAttribute
{
    public string? Roles { get; set; }

    public override bool IsValid(object? value)
    {
        string email = (string)value!;
        StaffManager staffManager = new();

        Staff? staff = staffManager.GetByEmail(email);

        if (staff == null)
        {
            return false;
        }

        if (Roles == null)
        {
            return true;
        }

        List<string> requiredRoles = [.. Roles.Split(",")];
        List<Role> roles = staffManager.GetRoles(staff);

        return roles.Select(e => e.Name).Intersect(requiredRoles).Any();
    }

    public override string FormatErrorMessage(string name)
    {
        return Roles == null
            ? $"{name} is not registered as a staff member."
            : $"{name} is not registered as a staff member with role {Roles}";
    }
}