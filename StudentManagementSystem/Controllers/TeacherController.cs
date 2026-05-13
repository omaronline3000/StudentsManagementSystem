using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Filters;
using StudentManagementSystem.Models;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers;

[StudentManagementSystem.Filters.SessionAuthorize("Teacher", "Admin")]
[Route("[controller]")]
public class TeacherController : Controller
{
    private const string UserTypeSessionKey = "UserType";
    private const string UserIdSessionKey = "UserId";
    private readonly StudentManagementDbContext _context;

    public TeacherController(StudentManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet("Profile")]
    public async Task<IActionResult> Profile()
    {
        var teacherId = GetCurrentTeacherId();
        if (teacherId == null)
            return RedirectToAction("Login", "Account");

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.TeacherId == teacherId.Value && !t.IsDeleted);

        if (teacher == null)
            return RedirectToAction("Login", "Account");

        var totalSessions = await _context.AttendanceSessions
            .CountAsync(s => s.TeacherId == teacherId.Value);

        var recentSessions = await _context.AttendanceSessions
            .Where(s => s.TeacherId == teacherId.Value)
            .OrderByDescending(s => s.CreatedAt)
            .Take(8)
            .ToListAsync();

        var totalAttendanceRecords = await _context.AttendanceRecords
            .Join(
                _context.AttendanceSessions.Where(s => s.TeacherId == teacherId.Value),
                record => record.SessionId,
                session => session.SessionId,
                (record, session) => record
            )
            .CountAsync();

        var model = new TeacherProfileViewModel
        {
            Teacher = teacher,
            TotalSessions = totalSessions,
            TotalAttendanceRecords = totalAttendanceRecords,
            RecentSessions = recentSessions
        };

        return View(model);
    }

    private int? GetCurrentTeacherId()
    {
        if (HttpContext.Session.GetString(UserTypeSessionKey) != "Teacher")
            return null;

        return HttpContext.Session.GetInt32(UserIdSessionKey);
    }
}
