using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using University.Models;

namespace University.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        ApplicationContext db;

        public HomeController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Groups()
        {
            return View("Groups",await db.Groups.Include(g => g.Course).Include(g => g.Curator).ToListAsync());
        }
        public async Task<IActionResult> Students()
        {
            return View("Students", await db.Students.Include(g => g.Group).ToListAsync());
        }
        public async Task<IActionResult> Teachers()
        {
            return View("Teachers", await db.Teachers.Include(g => g.Subjects).ToListAsync());
        }
        public async Task<IActionResult> Subjects()
        {
            return View("Subjects", await db.Subjects.ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
