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

    public List<string> GetInvolvedGroups(Student student)
    {
        using ApplicationDbContext db = new();
        IQueryable<int> groupIds =
            from studentGroup in db.StudentGroup
            where studentGroup.StudentId == student.Id
            select studentGroup.GroupId;

        return (
            from group1 in db.Group
            where groupIds.Any(id => id == group1.Id)
            select group1.Name
        ).ToList();
    }

    public Group ReassignLeaderByEmail(string name, string email)
    {
        using ApplicationDbContext db = new();
        // validator would've rejected no match

        Student member = db.Student.FirstOrDefault(e => e.Email == email)!;
        Group group = db.Group.FirstOrDefault(e => e.Name == name)!;

        StudentGroup? lookup = db.StudentGroup.FirstOrDefault(
            e => e.StudentId == member.Id && e.GroupId == group.Id
        );
        if (lookup == null)
        {
            return group;
        }

        group.LeaderId = member.Id;
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

            UserViewModel studentViewModel =
                new()
                {
                    Email = student.Email,
                    GivenName = student.GivenName,
                    LastName = student.LastName
                };

            if (groupIds.Add(group.Id))
            {
                Student leader = db.Student.FirstOrDefault(e => e.Id == group.LeaderId)!;
                UserViewModel leaderModel =
                    new()
                    {
                        Email = leader.Email,
                        GivenName = leader.GivenName,
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

    public GroupViewModel GenerateGroupViewModel(Group group_)
    {
        using ApplicationDbContext db = new();
        List<StudentGroup> studentGroups = (
            from studentGroup in db.StudentGroup
            where studentGroup.GroupId == group_.Id
            select studentGroup
        ).ToList();

        IQueryable<string> lookup =
            from studentGroup in db.StudentGroup
            where studentGroup.GroupId == group_.Id
            select studentGroup.StudentId;

        List<UserViewModel> members = (
            from student in db.Student
            where lookup.Any(id => id == student.Id)
            select new UserViewModel
            {
                Email = student.Email,
                GivenName = student.GivenName,
                LastName = student.LastName
            }
        ).ToList();

        Student leader = db.Student.FirstOrDefault(e => e.Id == group_.LeaderId)!;

        UserViewModel leaderModel =
            new()
            {
                Email = leader.Email,
                GivenName = leader.GivenName,
                LastName = leader.LastName
            };

        GroupViewModel model =
            new()
            {
                Id = group_.Id,
                Name = group_.Name,
                Leader = leaderModel,
                Members = members
            };

        return model;
    }
}