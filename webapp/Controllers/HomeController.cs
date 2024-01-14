using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using webapp.Helpers;
using webapp.Models;

namespace webapp.Controllers;

[AuthorizeForScopes(ScopeKeySection = "GraphApi:Scopes")]
public class HomeController(ILogger<HomeController> logger, IDownstreamApi graphApi) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IDownstreamApi _graphApi = graphApi;

    public async Task<IActionResult> Index()
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        return user == null ? Unauthorized() : View();
    }

    public async Task<IActionResult> Privacy()
    {
        AuthHelper gh = new();
        IUser? user = await gh.GetUser(_graphApi, _logger);
        return user == null ? Unauthorized() : View();
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
