using TaskFlow.Models;

namespace TaskFlow.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTasks { get; set; }
        public int ToDoCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
        public List<TaskItem> OverdueTasks { get; set; } = new();
        public List<TaskItem> UpcomingTasks { get; set; } = new();
    }
}