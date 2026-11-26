namespace Uni_Attendance.Models;

public class DashboardViewModel
{
    public int TotalProjects { get; set; }
    public int InProgressProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int TotalMembers { get; set; }
    public int OpenTasks { get; set; }
    public int OverdueTasks { get; set; }

    public IReadOnlyList<Project> RecentProjects { get; set; } = new List<Project>();

    public List<string> StatusLabels { get; set; } = new();
    public List<int> StatusCounts { get; set; } = new();
    public List<string> PriorityLabels { get; set; } = new();
    public List<int> PriorityCounts { get; set; } = new();
}
