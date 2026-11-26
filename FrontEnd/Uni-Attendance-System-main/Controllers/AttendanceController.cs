using Microsoft.AspNetCore.Mvc;
using Uni_Attendance.Models;

namespace UniversityAttendanceSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly AppDataStore _store;

        public AttendanceController(AppDataStore store)
        {
            _store = store;
        }

        public IActionResult Index()
        {
            var records = _store.GetAttendanceRecords();
            var today = DateTime.Today;
            var todayRecords = records.Where(r => r.Timestamp.Date == today).ToList();

            var model = new AttendanceViewModel
            {
                Records = records.ToList(),
                TotalStudents = _store.GetStudentCount(),
                PresentToday = todayRecords.Count(r => r.Status == "Present"),
                LateToday = todayRecords.Count(r => r.Status == "Late"),
                AbsentToday = _store.GetStudentCount() - todayRecords.Count
            };

            return View(model);
        }

        // Contract for the future backend:
        // POST /Attendance/CheckIn  { studentId, scannedAt } -> { success, message, studentName, time, status }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckIn([FromBody] CheckInRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.StudentId))
                return BadRequest(new { success = false, message = "Student ID is required." });

            var scannedAt = request.ScannedAt == default ? DateTime.Now : request.ScannedAt;
            var result = _store.CheckIn(request.StudentId.Trim(), scannedAt);

            if (!result.Success)
                return Ok(new { success = false, message = result.Message });

            var record = result.Record!;

            return Ok(new
            {
                success = true,
                message = result.Message,
                studentId = record.StudentId,
                studentName = record.StudentName,
                time = record.Timestamp.ToString("hh:mm tt"),
                status = record.Status
            });
        }

        public IActionResult QRScanner()
        {
            return View();
        }
    }
}
