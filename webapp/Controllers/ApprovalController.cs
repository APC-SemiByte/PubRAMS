using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using webapp.Helpers;
using webapp.Models;
using webapp.Models.Dtos;

namespace webapp.Controllers;

// TODO:
//   - submission
//   - approval
//   - tracking

[AuthorizeForScopes(ScopeKeySection = "GraphApi:Scopes")]
public class ApprovalController(ILogger<ApprovalController> logger, IDownstreamApi graphApi)
    : Controller
{
    private readonly ILogger<ApprovalController> _logger = logger;
    private readonly IDownstreamApi _graphApi = graphApi;

    public async Task<IActionResult> Index()
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);

        return View();
    }

    public IActionResult Submit()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int? id, [FromBody] SubmissionDto viewModel)
    {
        if (!ModelState.IsValid)
        {
            return Redirect("/");
        }

        AuthHelper gh = new();
        IUser? user = await gh.StudentOnly().GetUser(_graphApi, _logger);

        return user == null ? Redirect("/Roles") : Redirect("/Home/Privacy");
    }
}