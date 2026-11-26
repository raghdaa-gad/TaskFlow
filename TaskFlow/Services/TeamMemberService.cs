using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly AppDbContext _context;
        public TeamMemberService(AppDbContext context) => _context = context;

        // ---------- Users ----------
        public async Task<List<User>> GetAllUsersAsync() =>
            await _context.Users.AsNoTracking().OrderBy(u => u.FullName).ToListAsync();

        public async Task<User?> GetUserByIdAsync(int id) =>
            await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User> CreateUserAsync(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                throw new BusinessRuleException("This email is already in use.");

            user.CreatedAt = DateTime.Now;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var existing = await _context.Users.FindAsync(user.Id);
            if (existing == null) return false;

            if (await _context.Users.AnyAsync(u => u.Email == user.Email && u.Id != user.Id))
                throw new BusinessRuleException("This email is already in use.");

            existing.FullName = user.FullName;
            existing.Email = user.Email;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            // FKs to Projects and ProjectMembers are Restrict, so check before deleting
            var hasLinks = await _context.Projects.AnyAsync(p => p.CreatedByUserId == id)
                        || await _context.ProjectMembers.AnyAsync(m => m.UserId == id);
            if (hasLinks)
                throw new BusinessRuleException("Cannot delete this user because they are linked to projects. Remove them from projects first.");

            _context.Users.Remove(user); // Their tasks become unassigned (SetNull)
            await _context.SaveChangesAsync();
            return true;
        }

        // ---------- Project members ----------
        public async Task<List<ProjectMember>> GetMembersByProjectAsync(int projectId) =>
            await _context.ProjectMembers
                .Include(m => m.User)
                .AsNoTracking()
                .Where(m => m.ProjectId == projectId)
                .ToListAsync();

        public async Task<ProjectMember> AddMemberAsync(int projectId, int userId, string role = "Member")
        {
            if (!await _context.Projects.AnyAsync(p => p.Id == projectId))
                throw new BusinessRuleException("Project not found.");
            if (!await _context.Users.AnyAsync(u => u.Id == userId))
                throw new BusinessRuleException("User not found.");
            if (await _context.ProjectMembers.AnyAsync(m => m.ProjectId == projectId && m.UserId == userId))
                throw new BusinessRuleException("This user is already a member of the project.");

            var member = new ProjectMember { ProjectId = projectId, UserId = userId, Role = role };
            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<bool> UpdateMemberRoleAsync(int memberId, string role)
        {
            var member = await _context.ProjectMembers.FindAsync(memberId);
            if (member == null) return false;

            member.Role = role;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveMemberAsync(int memberId)
        {
            var member = await _context.ProjectMembers.FindAsync(memberId);
            if (member == null) return false;

            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TaskItem>> GetTasksByUserAsync(int userId) =>
            await _context.Tasks
                .Include(t => t.Project)
                .AsNoTracking()
                .Where(t => t.AssignedToUserId == userId)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
    }
}