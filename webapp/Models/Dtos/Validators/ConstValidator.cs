using System.ComponentModel.DataAnnotations;
using webapp.Models.EntityManagers;

namespace webapp.Models.Dtos.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ExistingCourseAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string code = (string)value!;

        ConstManager manager = new();
        return manager.CourseExists(code);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} was not found in the DB.";
    }
}

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

        ConstManager manager = new();
        return manager.RoleExists(name);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} was not found in the DB.";
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ExistingSchoolAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string code = (string)value!;

        ConstManager manager = new();
        return manager.SchoolExists(code);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} was not found in the DB.";
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ExistingSubjectAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string code = (string)value!;

        ConstManager manager = new();
        return manager.SubjectExists(code);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} was not found in the DB.";
    }
}