namespace Uni_Attendance.Models;

/// <summary>
/// Small in-memory store used until a real database is connected.
/// Registered as a singleton so edits survive requests during the demo.
/// </summary>
public class AppDataStore
{
    private readonly object _gate = new();
    private int _nextProjectId;
    private int _nextMemberId;
    private int _nextTaskId;
    private int _nextAttendanceId;

    public static readonly string[] ProjectStatuses =
        { "Planning", "In Progress", "Completed", "On Hold" };

    public static readonly string[] TaskStatuses =
        { "To Do", "In Progress", "In Review", "Done" };

    public static readonly string[] Priorities = { "Low", "Medium", "High" };

    public static readonly string[] Roles =
        { "Team Leader", "Admin", "Developer", "Designer", "Member" };

    // Demo student directory. A real backend would replace this with a database lookup.
    private static readonly Dictionary<string, string> Students = new()
    {
        ["2026001"] = "Student Demo",
        ["2023001"] = "Ahmed Mohamed",
        ["2023002"] = "Sarah Ali",
        ["2023003"] = "Omar Hassan",
        ["2023004"] = "Mariam Adel",
        ["2023005"] = "Khaled Nabil"
    };

    public AppDataStore()
    {
        Projects.AddRange(new List<Project>
        {
            new()
            {
                Id = 1,
                Name = "University Attendance System",
                Status = "In Progress",
                Description = "QR-based student attendance management system.",
                StartDate = new DateTime(2026, 10, 1),
                Deadline = new DateTime(2026, 12, 30),
                Progress = 65,
                Priority = "High",
                MemberIds = new List<int> { 1, 2, 3, 4 }
            },
            new()
            {
                Id = 2,
                Name = "Medical Management System",
                Status = "Completed",
                Description = "A platform for organizing medical information and patient records.",
                StartDate = new DateTime(2026, 8, 15),
                Deadline = new DateTime(2026, 10, 20),
                Progress = 100,
                Priority = "Medium",
                MemberIds = new List<int> { 2, 5 }
            },
            new()
            {
                Id = 3,
                Name = "Student Portal",
                Status = "Planning",
                Description = "A portal for student services and academic updates.",
                StartDate = new DateTime(2026, 11, 10),
                Deadline = new DateTime(2027, 2, 28),
                Progress = 25,
                Priority = "Medium",
                MemberIds = new List<int> { 1, 3 }
            },
            new()
            {
                Id = 4,
                Name = "Library Management System",
                Status = "On Hold",
                Description = "Catalog and borrowing workflow for the university library.",
                StartDate = new DateTime(2026, 9, 1),
                Deadline = new DateTime(2027, 1, 31),
                Progress = 10,
                Priority = "Low",
                MemberIds = new List<int> { 4, 6 }
            },
            new()
            {
                Id = 5,
                Name = "Graduation Project Tracking",
                Status = "In Progress",
                Description = "Track graduation project proposals, milestones and grading.",
                StartDate = new DateTime(2026, 10, 20),
                Deadline = new DateTime(2027, 3, 15),
                Progress = 40,
                Priority = "High",
                MemberIds = new List<int> { 1, 5, 6 }
            }
        });

        Members.AddRange(new List<TeamMember>
        {
            new()
            {
                Id = 1,
                FullName = "Suhaila Ahmed",
                Email = "suhaila@example.com",
                Phone = "+20 100 000 0001",
                MemberCode = "TM-001",
                Role = "Team Leader",
                Department = "Computer Science",
                JoiningDate = new DateTime(2026, 10, 1),
                IsActive = true,
                ProjectIds = new List<int> { 1, 3, 5 }
            },
            new()
            {
                Id = 2,
                FullName = "Ahmed Mohamed",
                Email = "ahmed@example.com",
                Phone = "+20 100 000 0002",
                MemberCode = "TM-002",
                Role = "Developer",
                Department = "Artificial Intelligence",
                JoiningDate = new DateTime(2026, 10, 5),
                IsActive = true,
                ProjectIds = new List<int> { 1, 2 }
            },
            new()
            {
                Id = 3,
                FullName = "Mohamed Khaled",
                Email = "mohamed@example.com",
                Phone = "+20 100 000 0003",
                MemberCode = "TM-003",
                Role = "Designer",
                Department = "Information Technology",
                JoiningDate = new DateTime(2026, 10, 8),
                IsActive = true,
                ProjectIds = new List<int> { 1, 3 }
            },
            new()
            {
                Id = 4,
                FullName = "Omar Hassan",
                Email = "omar@example.com",
                Phone = "+20 100 000 0004",
                MemberCode = "TM-004",
                Role = "Developer",
                Department = "Computer Science",
                JoiningDate = new DateTime(2026, 10, 12),
                IsActive = false,
                ProjectIds = new List<int> { 1, 4 }
            },
            new()
            {
                Id = 5,
                FullName = "Sara Ali",
                Email = "sara@example.com",
                Phone = "+20 100 000 0005",
                MemberCode = "TM-005",
                Role = "Designer",
                Department = "Information Technology",
                JoiningDate = new DateTime(2026, 10, 18),
                IsActive = true,
                ProjectIds = new List<int> { 2, 5 }
            },
            new()
            {
                Id = 6,
                FullName = "Khaled Nabil",
                Email = "khaled@example.com",
                Phone = "+20 100 000 0006",
                MemberCode = "TM-006",
                Role = "Member",
                Department = "Artificial Intelligence",
                JoiningDate = new DateTime(2026, 11, 2),
                IsActive = true,
                ProjectIds = new List<int> { 4, 5 }
            }
        });

        Tasks.AddRange(new List<TaskItem>
        {
            new() { Id = 1, Title = "Design Login Page", Description = "Create the login interface and validation design.", ProjectId = 1, AssigneeId = 3, Status = "To Do", Priority = "High", DueDate = new DateTime(2026, 12, 5) },
            new() { Id = 2, Title = "Database Design", Description = "Prepare the database schema and relationships.", ProjectId = 1, AssigneeId = 2, Status = "To Do", Priority = "Medium", DueDate = new DateTime(2026, 12, 8) },
            new() { Id = 3, Title = "Prepare Documentation", Description = "Write the initial project documentation.", ProjectId = 3, AssigneeId = 6, Status = "To Do", Priority = "Low", DueDate = new DateTime(2026, 12, 12) },
            new() { Id = 4, Title = "Develop Dashboard", Description = "Build the main dashboard and statistics cards.", ProjectId = 1, AssigneeId = 1, Status = "In Progress", Priority = "High", DueDate = new DateTime(2026, 12, 10) },
            new() { Id = 5, Title = "Student Management", Description = "Implement student information and management pages.", ProjectId = 3, AssigneeId = 4, Status = "In Progress", Priority = "Medium", DueDate = new DateTime(2026, 12, 14) },
            new() { Id = 6, Title = "Attendance Records", Description = "Review attendance records and filtering features.", ProjectId = 1, AssigneeId = 2, Status = "In Review", Priority = "Medium", DueDate = new DateTime(2026, 12, 16) },
            new() { Id = 7, Title = "Project Setup", Description = "Initialize the project structure and environment.", ProjectId = 1, AssigneeId = 1, Status = "Done", Priority = "Low", DueDate = new DateTime(2026, 11, 20) },
            new() { Id = 8, Title = "Requirements Analysis", Description = "Collect and organize system requirements.", ProjectId = 5, AssigneeId = 6, Status = "Done", Priority = "Low", DueDate = new DateTime(2026, 11, 25) },
            new() { Id = 9, Title = "Patient Records Module", Description = "Build the patient records module for the medical system.", ProjectId = 2, AssigneeId = 5, Status = "Done", Priority = "Medium", DueDate = new DateTime(2026, 10, 18) }
        });

        AttendanceRecords.AddRange(new List<AttendanceRecord>
        {
            new() { Id = 1, StudentId = "2023001", StudentName = Students["2023001"], Timestamp = DateTime.Today.AddHours(8).AddMinutes(15), Status = "Present" },
            new() { Id = 2, StudentId = "2023002", StudentName = Students["2023002"], Timestamp = DateTime.Today.AddHours(8).AddMinutes(35), Status = "Late" },
            new() { Id = 3, StudentId = "2023004", StudentName = Students["2023004"], Timestamp = DateTime.Today.AddHours(8).AddMinutes(5), Status = "Present" }
        });

        _nextProjectId = Projects.Max(p => p.Id) + 1;
        _nextMemberId = Members.Max(m => m.Id) + 1;
        _nextTaskId = Tasks.Max(t => t.Id) + 1;
        _nextAttendanceId = AttendanceRecords.Max(r => r.Id) + 1;
    }

    public List<Project> Projects { get; } = new();
    public List<TeamMember> Members { get; } = new();
    public List<TaskItem> Tasks { get; } = new();
    public List<AttendanceRecord> AttendanceRecords { get; } = new();

    public IReadOnlyList<Project> GetProjects()
    {
        lock (_gate) return Projects.OrderBy(p => p.Id).ToList();
    }

    public IReadOnlyList<TeamMember> GetMembers()
    {
        lock (_gate) return Members.OrderBy(m => m.Id).ToList();
    }

    public IReadOnlyList<TaskItem> GetTasks()
    {
        lock (_gate) return Tasks.OrderBy(t => t.Id).ToList();
    }

    public IReadOnlyList<AttendanceRecord> GetAttendanceRecords()
    {
        lock (_gate)
            return AttendanceRecords
                .OrderByDescending(r => r.Timestamp)
                .ThenByDescending(r => r.Id)
                .ToList();
    }

    public int GetStudentCount() => Students.Count;

    public CheckInResult CheckIn(string studentId, DateTime scannedAt)
    {
        lock (_gate)
        {
            if (!Students.TryGetValue(studentId, out var studentName))
                return new CheckInResult
                {
                    Success = false,
                    Message = "Unknown student ID: " + studentId
                };

            if (AttendanceRecords.Any(r =>
                    r.StudentId == studentId && r.Timestamp.Date == scannedAt.Date))
            {
                return new CheckInResult
                {
                    Success = false,
                    Message = studentName + " is already checked in today."
                };
            }

            var status = scannedAt.TimeOfDay > new TimeSpan(9, 0, 0) ? "Late" : "Present";
            var record = new AttendanceRecord
            {
                Id = _nextAttendanceId++,
                StudentId = studentId,
                StudentName = studentName,
                Timestamp = scannedAt,
                Status = status,
                Source = "QR"
            };

            AttendanceRecords.Add(record);

            return new CheckInResult
            {
                Success = true,
                Message = "Attendance recorded for " + studentName + ".",
                Record = record
            };
        }
    }

    public Project? FindProject(int id)
    {
        lock (_gate) return Projects.FirstOrDefault(p => p.Id == id);
    }

    public TeamMember? FindMember(int id)
    {
        lock (_gate) return Members.FirstOrDefault(m => m.Id == id);
    }

    public TaskItem? FindTask(int id)
    {
        lock (_gate) return Tasks.FirstOrDefault(t => t.Id == id);
    }

    public Project AddProject(Project project)
    {
        lock (_gate)
        {
            project.Id = _nextProjectId++;
            project.MemberIds ??= new List<int>();
            Projects.Add(project);
        }

        return project;
    }

    public TeamMember AddMember(TeamMember member)
    {
        lock (_gate)
        {
            member.Id = _nextMemberId++;
            member.ProjectIds ??= new List<int>();
            Members.Add(member);
        }

        return member;
    }

    public TaskItem AddTask(TaskItem task)
    {
        lock (_gate)
        {
            task.Id = _nextTaskId++;
            Tasks.Add(task);
        }

        return task;
    }

    public bool UpdateProject(Project source)
    {
        var project = FindProject(source.Id);
        if (project is null) return false;

        project.Name = source.Name;
        project.Status = source.Status;
        project.Description = source.Description;
        project.StartDate = source.StartDate;
        project.Deadline = source.Deadline;
        project.Progress = source.Progress;
        project.Priority = source.Priority;
        project.MemberIds = source.MemberIds ?? new List<int>();
        return true;
    }

    public bool UpdateMember(TeamMember source)
    {
        var member = FindMember(source.Id);
        if (member is null) return false;

        member.FullName = source.FullName;
        member.Email = source.Email;
        member.Phone = source.Phone;
        member.MemberCode = source.MemberCode;
        member.Role = source.Role;
        member.Department = source.Department;
        member.JoiningDate = source.JoiningDate;
        member.IsActive = source.IsActive;
        member.ProjectIds = source.ProjectIds ?? new List<int>();
        return true;
    }

    public bool UpdateTask(TaskItem source)
    {
        var task = FindTask(source.Id);
        if (task is null) return false;

        task.Title = source.Title;
        task.Description = source.Description;
        task.ProjectId = source.ProjectId;
        task.AssigneeId = source.AssigneeId;
        task.Status = source.Status;
        task.Priority = source.Priority;
        task.DueDate = source.DueDate;
        return true;
    }

    public bool DeleteTask(int id)
    {
        lock (_gate)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == id);
            if (task is null) return false;

            Tasks.Remove(task);
            return true;
        }
    }

    public bool DeleteProject(int id)
    {
        lock (_gate)
        {
            var project = Projects.FirstOrDefault(p => p.Id == id);
            if (project is null) return false;

            Projects.Remove(project);

            // Keep links consistent: unlink members and remove tasks of this project.
            foreach (var member in Members)
                member.ProjectIds.Remove(id);

            Tasks.RemoveAll(t => t.ProjectId == id);
            return true;
        }
    }

    public bool DeleteMember(int id)
    {
        lock (_gate)
        {
            var member = Members.FirstOrDefault(m => m.Id == id);
            if (member is null) return false;

            Members.Remove(member);

            // Keep links consistent: unlink projects and unassign tasks.
            foreach (var project in Projects)
                project.MemberIds.Remove(id);

            foreach (var task in Tasks.Where(t => t.AssigneeId == id))
                task.AssigneeId = 0;

            return true;
        }
    }

    public bool MoveTask(int id, string status)
    {
        if (!TaskStatuses.Contains(status)) return false;

        var task = FindTask(id);
        if (task is null) return false;

        task.Status = status;
        return true;
    }
}
