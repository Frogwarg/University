using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Models;

namespace University.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ILogger<TeacherController> _logger;

        ApplicationContext db;

        public TeacherController(ILogger<TeacherController> logger, ApplicationContext context)
        {
            _logger = logger;
            db = context;
        }
        public async Task<IActionResult> Teachers(string? searchFIO, string? selectedSubject, string? selectedDegree, string? selectedTeacher)
        {
            var teachers = await db.Teachers.Include(g => g.Subjects).ToListAsync();
            List<string> uniqueDegrees = teachers.SelectMany(g => g.Degrees).Distinct().ToList();
            List<Teacher> filteredTeachers = teachers;

            if (!string.IsNullOrEmpty(searchFIO))
            {
                var searchFIOcopy = searchFIO.ToLower();
                filteredTeachers = HomeController.SearchPeople(filteredTeachers, searchFIOcopy);
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
            var model = new TeachersViewModel
            {
                Teachers = teachers,
                SearchFIO = searchFIO,
                UniqueDegrees = uniqueDegrees,
                SelectedDegree = selectedDegree,
                Subjects = subjects,
                SelectedSubject = subjects.Find(g => g.Name == selectedSubject),
                FilteredTeachers = filteredTeachers,
                SelectedTeacher = teachers.Find(g => g.Name == selectedTeacher)
            };

            ViewData["Title"] = "Teachers List";
            return View("Teachers", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchTeachers(string? searchFIO, string? selectedSubject, string? selectedDegree, string selectedTeacher = null)
        {
            return RedirectToAction("Teachers", new { searchFIO, selectedSubject, selectedDegree, selectedTeacher });
        }
        [HttpPost]
        public async Task<IActionResult> CreateTeacher(string? createName, string? createSurname, string? createPatronymic, DateTime createDateOfBirth, string? createDegrees, string? createDesc, IFormFile createImage, List<Guid> crSelectedSubjects)
        {
            List<string> degrees = new List<string>();
            if (!string.IsNullOrEmpty(createDegrees))
            {
                degrees = createDegrees.Split(",").ToList();
                foreach (string degree in degrees)
                {
                    degree.Trim();
                }
            }
            Guid newID = Guid.NewGuid();
            Teacher newTeacher = new Teacher
            {
                Id = newID,
                Name = createName,
                Surname = createSurname,
                Patronymic = createPatronymic,
                DateOfBirth = new DateTimeOffset(DateTime.SpecifyKind(createDateOfBirth, DateTimeKind.Utc)),
                Degrees = degrees,
                Description = createDesc,
                Subjects = new List<Subject>()
            };

            List<Subject> subjects = new List<Subject>();
            if (crSelectedSubjects.Count != 0)
            {
                foreach (var subjectId in crSelectedSubjects)
                {
                    var subject = await db.Subjects.FindAsync(subjectId);
                    if (subject != null)
                    {
                        newTeacher.Subjects.Add(subject);
                    }
                }
            }
            if (createImage != null && createImage.Length > 0)
            {
                string fileName = $"{newID}{Path.GetExtension(createImage.FileName)}";
                string filePath = Path.Combine("wwwroot/images/Profiles/Teachers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createImage.CopyToAsync(stream);
                }
                newTeacher.ImagePath = $"/images/Profiles/Teachers/{fileName}";
            }

            await db.Teachers.AddAsync(newTeacher);
            await db.SaveChangesAsync();
            return RedirectToAction("Teachers");
        }
        public async Task<IActionResult> UpdateTeacher(Guid? updateId, string? updateName, string? updateSurname, string? updatePatronymic, DateTime updateDateOfBirth, string? updateDegrees, string? updateDesc, IFormFile currentImagePath, List<Guid> SelectedSubjects)
        {
            List<string> degrees = new List<string>();
            if (!string.IsNullOrEmpty(updateDegrees))
            {
                degrees = updateDegrees.Split(",").ToList();
                foreach (string degree in degrees)
                {
                    degree.Trim();
                }
            }
            var teacher = await db.Teachers.Include(t => t.Subjects).FirstOrDefaultAsync(t => t.Id == updateId);
            if (teacher != null) {
                teacher.Degrees = degrees;
                teacher.Name = updateName;
                teacher.Surname = updateSurname;
                teacher.Patronymic = updatePatronymic;
                teacher.DateOfBirth = new DateTimeOffset(DateTime.SpecifyKind(updateDateOfBirth, DateTimeKind.Utc));
                teacher.Description = updateDesc;
                teacher.Subjects.Clear();
                await db.SaveChangesAsync();
            }
            if (SelectedSubjects.Count != 0)
            {
                foreach (var subjectId in SelectedSubjects)
                {
                    var subject = await db.Subjects.FindAsync(subjectId);
                    if (subject != null)
                    {
                        teacher.Subjects.Add(subject);
                    }
                }
            }
            if (currentImagePath != null && currentImagePath.Length > 0 && teacher !=null)
            {
                string fileName = $"{teacher.Id}{Path.GetExtension(currentImagePath.FileName)}";
                string filePath = Path.Combine("wwwroot/images/Profiles/Teachers", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await currentImagePath.CopyToAsync(stream);
                }
                teacher.ImagePath = $"/images/Profiles/Teachers/{fileName}";
            }

            if (teacher != null)
            {
                db.Teachers.Update(teacher);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Teachers");
        }
        public async Task<IActionResult> DeleteTeacher(Guid TeacherID)
        {
            var teacher = await db.Teachers.FindAsync(TeacherID);
            if (teacher != null)
            {
                if (!string.IsNullOrEmpty(teacher.ImagePath))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", teacher.ImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(filePath) && teacher.ImagePath.TrimStart('/') != "images/default_empty.jpg")
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                db.Teachers.Remove(teacher);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Teachers");
        }
    }
}
