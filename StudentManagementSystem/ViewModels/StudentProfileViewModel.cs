using StudentManagementSystem.Models;

namespace StudentManagementSystem.ViewModels;

public class StudentProfileViewModel
{
    public Student Student { get; set; } = null!;
    public StudentAttendanceStatsViewModel Statistics { get; set; } = new();
    public IEnumerable<StudentAttendanceRecordViewModel> AttendanceRecords { get; set; } = [];
}
