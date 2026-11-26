using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Uni_Attendance.Models;

namespace UniversityAttendanceSystem.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly AppDataStore _store;

        public ProjectsController(AppDataStore store)
        {
            _store = store;
        }

        public IActionResult Index()
        {
            return View(_store.GetProjects());
        }

        public IActionResult Details(int id)
        {
            var project = _store.FindProject(id);
            if (project is null) return NotFound();

            ViewBag.ProjectMembers = _store.GetMembers()
                .Where(m => project.MemberIds.Contains(m.Id))
                .ToList();

            return View(project);
        }

        public IActionResult Create()
        {
            LoadMemberOptions();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Project project)
        {
            if (!ModelState.IsValid)
            {
                LoadMemberOptions();
                return View(project);
            }

            if (_store.GetProjects().Any(p =>
                    p.Name.Equals(project.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(nameof(project.Name), "A project with this name already exists.");
                LoadMemberOptions();
                return View(project);
            }

            project.Name = project.Name.Trim();
            _store.AddProject(project);
            TempData["SuccessMessage"] = "Project created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var project = _store.FindProject(id);
            if (project is null) return NotFound();

            LoadMemberOptions();
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Project project)
        {
            if (id != project.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                LoadMemberOptions();
                return View(project);
            }

            if (_store.GetProjects().Any(p =>
                    p.Id != id &&
                    p.Name.Equals(project.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(nameof(project.Name), "A project with this name already exists.");
                LoadMemberOptions();
                return View(project);
            }

            project.Name = project.Name.Trim();
            if (!_store.UpdateProject(project)) return NotFound();

            TempData["SuccessMessage"] = "Project updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (!_store.DeleteProject(id))
            {
                TempData["ErrorMessage"] = "Project could not be found.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Project deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Kanban()
        {
            var tasks = _store.GetTasks();
            PopulateTaskNames(tasks);
            return View(tasks);
        }

        private void LoadMemberOptions()
        {
            ViewBag.MemberOptions = new MultiSelectList(
                _store.GetMembers(), "Id", "FullName");
        }

        private void PopulateTaskNames(IEnumerable<TaskItem> tasks)
        {
            var projects = _store.GetProjects();
            var members = _store.GetMembers();

            foreach (var task in tasks)
            {
                task.ProjectName = projects.FirstOrDefault(p => p.Id == task.ProjectId)?.Name ?? "Unknown project";
                var member = members.FirstOrDefault(m => m.Id == task.AssigneeId);
                task.AssigneeName = member?.FullName ?? "Unassigned";
                task.AssigneeInitials = string.Join(string.Empty,
                    (member?.FullName ?? "?")
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => part[0])
                    .Take(2));
            }
        }
    }
}
