namespace StudentManagementSystem.ViewModels;

public class StudentAttendanceStatsViewModel
{
    public int StudentId { get; set; }
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public decimal AttendancePercentage { get; set; }
}
