using Microsoft.AspNetCore.Mvc;
using webapp.Models.ViewModels;
using webapp.Models.EntityManagers;

namespace webapp.Controllers;

public class RolesController : Controller
{
    public ActionResult Index()
    {
        RoleManager manager = new();
        StaffRolesModel model = manager.GetAllUserRoles();
        return View(model);
    }
}