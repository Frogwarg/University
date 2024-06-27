using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Models;

namespace University.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ILogger<SubjectController> _logger;

        ApplicationContext db;

        public SubjectController(ILogger<SubjectController> logger, ApplicationContext context)
        {
            _logger = logger;
            db = context;
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
            ViewData["Title"] = "Subjects List";
            return View("Subjects", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchSubjects(string? searchSubject)
        {
            return RedirectToAction("Subjects", new { searchSubject });
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
        [HttpPost]
        public async Task<IActionResult> UpdateSubject(Guid? subjectId, string? updateName)
        {
            var subject = await db.Subjects.FindAsync(subjectId);
            if (subject != null)
            {
                subject.Name = updateName;
                db.Subjects.Update(subject);
                await db.SaveChangesAsync();
            }
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

    }
}
