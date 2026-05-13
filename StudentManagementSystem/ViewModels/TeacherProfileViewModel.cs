using StudentManagementSystem.Models;

namespace StudentManagementSystem.ViewModels;

public class TeacherProfileViewModel
{
    public Teacher Teacher { get; set; } = null!;
    public int TotalSessions { get; set; }
    public int TotalAttendanceRecords { get; set; }
    public IEnumerable<AttendanceSession> RecentSessions { get; set; } = [];
}
