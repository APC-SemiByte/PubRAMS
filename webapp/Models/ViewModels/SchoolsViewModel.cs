namespace webapp.Models.ViewModels;

public class SchoolListViewModel
{
    public required List<string> Schools { get; set; }
}

public class SchoolRelatedOptionsViewModel
{
    public required List<string> Courses { get; set; }

    public required List<string> Subjects { get; set; }
}

