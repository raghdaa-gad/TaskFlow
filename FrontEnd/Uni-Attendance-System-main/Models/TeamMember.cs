using System.ComponentModel.DataAnnotations;

namespace Uni_Attendance.Models;

public class TeamMember
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Enter a valid phone number.")]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [StringLength(30, ErrorMessage = "Member ID cannot exceed 30 characters.")]
    [Display(Name = "Member ID")]
    public string? MemberCode { get; set; }

    [Required(ErrorMessage = "Please select a role.")]
    public string Role { get; set; } = "Member";

    [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters.")]
    public string? Department { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Joining Date")]
    public DateTime? JoiningDate { get; set; }

    [Display(Name = "Account Status")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Assigned Projects")]
    public List<int> ProjectIds { get; set; } = new();
}
