namespace TaskFlow.Models
{
    public class ProjectMember
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Role { get; set; } = "Member"; // Admin, Member, etc.
    }
}