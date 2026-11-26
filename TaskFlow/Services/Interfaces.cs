using TaskFlow.Models;
using TaskFlow.ViewModels;

namespace TaskFlow.Services
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllAsync();
        Task<Project?> GetByIdAsync(int id);
        Task<Project> CreateAsync(Project project);
        Task<bool> UpdateAsync(Project project);
        Task<bool> DeleteAsync(int id);
        Task<double> GetCompletionPercentageAsync(int projectId);
    }

    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(int id);
        Task<TaskItem> CreateAsync(TaskItem task);
        Task<bool> UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(int id);
        Task<List<TaskItem>> FilterAsync(TaskStatusEnum? status = null, TaskPriority? priority = null,
            int? projectId = null, int? assignedToUserId = null);
        Task<bool> ChangeStatusAsync(int taskId, TaskStatusEnum newStatus);
    }

    public interface ITeamMemberService
    {
        // Users
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);

        // Project members
        Task<List<ProjectMember>> GetMembersByProjectAsync(int projectId);
        Task<ProjectMember> AddMemberAsync(int projectId, int userId, string role = "Member");
        Task<bool> UpdateMemberRoleAsync(int memberId, string role);
        Task<bool> RemoveMemberAsync(int memberId);

        // Tasks assigned to a user
        Task<List<TaskItem>> GetTasksByUserAsync(int userId);
    }

    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardAsync(int upcomingDays = 3);
    }
}