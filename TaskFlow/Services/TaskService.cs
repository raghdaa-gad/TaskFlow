using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        public TaskService(AppDbContext context) => _context = context;

        public async Task<List<TaskItem>> GetAllAsync() =>
            await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .AsNoTracking()
                .OrderBy(t => t.DueDate)
                .ToListAsync();

        public async Task<TaskItem?> GetByIdAsync(int id) =>
            await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            if (task.DueDate.Date < DateTime.Today)
                throw new BusinessRuleException("Due date cannot be in the past.");

            await EnsureProjectAndUserExistAsync(task.ProjectId, task.AssignedToUserId);

            task.Status = TaskStatusEnum.ToDo; // New tasks always start as ToDo
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> UpdateAsync(TaskItem task)
        {
            var existing = await _context.Tasks.FindAsync(task.Id);
            if (existing == null) return false;

            // Only validate the date if it changed, so old tasks can still be edited
            if (task.DueDate.Date != existing.DueDate.Date && task.DueDate.Date < DateTime.Today)
                throw new BusinessRuleException("Due date cannot be in the past.");

            await EnsureProjectAndUserExistAsync(task.ProjectId, task.AssignedToUserId);

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.Priority = task.Priority;
            existing.DueDate = task.DueDate;
            existing.ProjectId = task.ProjectId;
            existing.AssignedToUserId = task.AssignedToUserId;
            // Status is not changed here; use ChangeStatusAsync instead

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TaskItem>> FilterAsync(TaskStatusEnum? status = null, TaskPriority? priority = null,
                                                      int? projectId = null, int? assignedToUserId = null)
        {
            var query = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .AsNoTracking()
                .AsQueryable();

            if (status.HasValue) query = query.Where(t => t.Status == status.Value);
            if (priority.HasValue) query = query.Where(t => t.Priority == priority.Value);
            if (projectId.HasValue) query = query.Where(t => t.ProjectId == projectId.Value);
            if (assignedToUserId.HasValue) query = query.Where(t => t.AssignedToUserId == assignedToUserId.Value);

            return await query.OrderBy(t => t.DueDate).ToListAsync();
        }

        // Only ToDo -> InProgress -> Completed; no skipping or going back
        public async Task<bool> ChangeStatusAsync(int taskId, TaskStatusEnum newStatus)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null) return false;

            if (task.Status == newStatus) return true;

            var allowed =
                (task.Status == TaskStatusEnum.ToDo && newStatus == TaskStatusEnum.InProgress) ||
                (task.Status == TaskStatusEnum.InProgress && newStatus == TaskStatusEnum.Completed);

            if (!allowed)
                throw new BusinessRuleException(
                    $"Cannot move task from {task.Status} to {newStatus}. Allowed order: ToDo -> InProgress -> Completed.");

            task.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task EnsureProjectAndUserExistAsync(int projectId, int? userId)
        {
            if (!await _context.Projects.AnyAsync(p => p.Id == projectId))
                throw new BusinessRuleException("Project not found.");

            if (userId.HasValue && !await _context.Users.AnyAsync(u => u.Id == userId.Value))
                throw new BusinessRuleException("Assigned user not found.");
        }
    }
}