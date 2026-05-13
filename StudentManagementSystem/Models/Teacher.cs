using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models;

[Table("Teachers")]
public class Teacher
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TeacherId { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Address { get; set; }

    [EmailAddress]
    [StringLength(320)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? NationalId { get; set; }

    [StringLength(100)]
    public string? Position { get; set; }

    [Required]
    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    // Navigation property
    public ICollection<AttendanceSession> AttendanceSessions { get; set; } = new List<AttendanceSession>();
}
