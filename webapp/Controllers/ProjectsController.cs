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
public class ProjectsController(ILogger<ProjectsController> logger, IDownstreamApi graphApi)
    : Controller
{
    private readonly ILogger<ProjectsController> _logger = logger;
    private readonly IDownstreamApi _graphApi = graphApi;

    public async Task<IActionResult> Index()
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);

        if (user == null)
        {
            return Redirect("/");
        }

        ProjectManager manager = new();
        ProjectListViewModel model = manager.GenerateProjectListViewModel(user);

        return View(model);
    }

    public async Task<IActionResult> New()
    {
        AuthHelper gh = new();
        IUser? user = await gh.StudentOnly().GetUser(_graphApi, _logger);
        return user == null ? Redirect("/Projects") : View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> New(
        [Bind(
            "Title,Group,Abstract,DocumentUrl,School,Subject,Course,AdviserEmail,InstructorEmail"
        )]
            SubmissionDto submission
    )
    {
        AuthHelper gh = new();
        IUser? user = await gh.StudentOnly().GetUser(_graphApi, _logger);

        if (user == null)
        {
            return Redirect("/Projects");
        }

        if (!ModelState.IsValid)
        {
            return View(submission);
        }

        ProjectManager manager = new();
        manager.Add(submission);
        return Redirect("/Projects");
    }

    [HttpPost]
    public async Task<IActionResult> Accept([Bind("ProjectId")] ActionDto dto)
    {
        AuthHelper gh = new();
        IUser? user = await gh.StaffOnly().GetUser(_graphApi, _logger);

        if (user == null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        ProjectManager manager = new();
        bool success = manager.Accept(dto.ProjectId, (Staff)user);

        return success ? Redirect("/Projects") : BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> Reject([Bind("ProjectId")] ActionDto dto)
    {
        AuthHelper gh = new();
        IUser? user = await gh.StaffOnly().GetUser(_graphApi, _logger);

        if (user == null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        ProjectManager manager = new();
        bool success = manager.Reject(dto.ProjectId, (Staff)user);

        return success ? Redirect("/Projects") : BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> Submit([Bind("ProjectId")] ActionDto dto)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);

        if (user == null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        ProjectManager manager = new();
        bool success = manager.Submit(dto.ProjectId, user);

        return success ? Redirect("/Projects") : BadRequest();
    }
}