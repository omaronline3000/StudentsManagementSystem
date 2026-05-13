using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StudentManagementSystem.Models;

[Table("Students")]
public class Student
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StudentId { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [EmailAddress]
    [StringLength(320)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? NationalId { get; set; }

    [Precision(5, 2)]
    [Range(0, 4)]
    public decimal? TotalGrade { get; set; }

    [Required]
    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    // Navigation property
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
}
