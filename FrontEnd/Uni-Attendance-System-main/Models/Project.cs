using System.ComponentModel.DataAnnotations;

namespace Uni_Attendance.Models;

public class Project : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Project name is required.")]
    [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters.")]
    [Display(Name = "Project Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a status.")]
    [Display(Name = "Project Status")]
    public string Status { get; set; } = "Planning";

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "Deadline is required.")]
    [DataType(DataType.Date)]
    public DateTime? Deadline { get; set; }

    [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100.")]
    public int Progress { get; set; }

    [Required(ErrorMessage = "Please select a priority.")]
    public string Priority { get; set; } = "Medium";

    [Display(Name = "Team Members")]
    public List<int> MemberIds { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && Deadline.HasValue && Deadline < StartDate)
        {
            yield return new ValidationResult(
                "Deadline cannot be earlier than the start date.",
                new[] { nameof(Deadline) });
        }
    }
}
