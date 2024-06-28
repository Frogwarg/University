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

            // Получаем свойства объекта
            PropertyInfo имяProperty = type.GetProperty("Name");
            PropertyInfo фамилияProperty = type.GetProperty("Surname");
            PropertyInfo отчествоProperty = type.GetProperty("Patronymic");

            if (имяProperty == null || фамилияProperty == null || отчествоProperty == null)
            {
                throw new ArgumentException("Класс не содержит необходимые свойства.");
            }

            var results = people.Where(p =>
                searchTerms.All(term =>
                    (имяProperty.GetValue(p)?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (фамилияProperty.GetValue(p)?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (отчествоProperty.GetValue(p)?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
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
