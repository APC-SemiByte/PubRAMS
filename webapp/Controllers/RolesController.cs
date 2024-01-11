using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using webapp.Helpers;
using webapp.Models;
using webapp.Models.EntityManagers;
using webapp.Models.ViewModels;

namespace webapp.Controllers;

[AuthorizeForScopes(ScopeKeySection = "GraphApi:Scopes")]
public class RolesController(IDownstreamApi graphApi) : Controller
{
    private readonly IDownstreamApi _graphApi = graphApi;

    public async Task<ActionResult> Index()
    {
        GraphHelper gh = new();
        // FIXME: USE ADMIN ROLE, THIS IS ONLY FOR TESTING
        /* List<string> roles = ["Admin"]; */
        List<string> roles = ["Unassigned"];
        IUser? user = await gh.RolesOnly(roles).GetUser(_graphApi);
        if (user == null)
        {
            return Unauthorized();
        }

        RoleManager manager = new();
        StaffRolesView model = manager.GetAllUserRoles();
        return View(model);
    }

    [HttpPost]
    public ActionResult Edit()
    {
        return View("Home/Index");
    }
}

