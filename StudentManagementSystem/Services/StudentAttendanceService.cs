using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Services;

public class StudentAttendanceService
{
    private readonly StudentManagementDbContext _context;

    public StudentAttendanceService(StudentManagementDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all attendance records for a specific student.
    /// Joins AttendanceRecords with AttendanceSessions and Teachers.
    /// Excludes deleted students and teachers.
    /// </summary>
    public async Task<IEnumerable<StudentAttendanceRecordViewModel>> GetStudentAttendanceAsync(int studentId)
    {
        var studentExists = await _context.Students
            .AnyAsync(s => s.StudentId == studentId && !s.IsDeleted);

        if (!studentExists)
            return Enumerable.Empty<StudentAttendanceRecordViewModel>();

        var attendanceRecords = await _context.AttendanceRecords
            .Where(ar => ar.StudentId == studentId)
            .Join(
                _context.AttendanceSessions,
                ar => ar.SessionId,
                s => s.SessionId,
                (ar, s) => new { AttendanceRecord = ar, Session = s }
            )
            .Join(
                _context.Teachers.Where(t => !t.IsDeleted),
                combined => combined.Session.TeacherId,
                t => t.TeacherId,
                (combined, teacher) => new StudentAttendanceRecordViewModel
                {
                    RecordId = combined.AttendanceRecord.RecordId,
                    SessionId = combined.AttendanceRecord.SessionId,
                    StudentId = combined.AttendanceRecord.StudentId,
                    CourseTitle = combined.Session.CourseTitle,
                    AttendanceDate = combined.Session.AttendanceDate,
                    TeacherName = teacher.Name,
                    IsPresent = combined.AttendanceRecord.IsPresent,
                    SessionCreatedAt = combined.Session.CreatedAt
                }
            )
            .OrderByDescending(ar => ar.AttendanceDate)
            .ToListAsync();

        return attendanceRecords;
    }

    /// <summary>
    /// Retrieves attendance statistics for a student.
    /// </summary>
    public async Task<StudentAttendanceStatsViewModel?> GetStudentAttendanceStatsAsync(int studentId)
    {
        var studentExists = await _context.Students
            .AnyAsync(s => s.StudentId == studentId && !s.IsDeleted);

        if (!studentExists)
            return null;

        var records = await _context.AttendanceRecords
            .Where(ar => ar.StudentId == studentId)
            .ToListAsync();

        var totalSessions = records.Count;
        var presentCount = records.Count(r => r.IsPresent);
        var absentCount = records.Count(r => !r.IsPresent);
        var attendancePercentage = totalSessions > 0
            ? Math.Round((presentCount * 100.0m) / totalSessions, 2)
            : 0;

        return new StudentAttendanceStatsViewModel
        {
            StudentId = studentId,
            TotalSessions = totalSessions,
            PresentCount = presentCount,
            AbsentCount = absentCount,
            AttendancePercentage = attendancePercentage
        };
    }
}
