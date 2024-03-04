using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;

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
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        if (user == null)
        {
            return Redirect("/");
        }

        ProjectManager manager = new();
        ProjectListViewModel model = manager.GenerateProjectListViewModel(user);

        return View(model);
    }

    public async Task<IActionResult> View(int? id)
    {
        AuthHelper gh = new();
        IUser user = (await gh.GetUser(_graphApi, _logger))!;
        ViewData["User"] = user;
        ViewData["UserType"] = user.GetType() == typeof(Student) ? "student" : "staff";
        StaffManager staffManager = new();
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null)
        {
            return Redirect("/Projects");
        }

        ProjectViewModel viewmodel = manager.GenerateProjectViewModel(project, user);
        return viewmodel == null ? Redirect("/Projects") : View(viewmodel);
    }

    public async Task<IActionResult> Download(int? id)
    {
        AuthHelper gh = new();
        IUser user = (await gh.GetUser(_graphApi, _logger))!;
        ViewData["User"] = user;
        ViewData["UserType"] = user.GetType() == typeof(Student) ? "student" : "staff";
        StaffManager staffManager = new();
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null)
        {
            return BadRequest();
        }

        string handle = project.BaseHandle + ".docx";
        string path = Path.Combine(_filesPath, handle);
        Stream file = System.IO.File.OpenRead(path);

        return File(file, "application/octet-stream", handle);
    }

    public async Task<IActionResult> DownloadPrf(int? id)
    {
        AuthHelper gh = new();
        IUser user = (await gh.GetUser(_graphApi, _logger))!;
        ViewData["User"] = user;
        ViewData["UserType"] = user.GetType() == typeof(Student) ? "student" : "staff";
        StaffManager staffManager = new();
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null || !project.HasPrf)
        {
            return BadRequest();
        }

        string handle = project.BaseHandle + "-prf.pdf";
        string path = Path.Combine(_filesPath, handle);
        Stream file = System.IO.File.OpenRead(path);

        return File(file, "application/octet-stream", handle);
    }

    public async Task<IActionResult> DownloadPdf(int? id)
    {
        AuthHelper gh = new();
        IUser user = (await gh.GetUser(_graphApi, _logger))!;
        ViewData["User"] = user;
        ViewData["UserType"] = user.GetType() == typeof(Student) ? "student" : "staff";
        StaffManager staffManager = new();
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null || !project.HasPdf)
        {
            return BadRequest();
        }

        string handle = project.BaseHandle + ".pdf";
        string path = Path.Combine(_filesPath, handle);
        Stream file = System.IO.File.OpenRead(path);

        return File(file, "application/octet-stream", handle);
    }

    public async Task<IActionResult> New()
    {
        AuthHelper gh = new();
        IUser? user = await gh.StudentOnly().GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();
        return user == null ? Redirect("/Projects") : View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> New(
        [Bind(
            "Title",
            "Abstract",
            "Continued",
            "Tags",
            "Category",
            "Group",
            "School",
            "Subject",
            "Course",
            "AdviserEmail",
            "InstructorEmail",
            "File"
        )] SubmissionDto dto
    )
    {
        AuthHelper gh = new();
        IUser? user = await gh.StudentOnly().GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        if (user == null)
        {
            return Redirect("/Projects");
        }

        if (!ModelState.IsValid || dto.File == null)
        {
            ModelState.AddModelError("File", "Project manuscript (.docx) is required");
            return View(dto);
        }

        ProjectManager manager = new();
        string handle = manager.Add(dto).BaseHandle + ".docx";
        string path = Path.Combine(_filesPath, handle);
        using Stream file = System.IO.File.Create(path);
        await dto.File.CopyToAsync(file);

        return Redirect("/Projects");
    }

    public async Task<IActionResult> Edit(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.StudentOnly().GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null || !manager.IsEditable(project))
        {
            return Redirect("/Projects");
        }

        SubmissionDto viewModel = manager.GenerateEditSubmissionDto(project);

        ViewData["ProjectId"] = project.Id;
        ViewData["Comment"] = project.StaffComment;
        ViewData["State"] = manager.GenerateStateViewModel(project);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind(
            "Id",
            "Title",
            "Abstract",
            "Continued",
            "Tags",
            "Category",
            "Group",
            "School",
            "Subject",
            "Course",
            "AdviserEmail",
            "InstructorEmail",
            "File",
            "Prf",
            "Pdf",
            "Comment",
            "SubmitFlag"
        )] SubmissionDto dto
    )
    {
        AuthHelper gh = new();
        IUser? user = await gh.StudentOnly().GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null || !manager.IsEditable(project))
        {
            return Redirect("/Projects");
        }

        ViewData["ProjectId"] = project.Id;
        ViewData["Comment"] = project.StaffComment;
        ViewData["State"] = manager.GenerateStateViewModel(project);

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        bool requiresPrf =
            project.StateId == (int)States.PrfStart
            && !project.HasPrf;

        if (requiresPrf && dto.Prf == null)
        {
            ModelState.AddModelError("Prf", "PRF is required");
            return View(dto);
        }

        bool requiresPdf =
            project.StateId == (int)States.Finalizing
            && !project.HasPdf;

        if (requiresPdf && dto.Pdf == null)
        {
            ModelState.AddModelError("Pdf", "PDF converted document is required");
            return View(dto);
        }

        if (dto.File != null)
        {
            string path = Path.Combine(_filesPath, project.BaseHandle + ".docx");
            using Stream file = System.IO.File.Create(path);
            await dto.File.CopyToAsync(file);
        }

        if (dto.Prf != null && project.StateId == (int)States.PrfStart)
        {
            string path = Path.Combine(_filesPath, project.BaseHandle + "-prf.pdf");
            using Stream file = System.IO.File.Create(path);
            await dto.Prf.CopyToAsync(file);
        }

        if (dto.Pdf != null && project.StateId == (int)States.Finalizing)
        {
            string path = Path.Combine(_filesPath, project.BaseHandle + ".pdf");
            using Stream file = System.IO.File.Create(path);
            await dto.Pdf.CopyToAsync(file);
        }

        manager.Edit(project, dto);

        return dto.SubmitFlag != null && (bool)dto.SubmitFlag
            ? Redirect("/Projects/Submit/" + id)
            : Redirect("/Projects");
    }

    public async Task<IActionResult> Accept(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.StaffOnly().GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null)
        {
            return BadRequest();
        }

        bool success = manager.Accept(project, user!);
        return success ? Redirect("/Projects") : BadRequest();
    }

    public async Task<IActionResult> Reject(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.StaffOnly().GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null)
        {
            return Redirect("/Projects");
        }

        ViewData["ProjectInfo"] = manager.GenerateProjectViewModel(project, user!);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int? id, RejectDto dto)
    {
        AuthHelper gh = new();
        IUser? user = await gh.StaffOnly().GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null)
        {
            return Redirect("/Projects");
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        bool success = manager.Reject(project, user!, dto);
        if (!success)
        {
            return BadRequest();
        }

        string handle = project.BaseHandle + ".docx";

        if (dto.File != null)
        {
            string path = Path.Combine(_filesPath, handle);
            using Stream file = System.IO.File.Create(path);
            await dto.File.CopyToAsync(file);
        }

        return Redirect("/Projects");
    }

    public async Task<IActionResult> Submit(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.StudentOnly().GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null)
        {
            return BadRequest();
        }

        if (!manager.IsSubmittable(project))
        {
            return Redirect($"/Projects/Edit/{id}");
        }

        bool success = manager.Submit(project, user!);
        return success ? Redirect("/Projects") : BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> CompletePrf(int? id, CompletePrfDto dto)
    {
        AuthHelper gh = new();
        IUser? user = await gh.RolesOnly([(int)Roles.EcHead]).GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project project = manager.Get(id, user)!;

        if (project == null || !ModelState.IsValid)
        {
            return BadRequest();
        }

        bool success = manager.CompletePrf(project, user!);
        if (!success)
        {
            return BadRequest();
        }

        string handle = project.BaseHandle + "-prf.pdf";
        string path = Path.Combine(_filesPath, handle);
        using Stream file = System.IO.File.Create(path);
        await dto.Prf.CopyToAsync(file);

        HttpContext.Response.Headers.Append("HX-Redirect", "/Projects");
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Assign(
        int? id,
        [Bind("ProofreaderEmail", "Deadline")] AssignDto dto
    )
    {
        AuthHelper gh = new();
        IUser? user = await gh.RolesOnly([(int)Roles.EcHead]).GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null || !ModelState.IsValid)
        {
            return BadRequest();
        }

        bool success = manager.Assign(project, dto, user!);
        if (!success)
        {
            return BadRequest();
        }

        HttpContext.Response.Headers.Append("HX-Redirect", "/Projects");
        return Ok();
    }

    public async Task<IActionResult> PublishRecord(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.RolesOnly([(int)Roles.Librarian]).GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null || project.StateId != (int)States.Publishing)
        {
            return Redirect("/Projects");
        }

        if (project.KohaRecordId != null)
        {
            return Redirect("/Projects/PublishItem/" + id);
        }

        BiblioDto? dto = manager.GenerateBiblioDto(project);
        return dto == null ? Redirect("/Projects") : View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> PublishRecord(
        int? id,
        [Bind(
            "Lead",
            "Title",
            "Subtitle",
            "Authors",
            "PublishPlace",
            "Publisher",
            "Date",
            "Summary",
            "Topic",
            "Subdivision",
            "Uri",
            "LinkText",
            "ItemType"
        )] BiblioDto dto
    )
    {
        AuthHelper gh = new();
        IUser? user = await gh.RolesOnly([(int)Roles.Librarian]).GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null || project.StateId != (int)States.Publishing)
        {
            return Redirect("/Projects");
        }

        if (project.KohaRecordId != null)
        {
            return Redirect("/Projects/PublishItem/" + id);
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        IConfigurationSection kohaApi = config.GetSection("Apis:Koha");
        string uri = kohaApi["BaseUrl"]!;

        MarcxmlBuilder builder = ProjectManager.GenerateKohaRequest(dto);

        HttpClient httpClient = new() { BaseAddress = new(uri) };
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );

        StringContent content = new(builder.ToString(), Encoding.UTF8, "application/marcxml+xml");
        HttpResponseMessage response = await httpClient.PostAsync("/biblios", content);
        string apiResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException(
                $"Rejected by Koha: {response.StatusCode}: {apiResult}"
            );
        }

        JsonNode node = JsonNode.Parse(apiResult)!;
        manager.SaveKohaRecordId(project, (int)node["biblio_id"]!);
        return Redirect($"/Projects/PublishItem/{node["biblio_id"]!}");
    }

    public async Task<IActionResult> PublishItem(int? id)
    {
        AuthHelper gh = new();
        IUser? user = await gh.RolesOnly([(int)Roles.Librarian]).GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null || project.StateId != (int)States.Publishing)
        {
            return Redirect("/Projects");
        }

        BiblioItemDto dto = manager.GenerateBiblioItemDto((int)id!);
        return dto == null ? Redirect("/Projects") : View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> PublishItem(
        int? id,
        [Bind(
            "Id",
            "HomeLibrary",
            "CurrentLibrary",
            "ShelvingLocation",
            "CallNumber",
            "AccessionNumber",
            "CopyNumber",
            "KohaItemType"
        )] BiblioItemDto dto
    )
    {
        AuthHelper gh = new();
        IUser? user = await gh.RolesOnly([(int)Roles.Librarian]).GetUser(_graphApi, _logger);
        StaffManager staffManager = new();
        ViewData["User"] = user;
        ViewData["UserType"] = user?.GetType() == typeof(Student) ? "student" : "staff";
        ViewData["UserRoles"] = staffManager.GetRoles(user).Select(e => e.Id).ToList();

        ProjectManager manager = new();
        Project? project = manager.Get(id, user);

        if (project == null || project.StateId != (int)States.Publishing)
        {
            return Redirect("/Projects");
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        IConfigurationSection kohaApi = config.GetSection("Apis:Koha");
        string uri = kohaApi["BaseUrl"]!;

        HttpClient httpClient = new() { BaseAddress = new(uri) };
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );

        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"/biblios/{project.KohaRecordId}/item", dto);
        string apiResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException(
                $"Rejected by Koha: {response.StatusCode}: {apiResult}"
            );
        }

        manager.MarkPublished(project);
        return Redirect("/Projects");
    }
}