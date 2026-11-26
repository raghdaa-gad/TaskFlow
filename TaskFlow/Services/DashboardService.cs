using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;
using TaskFlow.ViewModels;

namespace TaskFlow.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;
        public DashboardService(AppDbContext context) => _context = context;

        public async Task<DashboardViewModel> GetDashboardAsync(int upcomingDays = 3)
        {
            var today = DateTime.Today;
            var limit = today.AddDays(upcomingDays);

            var counts = await _context.Tasks
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            int Count(TaskStatusEnum s) => counts.FirstOrDefault(c => c.Status == s)?.Count ?? 0;

            var overdue = await _context.Tasks
                .Include(t => t.Project).Include(t => t.AssignedToUser)
                .AsNoTracking()
                .Where(t => t.DueDate < today && t.Status != TaskStatusEnum.Completed)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            var upcoming = await _context.Tasks
                .Include(t => t.Project).Include(t => t.AssignedToUser)
                .AsNoTracking()
                .Where(t => t.DueDate >= today && t.DueDate <= limit && t.Status != TaskStatusEnum.Completed)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            return new DashboardViewModel
            {
                ToDoCount = Count(TaskStatusEnum.ToDo),
                InProgressCount = Count(TaskStatusEnum.InProgress),
                CompletedCount = Count(TaskStatusEnum.Completed),
                TotalTasks = counts.Sum(c => c.Count),
                OverdueTasks = overdue,
                UpcomingTasks = upcoming
            };
        }
    }
}