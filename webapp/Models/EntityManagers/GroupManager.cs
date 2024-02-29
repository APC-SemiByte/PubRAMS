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

        var studentInfo =
            from studentGroup in db.StudentGroup
            join student in db.Student on studentGroup.StudentId equals student.Id
            join group_ in db.Group on studentGroup.GroupId equals group_.Id
            join leader in db.Student on group_.LeaderId equals leader.Id
            select new
            {
                Id = student.Id,
                Email = student.Email,
                GivenName = student.GivenName,
                LastName = student.LastName,
                GroupId = group_.Id,
                Group = group_.Name,
                Leader = leader
            };

        var groupInfo =
            from group_ in db.Group
            join leader in db.Student on group_.LeaderId equals leader.Id
            select new
            {
                Id = group_.Id,
                Name = group_.Name,
                Leader = new UserViewModel
                {
                    Id = leader.Id,
                    GivenName = leader.GivenName,
                    LastName = leader.LastName,
                    Email = leader.Email
                }
            };

        List<GroupViewModel> groups = (
            from group_ in groupInfo
            join studentGroup in studentInfo on group_.Id equals studentGroup.GroupId into studentGroups
            select new GroupViewModel
            {
                Info = new()
                {
                    Id = group_.Id,
                    Name = group_.Name,
                    Leader = group_.Leader
                },
                Members = studentGroups.Select(
                    e =>
                        new UserViewModel
                        {
                            Id = e.Id,
                            GivenName = e.GivenName,
                            LastName = e.LastName,
                            Email = e.Email
                        }
                ).ToList()
            }
        ).ToList();

        return new() { Groups = groups };
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
                Id = student.Id,
                Email = student.Email,
                GivenName = student.GivenName,
                LastName = student.LastName
            }
        ).ToList();

        Student leader = db.Student.FirstOrDefault(e => e.Id == group_.LeaderId)!;

        UserViewModel leaderModel =
            new()
            {
                Id = leader.Id,
                Email = leader.Email,
                GivenName = leader.GivenName,
                LastName = leader.LastName
            };

        GroupViewModel model =
            new()
            {
                Info = new()
                {
                    Id = group_.Id,
                    Name = group_.Name,
                    Leader = leaderModel
                },
                Members = members
            };

        return model;
    }
}