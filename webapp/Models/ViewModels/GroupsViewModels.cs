namespace webapp.Models.ViewModels;

public class StudentViewModel
{
    public required string GivenName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }
}

public class StudentListViewModel
{
    public required List<StudentViewModel> Students { get; set; }
}

public class GroupViewModel
{
    // keep this in an invisible element,
    // needed for identifying groups if there are group name collisions
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required StudentViewModel Leader { get; set; }

    public required List<StudentViewModel> Members { get; set; }
}

public class GroupListViewModel
{
    public required List<GroupViewModel> Groups { get; set; }
}
