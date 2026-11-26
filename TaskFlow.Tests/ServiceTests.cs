using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;
using TaskFlow.Services;
using Xunit;

namespace TaskFlow.Tests
{
    public class ServiceTests
    {
        // A fresh in-memory database for every test
        private static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private static async Task<Project> AddProjectAsync(AppDbContext ctx)
        {
            var project = new Project
            {
                Title = "Test Project",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(10)
            };
            ctx.Projects.Add(project);
            await ctx.SaveChangesAsync();
            return project;
        }

        private static async Task<TaskItem> AddTaskAsync(
            AppDbContext ctx, int projectId, TaskStatusEnum status, int dueInDays = 5)
        {
            var task = new TaskItem
            {
                Title = "Test Task",
                ProjectId = projectId,
                Status = status,
                DueDate = DateTime.Today.AddDays(dueInDays)
            };
            ctx.Tasks.Add(task);
            await ctx.SaveChangesAsync();
            return task;
        }

        // ---------- TaskService: status transitions ----------

        [Fact]
        public async Task ChangeStatus_ToDoToInProgress_Succeeds()
        {
            using var ctx = CreateContext();
            var project = await AddProjectAsync(ctx);
            var task = await AddTaskAsync(ctx, project.Id, TaskStatusEnum.ToDo);
            var service = new TaskService(ctx);

            var result = await service.ChangeStatusAsync(task.Id, TaskStatusEnum.InProgress);

            Assert.True(result);
            Assert.Equal(TaskStatusEnum.InProgress, (await ctx.Tasks.FindAsync(task.Id))!.Status);
        }

        [Fact]
        public async Task ChangeStatus_InProgressToCompleted_Succeeds()
        {
            using var ctx = CreateContext();
            var project = await AddProjectAsync(ctx);
            var task = await AddTaskAsync(ctx, project.Id, TaskStatusEnum.InProgress);
            var service = new TaskService(ctx);

            var result = await service.ChangeStatusAsync(task.Id, TaskStatusEnum.Completed);

            Assert.True(result);
            Assert.Equal(TaskStatusEnum.Completed, (await ctx.Tasks.FindAsync(task.Id))!.Status);
        }

        [Fact]
        public async Task ChangeStatus_SkippingInProgress_Throws()
        {
            using var ctx = CreateContext();
            var project = await AddProjectAsync(ctx);
            var task = await AddTaskAsync(ctx, project.Id, TaskStatusEnum.ToDo);
            var service = new TaskService(ctx);

            await Assert.ThrowsAsync<BusinessRuleException>(
                () => service.ChangeStatusAsync(task.Id, TaskStatusEnum.Completed));
        }

        [Fact]
        public async Task ChangeStatus_GoingBackwards_Throws()
        {
            using var ctx = CreateContext();
            var project = await AddProjectAsync(ctx);
            var task = await AddTaskAsync(ctx, project.Id, TaskStatusEnum.Completed);
            var service = new TaskService(ctx);

            await Assert.ThrowsAsync<BusinessRuleException>(
                () => service.ChangeStatusAsync(task.Id, TaskStatusEnum.ToDo));
        }

        [Fact]
        public async Task ChangeStatus_UnknownTask_ReturnsFalse()
        {
            using var ctx = CreateContext();
            var service = new TaskService(ctx);

            var result = await service.ChangeStatusAsync(999, TaskStatusEnum.InProgress);

            Assert.False(result);
        }

        // ---------- TaskService: due date rule ----------

        [Fact]
        public async Task CreateTask_DueDateInThePast_Throws()
        {
            using var ctx = CreateContext();
            var project = await AddProjectAsync(ctx);
            var service = new TaskService(ctx);

            var task = new TaskItem
            {
                Title = "Late task",
                ProjectId = project.Id,
                DueDate = DateTime.Today.AddDays(-1)
            };

            await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(task));
        }

        [Fact]
        public async Task CreateTask_DueDateToday_Succeeds_AndStartsAsToDo()
        {
            using var ctx = CreateContext();
            var project = await AddProjectAsync(ctx);
            var service = new TaskService(ctx);

            var task = new TaskItem
            {
                Title = "Today task",
                ProjectId = project.Id,
                DueDate = DateTime.Today,
                Status = TaskStatusEnum.Completed // must be ignored
            };

            var created = await service.CreateAsync(task);

            Assert.True(created.Id > 0);
            Assert.Equal(TaskStatusEnum.ToDo, created.Status);
        }

        [Fact]
        public async Task CreateTask_UnknownProject_Throws()
        {
            using var ctx = CreateContext();
            var service = new TaskService(ctx);

            var task = new TaskItem { Title = "Orphan", ProjectId = 123, DueDate = DateTime.Today.AddDays(1) };

            await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(task));
        }

        // ---------- TaskService: filtering ----------

        [Fact]
        public async Task Filter_ByStatus_ReturnsOnlyMatchingTasks()
        {
            using var ctx = CreateContext();
            var project = await AddProjectAsync(ctx);
            await AddTaskAsync(ctx, project.Id, TaskStatusEnum.ToDo);
            await AddTaskAsync(ctx, project.Id, TaskStatusEnum.InProgress);
            await AddTaskAsync(ctx, project.Id, TaskStatusEnum.InProgress);
            var service = new TaskService(ctx);

            var result = await service.FilterAsync(status: TaskStatusEnum.InProgress);

            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal(TaskStatusEnum.InProgress, t.Status));
        }

        // ---------- ProjectService ----------

        [Fact]
        public async Task CompletionPercentage_OneOfFourDone_Is25()
        {
            using var ctx = CreateContext();
            var project = await AddProjectAsync(ctx);
            await AddTaskAsync(ctx, project.Id, TaskStatusEnum.Completed);
            await AddTaskAsync(ctx, project.Id, TaskStatusEnum.ToDo);
            await AddTaskAsync(ctx, project.Id, TaskStatusEnum.ToDo);
            await AddTaskAsync(ctx, project.Id, TaskStatusEnum.InProgress);
            var service = new ProjectService(ctx);

            var percentage = await service.GetCompletionPercentageAsync(project.Id);

            Assert.Equal(25, percentage);
        }

        [Fact]
        public async Task CompletionPercentage_NoTasks_IsZero()
        {
            using var ctx = CreateContext();
            var project = await AddProjectAsync(ctx);
            var service = new ProjectService(ctx);

            var percentage = await service.GetCompletionPercentageAsync(project.Id);

            Assert.Equal(0, percentage);
        }

        [Fact]
        public async Task CreateProject_EndBeforeStart_Throws()
        {
            using var ctx = CreateContext();
            var service = new ProjectService(ctx);

            var project = new Project
            {
                Title = "Bad dates",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(-1)
            };

            await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(project));
        }

        // ---------- DashboardService ----------

        [Fact]
        public async Task Dashboard_CountsStatuses_AndFindsOverdueAndUpcoming()
        {
            using var ctx = CreateContext();
            var project = await AddProjectAsync(ctx);

            await AddTaskAsync(ctx, project.Id, TaskStatusEnum.ToDo, dueInDays: -2);        // overdue
            await AddTaskAsync(ctx, project.Id, TaskStatusEnum.Completed, dueInDays: -2);   // late but done, not overdue
            await AddTaskAsync(ctx, project.Id, TaskStatusEnum.InProgress, dueInDays: 2);   // upcoming
            await AddTaskAsync(ctx, project.Id, TaskStatusEnum.ToDo, dueInDays: 20);        // far away
            var service = new DashboardService(ctx);

            var dashboard = await service.GetDashboardAsync(upcomingDays: 3);

            Assert.Equal(4, dashboard.TotalTasks);
            Assert.Equal(2, dashboard.ToDoCount);
            Assert.Equal(1, dashboard.InProgressCount);
            Assert.Equal(1, dashboard.CompletedCount);
            Assert.Single(dashboard.OverdueTasks);
            Assert.Single(dashboard.UpcomingTasks);
        }
    }
}