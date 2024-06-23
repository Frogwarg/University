using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;
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

        public async Task<IActionResult> Index()
        {
            await db.SaveChangesAsync();
            return View();
        }
        public async Task<IActionResult> Groups(string? selectedCourse, string searchTerm)
        {
            var groups = await db.Groups.Include(g => g.Course).Include(g => g.Curator).ToListAsync();
            var uniqueCourses = groups.Select(g => g.Course.Name).Distinct().ToList();
            List<University.Models.Group> filteredGroups = groups;

            if (!string.IsNullOrEmpty(selectedCourse))
            {
                filteredGroups = filteredGroups.Where(g => g.Course.Name == selectedCourse).ToList();
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchTermcopy = searchTerm.ToLower();
                filteredGroups = filteredGroups.Where(g => g.Name.ToLower().Contains(searchTermcopy)).ToList();
            }

            var model = new GroupsViewModel
            {
                Groups = groups,
                UniqueCourses = uniqueCourses,
                SelectedCourse = selectedCourse,
                SearchTerm = searchTerm,
                FilteredGroups = filteredGroups
            };

            return View("Groups", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchGroups(string? selectedCourse, string searchTerm)
        {
            return RedirectToAction("Groups", new { selectedCourse, searchTerm });
        }


        public async Task<IActionResult> Students(string? searchFIO, string? searchGroup, string? selectedStudent)
        {
            var students = await db.Students.Include(g => g.Group).ToListAsync();
            List<Student> filteredStudents = students;

            if (!string.IsNullOrEmpty(searchFIO))
            {
                var searchFIOcopy = searchFIO.ToLower();
                filteredStudents = SearchPeople(filteredStudents, searchFIOcopy);
            }
            if (!string.IsNullOrEmpty(searchGroup))
            {
                searchGroup = searchGroup.ToLower();
                filteredStudents = filteredStudents.Where(g => g.Group.Name.ToLower().Contains(searchGroup)).ToList();
            }

            var model = new StudentsViewModel
            {
                Students = students,
                SearchFIO= searchFIO,
                SearchGroup= searchGroup,
                FilteredStudents = filteredStudents,
                SelectedStudent=students.Find(g=>g.Name==selectedStudent)
            };

            return View("Students", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchStudents(string? searchFIO, string? searchGroup, string selectedStudent = null)
        {
            return RedirectToAction("Students", new { searchFIO, searchGroup, selectedStudent });
        }


        public async Task<IActionResult> Teachers(string? searchFIO, string? selectedSubject, string? selectedDegree, string? selectedTeacher)
        {
            var teachers = await db.Teachers.Include(g => g.Subjects).ToListAsync();
            List<string> uniqueDegrees = teachers.SelectMany(g => g.Degrees).Distinct().ToList();
            List<Teacher> filteredTeachers = teachers;

            if (!string.IsNullOrEmpty(searchFIO))
            {
                var searchFIOcopy = searchFIO.ToLower();
                filteredTeachers = SearchPeople(filteredTeachers, searchFIOcopy);
            }

            if (!string.IsNullOrEmpty(selectedDegree))
            {
                filteredTeachers = filteredTeachers.Where(g => g.Degrees.Contains(selectedDegree)).ToList();
            }

            if (!string.IsNullOrEmpty(selectedSubject))
            {
                filteredTeachers = filteredTeachers.Where(teacher => teacher.Subjects.Any(subject => subject.Name == selectedSubject)).ToList();
            }
            var subjects = await db.Subjects.ToListAsync();
            Console.WriteLine("Èìÿ ó÷èòåëÿ: "+ selectedTeacher);
            var model = new TeachersViewModel
            {
                Teachers = teachers,
                SearchFIO = searchFIO,
                UniqueDegrees=uniqueDegrees,
                SelectedDegree=selectedDegree,
                Subjects = subjects,
                SelectedSubject=subjects.Find(g=>g.Name == selectedSubject),
                FilteredTeachers = filteredTeachers,
                SelectedTeacher = teachers.Find(g=> g.Name == selectedTeacher)
            };

            return View("Teachers", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchTeachers(string? searchFIO, string? selectedSubject, string? selectedDegree, string selectedTeacher = null)
        {
            return RedirectToAction("Teachers", new { searchFIO, selectedSubject, selectedDegree, selectedTeacher });
        }


        public async Task<IActionResult> Subjects(string? searchSubject)
        {
            var subjects = await db.Subjects.ToListAsync();
            List<Subject> filteredSubjects = subjects;

            if (!string.IsNullOrEmpty(searchSubject))
            {
                var searchSubjectCopy = searchSubject.ToLower();
                filteredSubjects = filteredSubjects.Where(g => g.Name.ToLower().Contains(searchSubjectCopy)).ToList();
            }

            var model = new SubjectsViewModel
            {
                Subjects = subjects,
                SearchSubject = searchSubject,
                FilteredSubjects = filteredSubjects
            };

            return View("Subjects", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchSubjects(string? searchSubject)
        {
            return RedirectToAction("Subjects", new { searchSubject});
        }


        public static List<T> SearchPeople<T>(List<T> people, string searchFIO)
        {
            string[] searchTerms = searchFIO.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Type type = typeof(T);

            // Ïîëó÷àåì ñâîéñòâà îáúåêòà
            PropertyInfo èìÿProperty = type.GetProperty("Name");
            PropertyInfo ôàìèëèÿProperty = type.GetProperty("Surname");
            PropertyInfo îò÷åñòâîProperty = type.GetProperty("Patronymic");

            if (èìÿProperty == null || ôàìèëèÿProperty == null || îò÷åñòâîProperty == null)
            {
                throw new ArgumentException("Êëàññ íå ñîäåðæèò íåîáõîäèìûå ñâîéñòâà.");
            }

            var results = people.Where(p =>
                searchTerms.All(term =>
                    (èìÿProperty.GetValue(p)?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (ôàìèëèÿProperty.GetValue(p)?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (îò÷åñòâîProperty.GetValue(p)?.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
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
