using System.ComponentModel.DataAnnotations;
using TaskFlow.Models;

namespace TaskFlow.ViewModels
{
    // Used by both Create and Edit team member forms
    public class UserFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        public User ToEntity() => new()
        {
            Id = Id,
            FullName = FullName.Trim(),
            Email = Email.Trim().ToLowerInvariant(),
            PasswordHash = "n/a" // Authentication is out of scope for this project
        };

        public static UserFormViewModel FromEntity(User u) => new()
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email
        };
    }
}