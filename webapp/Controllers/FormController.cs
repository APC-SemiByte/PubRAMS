using System.Diagnostics;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

using webapp.Helpers;
using webapp.Models;
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

    public async Task<IActionResult> Roles(string? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        if (id == null)
        {
            BadRequest();
        }

        StaffManager manager = new();
        OptionsViewModel model = new() { Options = manager.GetAvailableRoles((string)id!) };
        ViewData["OptionName"] = "role";
        return PartialView("/Views/Shared/FormComponents/_GenericSelector.cshtml", model);
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
        UsersViewModel model = manager.GenerateUsersViewModel();
        ViewData["OptionName"] = "leader";
        return PartialView("/Views/Shared/FormComponents/_UserSelector.cshtml", model);
    }

    public async Task<IActionResult> GroupMembers(string? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        if (id == null)
        {
            return BadRequest();
        }

        StudentManager manager = new();
        UsersViewModel model = manager.GenerateUsersViewModel(id);
        ViewData["OptionName"] = "member";
        return PartialView("/Views/Shared/FormComponents/_UserSelector.cshtml", model);
    }

    public async Task<IActionResult> NonGroupMembers(string? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        if (id == null)
        {
            return BadRequest();
        }

        StudentManager manager = new();
        UsersViewModel model = manager.GenerateUsersViewModel(id, invert: true);
        ViewData["OptionName"] = "student";
        return PartialView("/Views/Shared/FormComponents/_UserSelector.cshtml", model);
    }

    public async Task<IActionResult> Groups(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.StudentOnly().GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        GroupManager manager = new();
        OptionsViewModel model = new() { Options = manager.GetInvolvedGroups((Student)user) };

        ProjectManager projectManager = new();
        ViewData["OptionName"] = "group";
        ViewData["SelectedOption"] = projectManager.GetGroup(id);
        return PartialView("/Views/Shared/FormComponents/_GenericSelector.cshtml", model);
    }

    public async Task<IActionResult> Staff()
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        StaffManager manager = new();
        UsersViewModel model = manager.GenerateUsersViewModel();

        return PartialView("/Views/Shared/FormComponents/_UserSelector.cshtml", model);
    }

    public async Task<IActionResult> Adviser(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        StaffManager manager = new();
        UsersViewModel model = manager.GenerateUsersViewModel();

        ProjectManager projectManager = new();
        ViewData["OptionName"] = "adviser";
        ViewData["SelectedUser"] = projectManager.GetAdviser(id);

        return PartialView("/Views/Shared/FormComponents/_UserSelector.cshtml", model);
    }

    public async Task<IActionResult> Instructor(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        StaffManager manager = new();
        UsersViewModel model = manager.GenerateUsersViewModel();

        ProjectManager projectManager = new();
        ViewData["OptionName"] = "instructor";
        ViewData["SelectedUser"] = projectManager.GetInstructor(id);

        return PartialView("/Views/Shared/FormComponents/_UserSelector.cshtml", model);
    }

    public async Task<IActionResult> Schools(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        ConstManager manager = new();
        OptionsViewModel model = new() { Options = manager.GetSchools() };

        ProjectManager projectManager = new();
        ViewData["OptionName"] = "school";
        ViewData["SelectedOption"] = projectManager.GetSchool(id);

        return PartialView("/Views/Shared/FormComponents/_SchoolSelector.cshtml", model);
    }

    public async Task<IActionResult> Courses(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        ConstManager manager = new();
        OptionsViewModel model = new() { Options = manager.GetCourses() };

        ProjectManager projectManager = new();
        ViewData["OptionName"] = "course";
        ViewData["SelectedOption"] = projectManager.GetCourse(id);
        return PartialView("/Views/Shared/FormComponents/_GenericSelector.cshtml", model);
    }

    public async Task<IActionResult> Subjects(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        ConstManager manager = new();
        OptionsViewModel model = new() { Options = manager.GetSubjects() };

        ProjectManager projectManager = new();
        ViewData["OptionName"] = "subject";
        ViewData["SelectedOption"] = projectManager.GetSubject(id);
        return PartialView("/Views/Shared/FormComponents/_GenericSelector.cshtml", model);
    }

    public async Task<IActionResult> SchoolRelated(string? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        if (id == null)
        {
            return BadRequest();
        }

        ConstManager manager = new();
        SchoolRelatedOptionsViewModel model = manager.GenerateSchoolRelatedOptionsViewModel(
            (string)id
        );
        return PartialView("/Views/Shared/FormComponents/_SchoolRelatedSelector.cshtml", model);
    }

    public async Task<IActionResult> Proofreaders()
    {
        AuthHelper gh = new();
        IUser? user = await gh.RolesOnly([(int)Models.Roles.EcHead]).GetUser(_graphApi, _logger);
        if (user == null)
        {
            return Unauthorized();
        }

        StaffManager manager = new();
        UsersViewModel model = manager.GenerateUsersViewModel((int)Models.Roles.EcFaculty);
        return PartialView("/Views/Shared/FormComponents/_UserSelector.cshtml", model);
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