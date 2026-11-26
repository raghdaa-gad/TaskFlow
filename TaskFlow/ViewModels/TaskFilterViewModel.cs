using Microsoft.AspNetCore.Mvc.Rendering;
using TaskFlow.Models;

namespace TaskFlow.ViewModels
{
    // Bound from the query string on the Tasks list page, e.g. /Tasks?status=InProgress&projectId=2
    public class TaskFilterViewModel
    {
        public TaskStatusEnum? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public int? ProjectId { get; set; }
        public int? AssignedToUserId { get; set; }

        // Results and dropdown data, filled by the controller
        public List<TaskItem> Tasks { get; set; } = new();
        public IEnumerable<SelectListItem> Projects { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    }
}