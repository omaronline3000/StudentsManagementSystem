using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Filters;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers;

[StudentManagementSystem.Filters.SessionAuthorize("Teacher", "Admin")]
[Route("[controller]")]
public class AttendanceController : Controller
{
    private const string UserTypeSessionKey = "UserType";
    private const string UserIdSessionKey = "UserId";
    private readonly StudentManagementDbContext _context;
    private readonly ExcelParsingService _excelParsingService;

    public AttendanceController(StudentManagementDbContext context, ExcelParsingService excelParsingService)
    {
        _context = context;
        _excelParsingService = excelParsingService;
    }

    [HttpGet("/Teacher/Sessions")]
    public async Task<IActionResult> Index()
    {
        var query = _context.AttendanceSessions
            .Include(s => s.Teacher)
            .AsQueryable();

        if (HttpContext.Session.GetString(UserTypeSessionKey) == "Teacher")
        {
            var teacherId = HttpContext.Session.GetInt32(UserIdSessionKey);
            if (teacherId.HasValue)
                query = query.Where(s => s.TeacherId == teacherId.Value);
        }

        var sessions = await query
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return View(sessions);
    }

    [HttpGet("Details/{sessionId}")]
    public async Task<IActionResult> Details(int sessionId)
    {
        var session = await _context.AttendanceSessions
            .Include(s => s.Teacher)
            .Include(s => s.AttendanceRecords)
            .ThenInclude(a => a.Student)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null)
            return NotFound();

        if (HttpContext.Session.GetString(UserTypeSessionKey) == "Teacher")
        {
            var teacherId = HttpContext.Session.GetInt32(UserIdSessionKey);
            if (teacherId.HasValue && session.TeacherId != teacherId.Value)
            {
                TempData["ErrorMessage"] = "You do not have permission to view this session.";
                return RedirectToAction(nameof(Index));
            }
        }

        return View(session);
    }

    [HttpGet("Upload")]
    public async Task<IActionResult> Upload()
    {
        var model = new AttendanceUploadViewModel();

        if (HttpContext.Session.GetString(UserTypeSessionKey) == "Teacher")
        {
            var teacherId = HttpContext.Session.GetInt32(UserIdSessionKey);
            if (teacherId.HasValue)
                model.TeacherId = teacherId.Value;
        }

        return View(model);
    }

    [HttpPost("Upload")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(AttendanceUploadViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            if (model.TeacherId == 0 && HttpContext.Session.GetString(UserTypeSessionKey) == "Teacher")
            {
                var teacherId = HttpContext.Session.GetInt32(UserIdSessionKey);
                if (teacherId.HasValue)
                    model.TeacherId = teacherId.Value;
            }

            if (model.TeacherId == 0)
            {
                ModelState.AddModelError(string.Empty, "Teacher ID is required.");
                return View(model);
            }

            var rawUploadedNames = _excelParsingService.ParseAttendanceFile(model.ExcelFile);
            var uploadedNormalized = new Dictionary<string, string>();
            
            foreach(var name in rawUploadedNames) 
            {
                var norm = NormalizeName(name);
                if (!string.IsNullOrEmpty(norm) && !uploadedNormalized.ContainsKey(norm))
                    uploadedNormalized[norm] = name;
            }

            var teacherExists = await _context.Teachers.AnyAsync(t => t.TeacherId == model.TeacherId && !t.IsDeleted);
            if (!teacherExists)
            {
                ModelState.AddModelError(string.Empty, "Teacher not found.");
                return View(model);
            }

            var activeStudents = await _context.Students
                .Where(s => !s.IsDeleted)
                .ToListAsync();

            var session = new AttendanceSession
            {
                TeacherId = model.TeacherId,
                CourseTitle = model.CourseTitle,
                AttendanceDate = model.AttendanceDate?.Date ?? DateTime.UtcNow.Date,
                UploadedFileName = model.ExcelFile.FileName,
                CreatedAt = DateTime.UtcNow
            };

            _context.AttendanceSessions.Add(session);
            await _context.SaveChangesAsync();

            int matchCount = 0;
            var matchedNormalized = new HashSet<string>();

            foreach (var student in activeStudents)
            {
                var normDbName = NormalizeName(student.Name);
                var isPresent = uploadedNormalized.ContainsKey(normDbName);
                
                if (isPresent) 
                {
                    matchedNormalized.Add(normDbName);
                    matchCount++;
                }

                _context.AttendanceRecords.Add(new AttendanceRecord
                {
                    SessionId = session.SessionId,
                    StudentId = student.StudentId,
                    IsPresent = isPresent
                });
            }

            await _context.SaveChangesAsync();

            var unrecognized = uploadedNormalized.Where(k => !matchedNormalized.Contains(k.Key)).Select(k => k.Value).ToList();
            
            if (unrecognized.Any())
            {
                var preview = string.Join(", ", unrecognized.Take(5));
                var ellipsis = unrecognized.Count > 5 ? "..." : "";
                TempData["WarningMessage"] = $"Matched {matchCount} students. Unrecognized names in Excel: {preview}{ellipsis}";
            }
            else
            {
                TempData["SuccessMessage"] = $"Attendance uploaded successfully. Matched {matchCount} students.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    private static string NormalizeName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Trim().ToLowerInvariant();

        // Arabic character normalization (fixes mismatched Alefs, Ya/Alef-Maksura, Ha/Ta-Marbuta)
        normalized = normalized.Replace("أ", "ا").Replace("إ", "ا").Replace("آ", "ا")
                               .Replace("ة", "ه").Replace("ى", "ي");

        // Remove ALL non-alphanumeric characters (spaces, hyphens, punctuation, zero-width chars, Arabic diacritics)
        // This ensures 'John Doe', 'John-Doe', and 'JohnDoe' will all match perfectly as 'johndoe'
        normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"[^\p{L}\p{N}]", "");

        return normalized;
    }
}
