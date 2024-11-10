using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University.Models;
using University.ViewModels;

namespace University.Controllers
{
    public class GroupController : Controller
    {
        private readonly ILogger<GroupController> _logger;

        ApplicationContext db;

        public GroupController(ILogger<GroupController> logger, ApplicationContext context)
        {
            _logger = logger;
            db = context;
        }

        public async Task<IActionResult> Groups(Guid? selectedCourse, string searchTerm)
        {
            var groups = await db.Groups.Include(g => g.Course).Include(g => g.Curator).ToListAsync();
            var courses = await db.Courses.ToListAsync();

            var uniqueNames = new HashSet<string>();
            var uniqueCourses = new List<Course>();

            foreach (var course in courses)
            {
                if (uniqueNames.Add(course.Name))
                {
                    uniqueCourses.Add(course);
                }
            }

            List<Group> filteredGroups = groups;
            Course selectedcourse = new Course();

            if (selectedCourse != null && selectedCourse != Guid.Empty)
            {
                filteredGroups = filteredGroups.Where(g => g.Course.Id == selectedCourse).ToList();
                selectedcourse = await db.Courses.FirstOrDefaultAsync(g => g.Id == selectedCourse);
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
                SelectedCourse = selectedcourse,
                SearchTerm = searchTerm,
                FilteredGroups = filteredGroups,
                Teachers = await db.Teachers.ToListAsync()
            };

            ViewData["Title"] = "Groups List";
            return View("Groups", model);
        }
        [HttpGet]
        public async Task<IActionResult> SearchGroups(Guid? selectedCourse, string searchTerm)
        {
            return RedirectToAction("Groups", new { selectedCourse, searchTerm });
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup(string? createName, Guid createCourse, Guid createCurator, string createDesc = "")
        {
            Group newGroup = new Group
            {
                Name = createName,
                Description = createDesc,
                CourseId = (createCourse != null && createCourse != Guid.Empty ? createCourse : null),
                Course = (createCourse != null && createCourse != Guid.Empty ? await db.Courses.FindAsync(createCourse) : null),
                CuratorId = (createCurator != null && createCurator != Guid.Empty) ? createCurator : null,
                Curator = (createCurator != null && createCurator != Guid.Empty) ? await db.Teachers.FindAsync(createCurator) : null
            };
            await db.Groups.AddAsync(newGroup);
            await db.SaveChangesAsync();
            return RedirectToAction("Groups");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateGroup(Guid updateId, string? updateName, Guid updateCourse, Guid updateCurator, string updateDesc = "")
        {
            var group = await db.Groups.Include(g => g.Course).Include(g => g.Curator).FirstOrDefaultAsync(t => t.Id == updateId);
            if (group != null)
            {
                group.Name = updateName;
                group.Description = updateDesc;
                group.CourseId = (updateCourse != null && updateCourse != Guid.Empty) ? updateCourse : null;
                group.Course = (updateCourse != null && updateCourse != Guid.Empty ? await db.Courses.FindAsync(updateCourse) : null);

                if (updateCurator != null && updateCurator != Guid.Empty)
                {
                    group.CuratorId = updateCurator;
                    group.Curator = await db.Teachers.FindAsync(updateCurator);
                }
                db.Update(group);
                await db.SaveChangesAsync();
            }
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

        [HttpGet]
        public async Task<IActionResult> VerifyGroupNameAsync(string createName)
        {
            var isUnique = !await db.Groups.AnyAsync(t => t.Name == createName);
            return Json(isUnique);
        }
    }
}
