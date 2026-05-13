using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;

namespace StudentManagementSystem.Services;

public class AdminGradeService
{
    private readonly StudentManagementDbContext _context;

    public AdminGradeService(StudentManagementDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Updates a student's total grade.
    /// Validates grade is within acceptable range (0-100).
    /// </summary>
    public async Task<(bool Success, string Message, decimal? UpdatedGrade)> UpdateStudentGradeAsync(int studentId, decimal? totalGrade)
    {
        // Validate grade range
        if (totalGrade.HasValue)
        {
            if (totalGrade < 0 || totalGrade > 100)
                return (false, "Grade must be between 0 and 100.", null);
        }

        // Find student (exclude deleted)
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted);

        if (student == null)
            return (false, "Student not found or has been deleted.", null);

        // Update grade
        student.TotalGrade = totalGrade;
        _context.Students.Update(student);

        try
        {
            await _context.SaveChangesAsync();
            return (true, "Student grade updated successfully.", totalGrade);
        }
        catch (Exception ex)
        {
            return (false, $"An error occurred while updating the grade: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Retrieves a student's current grade and attendance summary.
    /// </summary>
    public async Task<dynamic> GetStudentGradeAndAttendanceAsync(int studentId)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted);

        if (student == null)
            return null;

        // Get attendance statistics
        var attendanceRecords = await _context.AttendanceRecords
            .Where(ar => ar.StudentId == studentId)
            .ToListAsync();

        var totalSessions = attendanceRecords.Count;
        var presentCount = attendanceRecords.Count(r => r.IsPresent);
        var attendancePercentage = totalSessions > 0 
            ? Math.Round((presentCount * 100.0) / totalSessions, 2) 
            : 0;

        return new
        {
            StudentId = student.StudentId,
            StudentName = student.Name,
            Email = student.Email,
            TotalGrade = student.TotalGrade,
            Attendance = new
            {
                TotalSessions = totalSessions,
                PresentCount = presentCount,
                AbsentCount = totalSessions - presentCount,
                AttendancePercentage = attendancePercentage
            }
        };
    }

    /// <summary>
    /// Bulk update grades for multiple students.
    /// </summary>
    public async Task<(int SuccessCount, int FailureCount, List<string> Errors)> BulkUpdateGradesAsync(
        Dictionary<int, decimal?> gradeUpdates)
    {
        int successCount = 0;
        int failureCount = 0;
        var errors = new List<string>();

        foreach (var update in gradeUpdates)
        {
            var result = await UpdateStudentGradeAsync(update.Key, update.Value);
            if (result.Success)
            {
                successCount++;
            }
            else
            {
                failureCount++;
                errors.Add($"StudentId {update.Key}: {result.Message}");
            }
        }

        return (successCount, failureCount, errors);
    }
}
