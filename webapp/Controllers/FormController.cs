using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
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
public class FormController(ILogger<HomeController> logger, IDownstreamApi graphApi) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IDownstreamApi _graphApi = graphApi;

    public async Task<IActionResult> Index()
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        return user == null ? Unauthorized() : View();
    }

    public async Task<IActionResult> Roles()
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        StaffManager manager = new();
        RolesOptionsViewModel model = new() { Roles = manager.GetAvailableRoles() };
        return PartialView("/Views/Shared/FormComponents/_RoleSelector.cshtml", model);
    }

    public async Task<IActionResult> Students()
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        StudentManager manager = new();
        StudentListViewModel model = manager.GenerateStudentListViewModel();
        return PartialView("/Views/Shared/FormComponents/_StudentSelector.cshtml", model);
    }

    public async Task<IActionResult> GroupMembers(GroupInfoDto group)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        StudentManager manager = new();
        StudentListViewModel model = manager.GenerateStudentListViewModelFromGroupName(group.GroupName);
        return PartialView("/Views/Shared/FormComponents/_StudentSelector.cshtml", model);
    }

    public async Task<IActionResult> NonGroupMembers(GroupInfoDto group)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        StudentManager manager = new();
        StudentListViewModel model = manager.GenerateStudentListViewModelFromGroupName(group.GroupName, invert: true);
        return PartialView("/Views/Shared/FormComponents/_StudentSelector.cshtml", model);
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
