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
public class RolesController(IDownstreamApi graphApi) : Controller
{
    private readonly IDownstreamApi _graphApi = graphApi;

    public async Task<ActionResult> Index()
    {
        AuthHelper gh = new();
        IUser? user = await gh.RolesOnly([(int)Roles.Admin]).GetUser(_graphApi, ViewData);
        if (user == null)
        {
            return Redirect("/Home/Index");
        }

        StaffManager manager = new();
        RolesListViewModel model = manager.GenerateRolesListViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(EditRoleDto dto)
    {
        AuthHelper gh = new();
        IUser? user = await gh.RolesOnly([(int)Roles.Admin]).GetUser(_graphApi, ViewData);
        if (user == null)
        {
            return Unauthorized();
        }

        StaffManager manager = new();
        Staff? staff = manager.GetByEmail(dto.Email)!;

        if (!ModelState.IsValid)
        {
            return BadRequest($"User or role does not exist.");
        }

        manager.ToggleRoleByEmail(dto.Email, dto.Role);
        RolesViewModel roles = manager.GenerateRolesViewModelFromStaff(staff);
        return PartialView("/Views/Roles/_RolePartial.cshtml", roles);
    }
}