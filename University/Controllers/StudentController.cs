using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class StudentController : Controller
    {
        private readonly ILogger<StudentController> _logger;

        ApplicationContext db;

        public StudentController(ILogger<StudentController> logger, ApplicationContext context)
        {
            _logger = logger;
            db = context;
        }

        public async Task<IActionResult> Students(string? searchFIO, string? searchGroup, string? selectedStudent)
        {
            var students = await db.Students.Include(g => g.Group).ToListAsync();
            List<Student> filteredStudents = students;

            if (!string.IsNullOrEmpty(searchFIO))
            {
                var searchFIOcopy = searchFIO.ToLower();
                filteredStudents = HomeController.SearchPeople(filteredStudents, searchFIOcopy);
            }
            if (!string.IsNullOrEmpty(searchGroup))
            {
                searchGroup = searchGroup.ToLower();
                filteredStudents = filteredStudents.Where(g => g.Group != null && g.Group.Name.ToLower().Contains(searchGroup)).ToList();
            }

            var model = new StudentsViewModel
            {
                Students = students,
                SearchFIO = searchFIO,
                SearchGroup = searchGroup,
                FilteredStudents = filteredStudents,
                SelectedStudent = students.Find(g => g.Name == selectedStudent),
                Groups = await db.Groups.Include(g => g.Course).Include(g => g.Curator).ToListAsync()
            };
            ViewData["Title"] = "Students List";
            return View("Students", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchStudents(string? searchFIO, string? searchGroup, string selectedStudent = null)
        {
            return RedirectToAction("Students", new { searchFIO, searchGroup, selectedStudent });
        }
        [HttpPost]
        public async Task<IActionResult> CreateStudent(string? createName, string? createSurname, string? createPatronymic, DateTime createDateOfBirth, string? createDesc, IFormFile createImage, Guid crSelectedGroup)
        {
            Guid newID = Guid.NewGuid();
            Group group = await db.Groups.FindAsync(crSelectedGroup);
            Student newStudent = new Student
            {
                Id = newID,
                Name = createName,
                Surname = createSurname,
                Patronymic = createPatronymic,
                DateOfBirth = new DateTimeOffset(DateTime.SpecifyKind(createDateOfBirth, DateTimeKind.Utc)),
                Description = createDesc
            };
            if (group != null) newStudent.Group = group;
            if (createImage != null && createImage.Length > 0)
            {
                string fileName = $"{newID}{Path.GetExtension(createImage.FileName)}";
                string filePath = Path.Combine("wwwroot/images/Profiles/Students", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createImage.CopyToAsync(stream);
                }
                newStudent.ImagePath = $"/images/Profiles/Students/{fileName}";
            }


            await db.Students.AddAsync(newStudent);
            await db.SaveChangesAsync();
            return RedirectToAction("Students");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStudent(Guid updateId, string? updateName, string? updateSurname, string? updatePatronymic, DateTime updateDateOfBirth, string? updateDesc, IFormFile currentImagePath, Guid selectedGroup)
        {
            Guid newID = Guid.NewGuid();
            Group group = await db.Groups.FindAsync(selectedGroup);
            var student = await db.Students.Include(g => g.Group).FirstOrDefaultAsync(t => t.Id == updateId);
            if (student != null)
            {
                student.Name = updateName;
                student.Surname = updateSurname;
                student.Patronymic = updatePatronymic;
                student.DateOfBirth= new DateTimeOffset(DateTime.SpecifyKind(updateDateOfBirth, DateTimeKind.Utc));
                student.Description = updateDesc;
            }
            if (group != null) {
                student.Group = group;
            }
            if (currentImagePath != null && currentImagePath.Length > 0 && student != null)
            {
                string fileName = $"{student.Id}{Path.GetExtension(currentImagePath.FileName)}";
                string filePath = Path.Combine("wwwroot/images/Profiles/Students", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await currentImagePath.CopyToAsync(stream);
                }
                student.ImagePath = $"/images/Profiles/Students/{fileName}";
            }

            if (student != null)
            {
                db.Students.Update(student);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Students");
        }
        public async Task<IActionResult> DeleteStudent(Guid StudentID)
        {
            var student = await db.Students.FindAsync(StudentID);
            if (student != null)
            {
                if (!string.IsNullOrEmpty(student.ImagePath))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", student.ImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(filePath) && student.ImagePath.TrimStart('/') != "images/default_empty.jpg")
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                db.Students.Remove(student);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Students");
        }
    }
}
