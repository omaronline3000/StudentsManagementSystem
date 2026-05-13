using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.ViewModels;

public class UpdateStudentGradeViewModel
{
    public int StudentId { get; set; }
    [Range(0, 4)]
    public decimal? TotalGrade { get; set; }
}
