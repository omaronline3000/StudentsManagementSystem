using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Filters;
using StudentManagementSystem.Services;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers;

[StudentManagementSystem.Filters.SessionAuthorize("Student", "Admin")]
[Route("[controller]")]
public class StudentController : Controller
{
    private const string UserTypeSessionKey = "UserType";
    private const string UserIdSessionKey = "UserId";
    private readonly StudentManagementDbContext _context;
    private readonly StudentAttendanceService _attendanceService;

    public StudentController(StudentManagementDbContext context, StudentAttendanceService attendanceService)
    {
        _context = context;
        _attendanceService = attendanceService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var students = await _context.Students
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.StudentId)
            .ToListAsync();

        return View(students);
    }

    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == id && !s.IsDeleted);

        if (student == null)
            return NotFound();

        return View(student);
    }

    [HttpGet("Profile")]
    public async Task<IActionResult> Profile()
    {
        var studentId = GetCurrentStudentId();
        if (studentId == null)
            return RedirectToAction("Login", "Account");

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == studentId.Value && !s.IsDeleted);

        if (student == null)
            return RedirectToAction("Login", "Account");

        var attendanceRecords = (await _attendanceService.GetStudentAttendanceAsync(studentId.Value)).ToList();
        var stats = await _attendanceService.GetStudentAttendanceStatsAsync(studentId.Value)
            ?? new StudentAttendanceStatsViewModel { StudentId = studentId.Value };

        var model = new StudentProfileViewModel
        {
            Student = student,
            Statistics = stats,
            AttendanceRecords = attendanceRecords.Take(8)
        };

        return View(model);
    }

    [HttpGet("Attendance")]
    public async Task<IActionResult> Attendance(int? id = null)
    {
        var studentId = id ?? GetCurrentStudentId();
        if (studentId == null)
            return RedirectToAction("Login", "Account");

        var attendance = await _attendanceService.GetStudentAttendanceAsync(studentId.Value);
        ViewBag.StudentId = studentId.Value;
        ViewBag.StudentName = await _context.Students
            .Where(s => s.StudentId == studentId.Value && !s.IsDeleted)
            .Select(s => s.Name)
            .FirstOrDefaultAsync();

        return View(attendance);
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == id && !s.IsDeleted);

        if (student != null)
        {
            student.IsDeleted = true;
            student.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Student deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    private int? GetCurrentStudentId()
    {
        if (HttpContext.Session.GetString(UserTypeSessionKey) != "Student")
            return null;

        return HttpContext.Session.GetInt32(UserIdSessionKey);
    }
}
