using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Key for Creator
        public int? CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        // Navigation Properties
        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}