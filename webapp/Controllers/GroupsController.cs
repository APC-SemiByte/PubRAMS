using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using webapp.Helpers;
using webapp.Models;
using webapp.Models.Dtos;
using webapp.Models.EntityManagers;
using webapp.Models.ViewModels;

namespace webapp.Controllers;

[AuthorizeForScopes(ScopeKeySection = "GraphApi:Scopes")]
public class GroupsController(IDownstreamApi graphApi) : Controller
{
    private readonly IDownstreamApi _graphApi = graphApi;

    public async Task<ActionResult> Index()
    {
        AuthHelper gh = new();
        // FIXME: USE ADMIN ROLE, THIS IS ONLY FOR TESTING
        /* IUser? user = await gh.RolesOnly(["Admin"]).GetUser(_graphApi); */
        IUser? user = await gh.GetUser(_graphApi);
        if (user == null)
        {
            return Redirect("/Home/Index");
        }

        GroupManager manager = new();
        GroupListViewModel model = manager.GenerateGroupListViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Add(AddGroupDto dto)
    {
        AuthHelper gh = new();
        /* IUser? user = await gh.RolesOnly(["Instructor"]).GetUser(_graphApi); */
        IUser? user = await gh.StaffOnly().GetUser(_graphApi);
        if (user == null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        GroupManager manager = new();
        Group group = manager.AddWithLeaderEmail(dto.Name, dto.LeaderEmail);
        GroupViewModel model = manager.GenerateGroupViewModel(group);

        return PartialView("/Views/Groups/_GroupPartial.cshtml", model);
    }

    [HttpPost]
    public async Task<ActionResult> AddMember(GroupMemberDto dto)
    {
        AuthHelper gh = new();
        // FIXME: USE ADMIN ROLE, THIS IS ONLY FOR TESTING
        /* IUser? user = await gh.RolesOnly(["Admin"]).GetUser(_graphApi); */
        IUser? user = await gh.StaffOnly().GetUser(_graphApi);
        if (user == null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest("Group or user not found.");
        }

        GroupManager manager = new();
        Group group = manager.AddMemberByEmail(dto.GroupName, dto.StudentEmail);
        GroupViewModel model = manager.GenerateGroupViewModel(group);

        return PartialView("/Views/Groups/_GroupPartial.cshtml", model);
    }

    [HttpPost]
    public async Task<ActionResult> RemoveMember(GroupMemberDto dto)
    {
        AuthHelper gh = new();
        // FIXME: USE ADMIN ROLE, THIS IS ONLY FOR TESTING
        /* IUser? user = await gh.RolesOnly(["Admin"]).GetUser(_graphApi); */
        IUser? user = await gh.StaffOnly().GetUser(_graphApi);
        if (user == null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest("Group or user not found.");
        }

        GroupManager manager = new();
        Group group = manager.RemoveMemberByEmail(dto.GroupName, dto.StudentEmail);
        GroupViewModel model = manager.GenerateGroupViewModel(group);

        return PartialView("/Views/Groups/_GroupPartial.cshtml", model);
    }
}
