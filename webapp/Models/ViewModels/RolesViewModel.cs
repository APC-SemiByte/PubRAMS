namespace webapp.Models.ViewModels;

public class RolesViewModel
{
    public required string Email { get; set; }

    public required List<string> Roles { get; set; }

    // for validation feedback (see `Edit` method in the controller and `_RolesPartial.cshtml`)
    public string? Info { get; set; }
}

public class RolesListViewModel
{
    public required List<RolesViewModel> StaffRoles { get; set; }
}

