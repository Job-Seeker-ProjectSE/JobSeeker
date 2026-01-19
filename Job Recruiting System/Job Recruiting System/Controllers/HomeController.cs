using Job_Recruiting_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Job_Recruiting_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()  // When we put a href to a link it will redirect to this Page! Index = HomePage
        {
            return View(); 
        }

        public IActionResult Privacy() // The same as above but in this case it will go to Privacy page.
        {
            return View();
        }

        // The code below is to catch Errors. Currently its the same for any type of error.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
