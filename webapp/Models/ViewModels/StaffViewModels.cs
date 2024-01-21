namespace webapp.Models.ViewModels;

public class StaffViewModel
{
    public required string GivenName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }
}

public class StaffListViewModel
{
    public required List<StaffViewModel> Staff { get; set; }
}

