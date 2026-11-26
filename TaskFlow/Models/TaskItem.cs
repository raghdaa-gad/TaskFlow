using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

    public enum TaskStatusEnum
    {
        ToDo,
        InProgress,
        Completed
    }

    public class TaskItem
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public TaskPriority Priority { get; set; }
        public TaskStatusEnum Status { get; set; }

        public DateTime DueDate { get; set; }

        // Foreign Keys
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int? AssignedToUserId { get; set; }
        public User? AssignedToUser { get; set; }
    }
}