using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Uni_Attendance.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Task title is required.")]
    [StringLength(150, ErrorMessage = "Task title cannot exceed 150 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Please select a project.")]
    [Display(Name = "Project")]
    public int? ProjectId { get; set; }

    [Required(ErrorMessage = "Please select an assignee.")]
    [Display(Name = "Assignee")]
    public int? AssigneeId { get; set; }

    [Required(ErrorMessage = "Please select a status.")]
    public string Status { get; set; } = "To Do";

    [Required(ErrorMessage = "Please select a priority.")]
    public string Priority { get; set; } = "Medium";

    [Required(ErrorMessage = "Due date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Due Date")]
    public DateTime? DueDate { get; set; }

    // Display-only values, filled by the controller from the in-memory store.
    [BindNever]
    public string? ProjectName { get; set; }

    [BindNever]
    public string? AssigneeName { get; set; }

    [BindNever]
    public string? AssigneeInitials { get; set; }
}
