using Microsoft.AspNetCore.Mvc;

namespace PhoneDirectory.ReportService.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
