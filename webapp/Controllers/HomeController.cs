using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using webapp.Models;

namespace webapp.Controllers;

[AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDownstreamApi _downstreamApi;

    public HomeController(ILogger<HomeController> logger,
            IDownstreamApi downstreamApi)
    {
        _logger = logger;
        _downstreamApi = downstreamApi;
    }

    public async Task<IActionResult> IndexAsync()
    {
        using var response = await _downstreamApi.CallApiForUserAsync("DownstreamApi").ConfigureAwait(false);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var apiResult = await response.Content.ReadFromJsonAsync<JsonDocument>().ConfigureAwait(false);
            ViewData["ApiResult"] = JsonSerializer.Serialize(apiResult, new JsonSerializerOptions { WriteIndented = true });
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}: {error}");
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult AddProject()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult MyProject()
    {
        return View();
    }

    public IActionResult Cataloging()
    {
        return View();
    }

    public IActionResult AllProjects()
    {
        return View();
    }

    public IActionResult PendingProjects()
    {
        return View();
    }

    public IActionResult AllGroups()
    {
        return View();
    }

    public IActionResult TrackerEngHead()
    {
        return View();
    }

   public IActionResult Roles()
    {
        return View();
    }

       public IActionResult TrackerProofreader()
    {
        return View();
    }



    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
