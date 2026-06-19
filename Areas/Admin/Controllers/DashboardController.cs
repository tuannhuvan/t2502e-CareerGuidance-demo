using Microsoft.AspNetCore.Mvc;

namespace CareerGuidance.Areas.Admin.Controllers;

public class DashboardController : BaseAdminController
{
    public IActionResult Index()
    {
        return View();
    }
}
