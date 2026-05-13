namespace StudentManagementSystem.ViewModels;

public class StudentAttendanceRecordViewModel
{
    public int RecordId { get; set; }
    public int SessionId { get; set; }
    public int StudentId { get; set; }
    public string CourseTitle { get; set; } = null!;
    public DateTime AttendanceDate { get; set; }
    public string TeacherName { get; set; } = null!;
    public bool IsPresent { get; set; }
    public DateTime SessionCreatedAt { get; set; }
}
