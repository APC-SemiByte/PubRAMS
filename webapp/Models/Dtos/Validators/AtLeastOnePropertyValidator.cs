using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace webapp.Models.Dtos.Validators;

// See https://stackoverflow.com/a/26424791
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AtLeastOnePropertyAttribute : ValidationAttribute
{
    private string[] PropertyList { get; set; }

    public AtLeastOnePropertyAttribute(params string[] propertyList)
    {
        this.PropertyList = propertyList;
    }

    //See http://stackoverflow.com/a/1365669
    public override object TypeId
    {
        get
        {
            return this;
        }
    }

    public override bool IsValid(object? value)
    {
        PropertyInfo propertyInfo;
        foreach (string propertyName in PropertyList)
        {
            propertyInfo = value!.GetType().GetProperty(propertyName)!;

            if (propertyInfo != null && propertyInfo.GetValue(value, null) != null)
            {
                return true;
            }
        }

        return false;
    }
}