using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace webapp.Models;

[Index(nameof(Name))]
public class Role
{
    public required int Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(128)]
    public required string Desc { get; set; }
}

public enum Roles
{
    Unassigned = 1,
    Admin,
    Instructor,
    ExecutiveDirector,
    EcHead,
    EcFaculty,
    Librarian,
    PblCoordinator,
    ProgramDirector
}