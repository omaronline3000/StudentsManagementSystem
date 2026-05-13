using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(50)]
    [Display(Name = "National ID")]
    public string NationalId { get; set; } = null!;
}
