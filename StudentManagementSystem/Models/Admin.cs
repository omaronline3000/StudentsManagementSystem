using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models;

[Table("Admins")]
public class Admin
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AdminId { get; set; }

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
}
