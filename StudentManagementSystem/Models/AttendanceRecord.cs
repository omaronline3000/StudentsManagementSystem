using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models;

[Table("AttendanceRecords")]
public class AttendanceRecord
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RecordId { get; set; }

    [Required]
    public int SessionId { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public bool IsPresent { get; set; } = false;

    // Foreign keys and navigation properties
    [ForeignKey(nameof(SessionId))]
    public AttendanceSession AttendanceSession { get; set; } = null!;

    [ForeignKey(nameof(StudentId))]
    public Student Student { get; set; } = null!;
}
