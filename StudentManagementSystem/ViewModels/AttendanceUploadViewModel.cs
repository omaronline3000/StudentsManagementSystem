using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.ViewModels;

public class AttendanceUploadViewModel
{
    public int TeacherId { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Course Title")]
    public string CourseTitle { get; set; } = null!;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Attendance Date")]
    public DateTime? AttendanceDate { get; set; }

    [Required]
    [Display(Name = "Excel File")]
    public IFormFile ExcelFile { get; set; } = null!;
}
