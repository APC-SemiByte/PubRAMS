namespace webapp.Models.ViewModels;

public class RolesViewModel
{
    public required string Email { get; set; }

    public required List<string> Roles { get; set; }

    public string? Info { get; set; }
}

public class RolesListViewModel
{
    public required List<RolesViewModel> StaffRoles { get; set; }
}
