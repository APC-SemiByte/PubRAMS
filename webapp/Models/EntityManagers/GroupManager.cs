using webapp.Data;
using webapp.Models.ViewModels;

namespace webapp.Models.EntityManagers;

public class GroupManager
{
    public Group AddWithLeaderEmail(string name, string email)
    {
        using ApplicationDbContext db = new();
        // validator would've rejected no match
        Student leader = db.Student.FirstOrDefault(e => e.Email == email)!;
        Group group = new() { Name = name, LeaderId = leader.Id };

        _ = db.Group.Add(group);
        _ = db.SaveChanges();

        int groupId = db.Group.FirstOrDefault(e => e.Name == group.Name)!.Id;
        StudentGroup newStudentGroup = new() { StudentId = group.LeaderId!, GroupId = groupId };
        _ = db.StudentGroup.Add(newStudentGroup);
        _ = db.SaveChanges();

        return group;
    }

    public Group AddMemberByEmail(string name, string email)
    {
        using ApplicationDbContext db = new();
        // validator would've rejected no match
        Student member = db.Student.FirstOrDefault(e => e.Email == email)!;
        Group group = db.Group.FirstOrDefault(e => e.Name == name)!;

        StudentGroup newStudentGroup = new() { StudentId = member.Id, GroupId = group.Id };
        _ = db.StudentGroup.Add(newStudentGroup);
        _ = db.SaveChanges();

        return group;
    }

    public Group RemoveMemberByEmail(string name, string email)
    {
        using ApplicationDbContext db = new();
        // validator would've rejected no match
        Student member = db.Student.FirstOrDefault(e => e.Email == email)!;
        Group group = db.Group.FirstOrDefault(e => e.Name == name)!;

        if (member.Id == group.LeaderId)
        {
            return group;
        }

        StudentGroup studentGroup = db.StudentGroup.FirstOrDefault(
            e => e.StudentId == member.Id && e.GroupId == group.Id
        )!;

        _ = db.StudentGroup.Remove(studentGroup);
        _ = db.SaveChanges();

        return group;
    }

    public GroupListViewModel GenerateGroupListViewModel()
    {
        using ApplicationDbContext db = new();
        List<StudentGroup> studentGroups = [.. db.StudentGroup];

        GroupListViewModel model = new() { Groups = [] };
        HashSet<int> groupIds = [];
        foreach (StudentGroup studentGroup in studentGroups)
        {
            // ignore null bc if it's in the db, it conforms to the foreign key contraint
            // which was checked by the validator
            Student student = db.Student.FirstOrDefault(e => e.Id == studentGroup.StudentId)!;
            Group group = db.Group.FirstOrDefault(e => e.Id == studentGroup.GroupId)!;

            StudentViewModel studentViewModel =
                new()
                {
                    Email = student.Email,
                    FirstName = student.FirstName,
                    LastName = student.LastName
                };

            if (groupIds.Add(group.Id))
            {
                Student leader = db.Student.FirstOrDefault(e => e.Id == group.LeaderId)!;
                StudentViewModel leaderModel =
                    new()
                    {
                        Email = leader.Email,
                        FirstName = leader.FirstName,
                        LastName = leader.LastName
                    };

                model.Groups.Add(
                    new()
                    {
                        Id = group.Id,
                        Name = group.Name,
                        Leader = leaderModel,
                        Members = [studentViewModel]
                    }
                );

                continue;
            }

            model.Groups.FirstOrDefault(e => e.Id == group.Id)?.Members.Add(studentViewModel);
        }

        return model;
    }

    public GroupViewModel GenerateGroupViewModel(Group group)
    {
        using ApplicationDbContext db = new();
        List<StudentGroup> studentGroups = db.StudentGroup.Where(e => e.GroupId == group.Id)
            .ToList();

        HashSet<string> lookup = db.StudentGroup.Where(e => e.GroupId == group.Id)
            .Select(e => e.StudentId)
            .ToHashSet();

        List<StudentViewModel> members = db.Student.Where(e => lookup.Contains(e.Id))
            .Select(
                e =>
                    new StudentViewModel
                    {
                        Email = e.Email,
                        FirstName = e.FirstName,
                        LastName = e.LastName
                    }
            )
            .ToList();

        Student leader = db.Student.FirstOrDefault(e => e.Id == group.LeaderId)!;

        StudentViewModel leaderModel =
            new()
            {
                Email = leader.Email,
                FirstName = leader.FirstName,
                LastName = leader.LastName
            };

        GroupViewModel model =
            new()
            {
                Id = group.Id,
                Name = group.Name,
                Leader = leaderModel,
                Members = members
            };

        return model;
    }
}

