using System.ComponentModel.DataAnnotations;
using TaskFlow.Models;

namespace TaskFlow.ViewModels
{
    // Used by both Create and Edit project forms
    public class ProjectFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(30);

        public Project ToEntity() => new()
        {
            Id = Id,
            Title = Title.Trim(),
            Description = Description?.Trim() ?? string.Empty,
            StartDate = StartDate,
            EndDate = EndDate
        };

        public static ProjectFormViewModel FromEntity(Project p) => new()
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            StartDate = p.StartDate,
            EndDate = p.EndDate
        };
    }
}