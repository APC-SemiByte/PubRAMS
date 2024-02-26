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
public class ProjectsController : Controller
{
    private readonly ILogger<ProjectsController> _logger;
    private readonly IDownstreamApi _graphApi;
    private readonly string _filesPath;

    public ProjectsController(ILogger<ProjectsController> logger, IDownstreamApi graphApi)
    {
        _logger = logger;
        _graphApi = graphApi;

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        IConfigurationSection paths = config.GetSection("Paths");
        _filesPath = paths["Files"]!;
    }

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

    public async Task<IActionResult> Download(int id)
    {
        AuthHelper gh = new();
        IUser? _ = await gh.GetUser(_graphApi, _logger);

        if (id == 0)
        {
            return BadRequest();
        }

        ProjectManager manager = new();
        string handle = manager.GetDocumentHandle(id);
        string path = Path.Combine(_filesPath, handle);
        Stream file = System.IO.File.OpenRead(path);

        return File(file, "application/octet-stream", handle);
    }

    public async Task<IActionResult> DownloadPrf(int id)
    {
        AuthHelper gh = new();
        IUser? _ = await gh.GetUser(_graphApi, _logger);

        if (id == 0)
        {
            return BadRequest();
        }

        ProjectManager manager = new();
        string? handle = manager.GetPrfHandle(id);
        if (handle == null)
        {
            return BadRequest();
        }

        string path = Path.Combine(_filesPath, handle);
        Stream file = System.IO.File.OpenRead(path);

        return File(file, "application/octet-stream", handle);
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
            "Title,Group,Abstract,School,Subject,Course,AdviserEmail,InstructorEmail,File"
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
        string handle = manager.Add(submission);
        string path = Path.Combine(_filesPath, handle);
        using Stream file = System.IO.File.Create(path);
        await submission.File.CopyToAsync(file);

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
        bool success = await manager.Accept(dto, (Staff)user);

        return success ? Redirect("/Projects") : BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> Reject([Bind("ProjectId,File")] FileActionDto dto)
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
        bool success = await manager.Reject(dto, (Staff)user, _filesPath);

        return success ? Redirect("/Projects") : BadRequest();
    }

    /// <summary>
    /// For required actions (updating files, revisions)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Submit([Bind("ProjectId,File")] FileActionDto dto)
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
        bool success = await manager.Submit(dto, user, _filesPath);

        return success ? Redirect("/Projects") : BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> Assign([Bind("ProjectId,ProofreaderEmail,Deadline")] AssignDto dto)
    {
        AuthHelper gh = new();
        IUser? user = await gh.RolesOnly([(int)Roles.EcHead]).GetUser(_graphApi, _logger);

        if (user == null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        ProjectManager manager = new();
        bool success = manager.Assign(dto, (Staff)user);

        return success ? Redirect("/Projects") : BadRequest();
    }

    public async Task<IActionResult> Publish(int id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);

        if (user == null)
        {
            return Unauthorized();
        }

        ProjectManager manager = new();
        MarcxmlBuilder builder = manager.Publish(id);

        return View(builder);
    }
}