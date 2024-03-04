using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace webapp.Models;

[Index(nameof(Name))]
public class Category
{
    public required int Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }
}

public enum Categories
{
    Hospitality = 1,
    FoodService,
    Retail,
    Medical,
    Education,
    Ecommerce,
    Agrigulture,
    Government,
    HumanResource,
    Marketing,
    Manufacturing,
    Others
}