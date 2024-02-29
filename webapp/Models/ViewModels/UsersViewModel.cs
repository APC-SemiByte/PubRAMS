namespace webapp.Models.ViewModels;

public class UserViewModel
{
    public required string GivenName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }
}

public class UsersViewModel
{
    public required List<UserViewModel> Users { get; set; }
}
