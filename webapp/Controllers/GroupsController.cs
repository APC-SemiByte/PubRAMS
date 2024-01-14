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
    public async Task<ActionResult> Edit(EditRoleDto dto)
    {
        AuthHelper gh = new();
        // FIXME: USE ADMIN ROLE, THIS IS ONLY FOR TESTING
        /* IUser? user = await gh.RolesOnly(["Admin"]).GetUser(_graphApi); */
        IUser? user = await gh.StaffOnly().GetUser(_graphApi);
        if (user == null)
        {
            return Unauthorized();
        }

        StaffManager manager = new();
        Staff? staff = manager.GetByEmail(dto.Email)!;

        if (!ModelState.IsValid)
        {
            // if you don't care about an indicator for an invalid role in the request,
            // uncomment this next line and comment out everything else in this `if` block
            /* return BadRequest($"Failed to toggle role `{dto.Role}` for user `{dto.Email}`"); */

            if (staff == null)
            {
                return BadRequest("User does not exist.");
            }

            RolesViewModel model = manager.GenerateRolesViewModelFromStaff(staff);
            model.Info = $"Role `{dto.Role}` does not exist";
            return PartialView("/Views/Roles/_RolePartial.cshtml", model);
        }

        if (!manager.ToggleRoleByEmail(dto.Email, dto.Role))
        {
            return Problem("An unknown problem occurred.");
        }

        // ignore null: model validator confirms that the user and role exist.
        RolesViewModel roles = manager.GenerateRolesViewModelFromStaff(staff);
        return PartialView("/Views/Roles/_RolePartial.cshtml", roles);
    }
}

