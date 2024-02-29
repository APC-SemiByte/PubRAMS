namespace webapp.Models.ViewModels;

public class GroupViewModel
{
    // keep this in an invisible element,
    // needed for identifying groups if there are group name collisions
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required UserViewModel Leader { get; set; }

    public required List<UserViewModel> Members { get; set; }
}

public class GroupListViewModel
{
    public required List<GroupViewModel> Groups { get; set; }
}