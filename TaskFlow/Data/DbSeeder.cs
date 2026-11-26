using Microsoft.EntityFrameworkCore;
using TaskFlow.Models;

namespace TaskFlow.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Run only once: skip if users already exist
            if (await context.Users.AnyAsync()) return;

            var today = DateTime.Today;

            // ----- Users (placeholder password hashes, auth is not part of the project scope) -----
            var users = new List<User>
            {
                new() { FullName = "Ahmed Ehab",  Email = "ahmed@taskflow.com",   PasswordHash = "seed-hash" },
                new() { FullName = "Ahmed Ezzab", Email = "ezzab@taskflow.com",   PasswordHash = "seed-hash" },
                new() { FullName = "Soheila",     Email = "soheila@taskflow.com", PasswordHash = "seed-hash" },
                new() { FullName = "Ragda",       Email = "ragda@taskflow.com",   PasswordHash = "seed-hash" }
            };
            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // ----- Projects -----
            var projects = new List<Project>
            {
                new()
                {
                    Title = "Website Redesign",
                    Description = "Redesign the company website with a modern look.",
                    StartDate = today.AddDays(-20), EndDate = today.AddDays(30),
                    CreatedByUserId = users[0].Id
                },
                new()
                {
                    Title = "Mobile App Launch",
                    Description = "Build and release the first version of the mobile app.",
                    StartDate = today.AddDays(-10), EndDate = today.AddDays(60),
                    CreatedByUserId = users[1].Id
                },
                new()
                {
                    Title = "Internal Documentation",
                    Description = "Write onboarding and process documentation.",
                    StartDate = today, EndDate = today.AddDays(14),
                    CreatedByUserId = users[0].Id
                }
            };
            context.Projects.AddRange(projects);
            await context.SaveChangesAsync();

            // ----- Project members -----
            context.ProjectMembers.AddRange(
                new ProjectMember { ProjectId = projects[0].Id, UserId = users[0].Id, Role = "Admin" },
                new ProjectMember { ProjectId = projects[0].Id, UserId = users[2].Id, Role = "Member" },
                new ProjectMember { ProjectId = projects[0].Id, UserId = users[3].Id, Role = "Member" },
                new ProjectMember { ProjectId = projects[1].Id, UserId = users[1].Id, Role = "Admin" },
                new ProjectMember { ProjectId = projects[1].Id, UserId = users[0].Id, Role = "Member" },
                new ProjectMember { ProjectId = projects[2].Id, UserId = users[0].Id, Role = "Admin" },
                new ProjectMember { ProjectId = projects[2].Id, UserId = users[3].Id, Role = "Member" }
            );

            // ----- Tasks (mix of statuses, priorities, overdue and upcoming) -----
            context.Tasks.AddRange(
                // Website Redesign
                new TaskItem { Title = "Create wireframes",      Description = "Low-fidelity wireframes for all pages.", Priority = TaskPriority.High,   Status = TaskStatusEnum.Completed,  DueDate = today.AddDays(-5), ProjectId = projects[0].Id, AssignedToUserId = users[2].Id },
                new TaskItem { Title = "Design color palette",   Description = "Pick brand colors and fonts.",           Priority = TaskPriority.Medium, Status = TaskStatusEnum.Completed,  DueDate = today.AddDays(-3), ProjectId = projects[0].Id, AssignedToUserId = users[2].Id },
                new TaskItem { Title = "Build landing page",     Description = "Implement the landing page in HTML/CSS.", Priority = TaskPriority.High,   Status = TaskStatusEnum.InProgress, DueDate = today.AddDays(2),  ProjectId = projects[0].Id, AssignedToUserId = users[3].Id },
                new TaskItem { Title = "Fix navbar on mobile",   Description = "Navbar breaks on small screens.",         Priority = TaskPriority.Medium, Status = TaskStatusEnum.ToDo,       DueDate = today.AddDays(-1), ProjectId = projects[0].Id, AssignedToUserId = users[3].Id },
                new TaskItem { Title = "Write page content",     Description = "Final copy for every page.",              Priority = TaskPriority.Low,    Status = TaskStatusEnum.ToDo,       DueDate = today.AddDays(15), ProjectId = projects[0].Id, AssignedToUserId = null },

                // Mobile App Launch
                new TaskItem { Title = "Set up CI/CD pipeline",  Description = "Automated build and deploy.",             Priority = TaskPriority.High,   Status = TaskStatusEnum.InProgress, DueDate = today.AddDays(3),  ProjectId = projects[1].Id, AssignedToUserId = users[1].Id },
                new TaskItem { Title = "Implement login screen", Description = "Login UI and validation.",                Priority = TaskPriority.High,   Status = TaskStatusEnum.ToDo,       DueDate = today.AddDays(10), ProjectId = projects[1].Id, AssignedToUserId = users[0].Id },
                new TaskItem { Title = "Prepare store listing",  Description = "Screenshots and description.",            Priority = TaskPriority.Low,    Status = TaskStatusEnum.ToDo,       DueDate = today.AddDays(45), ProjectId = projects[1].Id, AssignedToUserId = null },

                // Internal Documentation
                new TaskItem { Title = "Write onboarding guide", Description = "Steps for new team members.",            Priority = TaskPriority.Medium, Status = TaskStatusEnum.InProgress, DueDate = today.AddDays(5),  ProjectId = projects[2].Id, AssignedToUserId = users[3].Id },
                new TaskItem { Title = "Document release process", Description = "How we ship to production.",           Priority = TaskPriority.Low,    Status = TaskStatusEnum.ToDo,       DueDate = today.AddDays(12), ProjectId = projects[2].Id, AssignedToUserId = users[0].Id }
            );

            await context.SaveChangesAsync();
        }
    }
}