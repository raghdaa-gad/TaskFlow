namespace Uni_Attendance.Models;

public class AttendanceRecord
{
    public int Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } = "Present";
    public string Source { get; set; } = "QR";
}

public class CheckInRequest
{
    public string StudentId { get; set; } = string.Empty;
    public DateTime ScannedAt { get; set; }
}

public class CheckInResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public AttendanceRecord? Record { get; set; }
}

public class AttendanceViewModel
{
    public List<AttendanceRecord> Records { get; set; } = new();
    public int TotalStudents { get; set; }
    public int PresentToday { get; set; }
    public int AbsentToday { get; set; }
    public int LateToday { get; set; }
}
