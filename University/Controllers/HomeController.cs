using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using University.Models;
using University.ViewModels;

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

        public async Task<IActionResult> Index()
        {
            await db.SaveChangesAsync();
            return View();
        }


        public static List<T> SearchPeople<T>(List<T> people, string searchFIO)
        {
            string[] searchTerms = searchFIO.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Type type = typeof(T);

            // �������� �������� �������
            PropertyInfo ���Property = type.GetProperty("Name");
            PropertyInfo �������Property = type.GetProperty("Surname");
            PropertyInfo ��������Property = type.GetProperty("Patronymic");

            if (���Property == null || �������Property == null || ��������Property == null)
            {
                throw new ArgumentException("����� �� �������� ����������� ��������.");
            }

            var results = people.Where(p =>
                searchTerms.All(term =>
                    (���Property.GetValue(p)?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (�������Property.GetValue(p)?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (��������Property.GetValue(p)?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                )
            ).ToList();

            return results;
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
