using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Uni_Attendance.Models;

namespace UniversityAttendanceSystem.Controllers
{
    public class TeamMembersController : Controller
    {
        private readonly AppDataStore _store;

        public TeamMembersController(AppDataStore store)
        {
            _store = store;
        }

        public IActionResult Index()
        {
            ViewBag.Projects = _store.GetProjects();
            return View(_store.GetMembers());
        }

        public IActionResult Details(int id)
        {
            var member = _store.FindMember(id);
            if (member is null) return NotFound();

            ViewBag.AssignedProjects = _store.GetProjects()
                .Where(p => member.ProjectIds.Contains(p.Id))
                .ToList();

            return View(member);
        }

        public IActionResult Create()
        {
            LoadProjectOptions();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TeamMember member)
        {
            if (!ModelState.IsValid)
            {
                LoadProjectOptions();
                return View(member);
            }

            if (_store.GetMembers().Any(m =>
                    m.Email.Equals(member.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(nameof(member.Email), "A member with this email already exists.");
                LoadProjectOptions();
                return View(member);
            }

            member.FullName = member.FullName.Trim();
            member.Email = member.Email.Trim();
            _store.AddMember(member);
            TempData["SuccessMessage"] = "Team member added successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var member = _store.FindMember(id);
            if (member is null) return NotFound();

            LoadProjectOptions();
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TeamMember member)
        {
            if (id != member.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                LoadProjectOptions();
                return View(member);
            }

            if (_store.GetMembers().Any(m =>
                    m.Id != id &&
                    m.Email.Equals(member.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(nameof(member.Email), "A member with this email already exists.");
                LoadProjectOptions();
                return View(member);
            }

            member.FullName = member.FullName.Trim();
            member.Email = member.Email.Trim();
            if (!_store.UpdateMember(member)) return NotFound();

            TempData["SuccessMessage"] = "Team member updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (!_store.DeleteMember(id))
            {
                TempData["ErrorMessage"] = "Team member could not be found.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Team member deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void LoadProjectOptions()
        {
            ViewBag.ProjectOptions = new MultiSelectList(
                _store.GetProjects(), "Id", "Name");
        }
    }
}
