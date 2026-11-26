using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskFlow.Models;

namespace TaskFlow.ViewModels
{
    // Used by both Create and Edit task forms
    public class TaskFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority is required.")]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

        [Required(ErrorMessage = "Please select a project.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a project.")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Display(Name = "Assigned To")]
        public int? AssignedToUserId { get; set; }

        // Dropdown data, filled by the controller (not posted back)
        public IEnumerable<SelectListItem> Projects { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();

        public TaskItem ToEntity() => new()
        {
            Id = Id,
            Title = Title.Trim(),
            Description = Description?.Trim() ?? string.Empty,
            Priority = Priority,
            DueDate = DueDate,
            ProjectId = ProjectId,
            AssignedToUserId = AssignedToUserId
        };

        public static TaskFormViewModel FromEntity(TaskItem t) => new()
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority,
            DueDate = t.DueDate,
            ProjectId = t.ProjectId,
            AssignedToUserId = t.AssignedToUserId
        };
    }
}