using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;

        ApplicationContext db;

        public CourseController(ILogger<CourseController> logger, ApplicationContext context)
        {
            _logger = logger;
            db = context;
        }

        public async Task<IActionResult> Courses(string? searchCourse)
        {
            var courses = await db.Courses.ToListAsync();
            List<Course> filteredCourses = courses;

            if (!string.IsNullOrEmpty(searchCourse))
            {
                var searchCourseCopy = searchCourse.ToLower();
                filteredCourses = filteredCourses.Where(g => g.Name.ToLower().Contains(searchCourseCopy)).ToList();
            }

            var model = new CoursesViewModel
            {
                Courses = courses,
                SearchCourse = searchCourse,
                FilteredCourses = filteredCourses
            };

            ViewData["Title"] = "Courses List";
            return View("Courses", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchCourses(string? searchCourse)
        {
            return RedirectToAction("Courses", new { searchCourse });
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse(string? createName, string? createDesc)
        {
            Course newCourse = new Course
            {
                Name = createName,
                Description = createDesc
            };
            await db.Courses.AddAsync(newCourse);
            await db.SaveChangesAsync();
            return RedirectToAction("Courses");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCourse(Guid updateId, string? updateName, string? updateDesc)
        {
            var course = await db.Courses.FirstOrDefaultAsync(c => c.Id == updateId);
            if (course != null)
            {
                course.Name = updateName;
                course.Description = updateDesc;
                db.Courses.Update(course);
                await db.SaveChangesAsync();
            }
            
            return RedirectToAction("Courses");
        }
        public async Task<IActionResult> DeleteCourse(Guid CourseID)
        {
            var course = await db.Courses.FindAsync(CourseID);
            if (course != null)
            {
                db.Courses.Remove(course);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Courses");
        }


        [HttpGet]
        public async Task<IActionResult> VerifyCourseName(string createName)
        {
            var isUnique = !await db.Courses.AnyAsync(t => t.Name == createName);
            return Json(isUnique);
        }
    }
}
