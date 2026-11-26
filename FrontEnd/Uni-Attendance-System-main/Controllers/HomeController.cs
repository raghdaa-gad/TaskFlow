using Microsoft.AspNetCore.Mvc;
using Uni_Attendance.Models;

namespace UniversityAttendanceSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDataStore _store;

        public HomeController(AppDataStore store)
        {
            _store = store;
        }

        public IActionResult Index()
        {
            var projects = _store.GetProjects();
            var members = _store.GetMembers();
            var tasks = _store.GetTasks();

            var model = new DashboardViewModel
            {
                TotalProjects = projects.Count,
                InProgressProjects = projects.Count(p => p.Status == "In Progress"),
                CompletedProjects = projects.Count(p => p.Status == "Completed"),
                TotalMembers = members.Count,
                OpenTasks = tasks.Count(t => t.Status != "Done"),
                OverdueTasks = tasks.Count(t =>
                    t.Status != "Done" && t.DueDate.HasValue && t.DueDate.Value.Date < DateTime.Today),

                RecentProjects = projects
                    .OrderByDescending(p => p.Id)
                    .Take(3)
                    .ToList(),

                StatusLabels = AppDataStore.ProjectStatuses.ToList(),
                StatusCounts = AppDataStore.ProjectStatuses
                    .Select(status => projects.Count(p => p.Status == status))
                    .ToList(),

                PriorityLabels = AppDataStore.Priorities.ToList(),
                PriorityCounts = AppDataStore.Priorities
                    .Select(priority => tasks.Count(t => t.Priority == priority))
                    .ToList()
            };

            return View(model);
        }
    }
}
