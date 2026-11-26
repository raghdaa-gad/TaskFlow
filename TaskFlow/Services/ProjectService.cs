using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        public ProjectService(AppDbContext context) => _context = context;

        public async Task<List<Project>> GetAllAsync() =>
            await _context.Projects
                .Include(p => p.Tasks)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

        public async Task<Project?> GetByIdAsync(int id) =>
            await _context.Projects
                .Include(p => p.CreatedByUser)
                .Include(p => p.ProjectMembers).ThenInclude(m => m.User)
                .Include(p => p.Tasks).ThenInclude(t => t.AssignedToUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Project> CreateAsync(Project project)
        {
            Validate(project);
            project.CreatedAt = DateTime.Now;
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<bool> UpdateAsync(Project project)
        {
            var existing = await _context.Projects.FindAsync(project.Id);
            if (existing == null) return false;

            Validate(project);
            existing.Title = project.Title;
            existing.Description = project.Description;
            existing.StartDate = project.StartDate;
            existing.EndDate = project.EndDate;

            await _context.SaveChangesAsync();
            return true;
        }

        // Tasks and ProjectMembers are deleted automatically (database cascade)
        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetCompletionPercentageAsync(int projectId)
        {
            var total = await _context.Tasks.CountAsync(t => t.ProjectId == projectId);
            if (total == 0) return 0;

            var done = await _context.Tasks.CountAsync(t =>
                t.ProjectId == projectId && t.Status == TaskStatusEnum.Completed);

            return Math.Round(done * 100.0 / total, 1);
        }

        private static void Validate(Project p)
        {
            if (p.EndDate.Date < p.StartDate.Date)
                throw new BusinessRuleException("End date must be after the start date.");
        }
    }
}