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
        if (user == null)
        {
            return Redirect("/Projects");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> New(
        [Bind("Title,Group,Abstract,DocumentUrl,School,Subject,Course,AdviserEmail,InstructorEmail")]
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
}