using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using webapp.Models.ViewModels;
using webapp.Helpers;
using webapp.Models;

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
        IUser? user = await GraphHelper.GetUser(_graphApi, _logger);
        Type? userType = user?.GetType();

        switch (userType)
        {
            case Type when userType == typeof(Student):
                break;

            case Type when userType == typeof(Staff):
                break;
            default:
                break;
        }

        return View();
    }

    public IActionResult Submit()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(
        [Bind("Title,Abstract,InstructorEmail")] SubmissionModel viewModel
    )
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        IUser? user = await GraphHelper.GetUser(_graphApi, _logger);

        return user?.GetType() == typeof(Staff)
            ? Redirect("/Home/Privacy")
            : (IActionResult)Redirect("/Home/Index");
    }
}