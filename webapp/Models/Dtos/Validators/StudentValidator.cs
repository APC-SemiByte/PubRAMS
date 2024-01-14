using System.ComponentModel.DataAnnotations;
using webapp.Models.EntityManagers;

namespace webapp.Models.Dtos.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ExistingStudentAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string email = (string)value!;
        StudentManager studentManager = new();
        return studentManager.GetByEmail(email) != null;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} is not registered as a student.";
    }
}

