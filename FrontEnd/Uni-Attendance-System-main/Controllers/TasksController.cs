using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Uni_Attendance.Models;

namespace UniversityAttendanceSystem.Controllers
{
    public class TasksController : Controller
    {
        private readonly AppDataStore _store;

        public TasksController(AppDataStore store)
        {
            _store = store;
        }

        public IActionResult Index()
        {
            var tasks = _store.GetTasks();
            PopulateTaskNames(tasks);

            ViewBag.Projects = _store.GetProjects();
            ViewBag.Members = _store.GetMembers();
            ViewBag.Statuses = AppDataStore.TaskStatuses;
            ViewBag.Priorities = AppDataStore.Priorities;

            return View(tasks);
        }

        public IActionResult Create()
        {
            LoadFormOptions();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskItem task)
        {
            if (!ModelState.IsValid)
            {
                LoadFormOptions();
                return View(task);
            }

            _store.AddTask(task);
            TempData["SuccessMessage"] = "Task created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var task = _store.FindTask(id);
            if (task is null) return NotFound();

            LoadFormOptions();
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TaskItem task)
        {
            if (id != task.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                LoadFormOptions();
                return View(task);
            }

            if (!_store.UpdateTask(task)) return NotFound();

            TempData["SuccessMessage"] = "Task updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (!_store.DeleteTask(id))
            {
                TempData["ErrorMessage"] = "Task could not be found.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Task deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Move(int id, string status)
        {
            if (!_store.MoveTask(id, status))
                return Json(new { success = false, message = "Task or status not found." });

            return Json(new { success = true });
        }

        private void LoadFormOptions()
        {
            ViewBag.ProjectOptions = new SelectList(_store.GetProjects(), "Id", "Name");
            ViewBag.AssigneeOptions = new SelectList(_store.GetMembers(), "Id", "FullName");
            ViewBag.Statuses = new SelectList(AppDataStore.TaskStatuses);
            ViewBag.Priorities = new SelectList(AppDataStore.Priorities);
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
