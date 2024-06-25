using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using University.Models;
using static System.Net.Mime.MediaTypeNames;

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
                Description=createDesc
            };
            await db.Courses.AddAsync(newCourse);
            await db.SaveChangesAsync();
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


        public async Task<IActionResult> Groups(Course? selectedCourse, string searchTerm)
        {
            var groups = await db.Groups.Include(g => g.Course).Include(g => g.Curator).ToListAsync();

            var uniqueNames = new HashSet<string>();
            var uniqueCourses = new List<Course>();

            foreach (var course in db.Courses)
            {
                if (uniqueNames.Add(course.Name))
                {
                    uniqueCourses.Add(course);
                }
            }

            List<Group> filteredGroups = groups;

            if (selectedCourse !=null && !string.IsNullOrEmpty(selectedCourse.Name))
            {
                Console.WriteLine("SelectedCourse.Name: " + selectedCourse.Name);
                filteredGroups = filteredGroups.Where(g => g.Course.Name == selectedCourse.Name).ToList();
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
                FilteredGroups = filteredGroups,
                Teachers=db.Teachers.ToList()
            };

            return View("Groups", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchGroups(Course? selectedCourse, string searchTerm)
        {
            return RedirectToAction("Groups", new { selectedCourse, searchTerm });
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup(string? createName, Guid createCourse, Guid createCurator, string createDesc="")
        {
            Console.WriteLine("Ñîçäàíèå ãðóïïû --------------------------------------------------------------------------------------------");
            Group newGroup = new Group
            {
                Name = createName,
                Description = createDesc,
                CourseId=(createCourse!=null && createCourse != Guid.Empty ? createCourse : null),
                Course = (createCourse != null && createCourse != Guid.Empty ? await db.Courses.FindAsync(createCourse) : null),
                CuratorId = (createCurator != null && createCurator != Guid.Empty) ? createCurator : null,
                Curator = (createCurator!=null && createCurator != Guid.Empty) ?  await db.Teachers.FindAsync(createCurator): null
            };
            await db.Groups.AddAsync(newGroup);
            await db.SaveChangesAsync();
            return RedirectToAction("Groups");
        }
        public async Task<IActionResult> DeleteGroup(Guid GroupID)
        {
            var group = await db.Groups.FindAsync(GroupID);
            if (group != null)
            {
                db.Groups.Remove(group);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Groups");
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
                filteredStudents = filteredStudents.Where(g => g.Group !=null && g.Group.Name.ToLower().Contains(searchGroup)).ToList();
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
            return View("Students", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchStudents(string? searchFIO, string? searchGroup, string selectedStudent = null)
        {
            return RedirectToAction("Students", new { searchFIO, searchGroup, selectedStudent });
        }
        [HttpPost]
        public async Task<IActionResult> CreateStudent(string? createName, string? createSurname, string? createPatronymic, DateTime createDateOfBirth, string? createDesc, IFormFile createImage, Guid selectedGroup)
        {
            Guid newID = Guid.NewGuid();
            Group group = await db.Groups.FindAsync(selectedGroup);
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
        public async Task<IActionResult> DeleteStudent(Guid StudentID)
        {
            var student = await db.Students.FindAsync(StudentID);
            if (student != null)
            {
                if (!string.IsNullOrEmpty(student.ImagePath))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", student.ImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                db.Students.Remove(student);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Students");
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
        [HttpPost]
        public async Task<IActionResult> CreateTeacher(string? createName, string? createSurname, string? createPatronymic, DateTime createDateOfBirth, string? createDegrees, string? createDesc, IFormFile createImage, List<Guid> SelectedSubjects)
        {
            List<string> degrees = new List<string>();
            if (!string.IsNullOrEmpty(createDegrees))
            {
                degrees = createDegrees.Split(",").ToList();
                foreach(string degree in degrees)
                {
                    degree.Trim();
                }
            }
            List<Subject> subjects = new List<Subject>();
            if (SelectedSubjects.Count != 0)
            {
                foreach (var subject in SelectedSubjects)
                {
                    subjects.Add(await db.Subjects.FindAsync(subject));
                }
            }
            Guid newID = Guid.NewGuid();
            Teacher newTeacher = new Teacher
            {
                Id=newID,
                Name = createName,
                Surname = createSurname,
                Patronymic = createPatronymic,
                DateOfBirth = new DateTimeOffset(DateTime.SpecifyKind(createDateOfBirth, DateTimeKind.Utc)),
                Degrees = degrees,
                Description = createDesc,
                Subjects = subjects
            };
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
        public async Task<IActionResult> DeleteTeacher(Guid TeacherID)
        {
            var teacher = await db.Teachers.FindAsync(TeacherID);
            if (teacher != null)
            {
                if (!string.IsNullOrEmpty(teacher.ImagePath))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", teacher.ImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                db.Teachers.Remove(teacher);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Teachers");
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
        [HttpPost]
        public async Task<IActionResult> CreateSubject(string? createName)
        {

            Subject newSubject = new Subject
            {
                Name = createName
            };
            await db.Subjects.AddAsync(newSubject);
            await db.SaveChangesAsync();
            return RedirectToAction("Subjects");
        }
        public async Task<IActionResult> DeleteSubject(Guid SubjectID)
        {
            var subject = await db.Subjects.FindAsync(SubjectID);
            if (subject != null)
            {
                db.Subjects.Remove(subject);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Subjects");
        }
        [HttpGet]
        public IActionResult VerifySubjectName(string createName)
        {
            var isUnique = !db.Subjects.Any(t => t.Name == createName);
            return Json(isUnique);
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
