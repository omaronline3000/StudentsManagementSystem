using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models;

[Table("AttendanceSessions")]
public class AttendanceSession
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SessionId { get; set; }

    [Required]
    public int TeacherId { get; set; }

    [Required]
    [StringLength(200)]
    public string CourseTitle { get; set; } = null!;

    [Required]
    [DataType(DataType.Date)]
    public DateTime AttendanceDate { get; set; }

    [StringLength(260)]
    public string? UploadedFileName { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key and navigation property
    [ForeignKey(nameof(TeacherId))]
    public Teacher Teacher { get; set; } = null!;

    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
}
