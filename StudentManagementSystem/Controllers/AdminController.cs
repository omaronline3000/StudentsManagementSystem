using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Filters;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers;

public class AdminController : Controller
{
    private readonly StudentManagementDbContext _context;
    private readonly AdminGradeService _gradeService;

    public AdminController(StudentManagementDbContext context, AdminGradeService gradeService)
    {
        _context = context;
        _gradeService = gradeService;
    }

    // GET: /admin
    public async Task<IActionResult> Index()
    {
        var totalStudents = await _context.Students.CountAsync(s => !s.IsDeleted);
        var totalTeachers = await _context.Teachers.CountAsync(t => !t.IsDeleted);
        var totalAttendanceSessions = await _context.AttendanceSessions.CountAsync();
        var totalAttendanceRecords = await _context.AttendanceRecords.CountAsync();

        ViewBag.TotalStudents = totalStudents;
        ViewBag.TotalTeachers = totalTeachers;
        ViewBag.TotalAttendanceSessions = totalAttendanceSessions;
        ViewBag.TotalAttendanceRecords = totalAttendanceRecords;

        return View();
    }

    // GET: /admin/students
    public async Task<IActionResult> Students()
    {
        var students = await _context.Students
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.StudentId)
            .ToListAsync();

        return View(students);
    }

    // GET: /admin/student-create
    public IActionResult StudentCreate()
    {
        return View();
    }

    // POST: /admin/student-create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StudentCreate(Student student)
    {
        if (ModelState.IsValid)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Student '{student.Name}' has been added successfully.";
            return RedirectToAction("Students");
        }
        return View(student);
    }

    // GET: /admin/teachers
    public async Task<IActionResult> Teachers()
    {
        var teachers = await _context.Teachers
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.TeacherId)
            .ToListAsync();

        return View(teachers);
    }

    // GET: /admin/teacher-create
    public IActionResult TeacherCreate()
    {
        return View();
    }

    // POST: /admin/teacher-create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TeacherCreate(Teacher teacher)
    {
        if (ModelState.IsValid)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Teacher '{teacher.Name}' has been added successfully.";
            return RedirectToAction("Teachers");
        }
        return View(teacher);
    }

    // GET: /admin/student-edit/{studentId}
    public async Task<IActionResult> StudentEdit(int studentId)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted);

        if (student == null)
        {
            TempData["ErrorMessage"] = "Student not found.";
            return RedirectToAction("Students");
        }

        return View(student);
    }

    // POST: /admin/student-edit/{studentId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StudentEdit(int studentId, Student updatedStudent)
    {
        if (studentId != updatedStudent.StudentId)
        {
            ModelState.AddModelError(string.Empty, "Student ID mismatch.");
            return View(updatedStudent);
        }

        if (!ModelState.IsValid)
            return View(updatedStudent);

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted);

        if (student == null)
        {
            TempData["ErrorMessage"] = "Student not found.";
            return RedirectToAction("Students");
        }

        student.Name = updatedStudent.Name;
        student.Email = updatedStudent.Email;
        student.NationalId = updatedStudent.NationalId;
        student.Address = updatedStudent.Address;

        await _context.SaveChangesAsync();

        var (success, message, updatedGrade) = await _gradeService.UpdateStudentGradeAsync(
            studentId,
            updatedStudent.TotalGrade
        );

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(updatedStudent);
        }

        TempData["SuccessMessage"] = $"Student '{student.Name}' updated successfully.";
        return RedirectToAction("Students");
    }

    // GET: /admin/grades-bulk-update
    public async Task<IActionResult> GradesBulkUpdate()
    {
        var students = await _context.Students
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.StudentId)
            .ToListAsync();

        return View(students);
    }

    // POST: /admin/grades-bulk-update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GradesBulkUpdate(Dictionary<int, decimal?> gradeUpdates)
    {
        if (gradeUpdates == null || gradeUpdates.Count == 0)
        {
            TempData["ErrorMessage"] = "No grade updates provided.";
            return RedirectToAction("GradesBulkUpdate");
        }

        var (successCount, failureCount, errors) = await _gradeService.BulkUpdateGradesAsync(gradeUpdates);

        TempData["SuccessMessage"] = $"Bulk update completed. {successCount} successful, {failureCount} failed.";
        if (errors.Count > 0)
            TempData["ErrorDetails"] = string.Join("; ", errors);

        return RedirectToAction("Students");
    }

    // GET: /admin/teacher-edit/{teacherId}
    public async Task<IActionResult> TeacherEdit(int teacherId)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.TeacherId == teacherId && !t.IsDeleted);

        if (teacher == null)
        {
            TempData["ErrorMessage"] = "Teacher not found.";
            return RedirectToAction("Teachers");
        }

        return View(teacher);
    }

    // POST: /admin/teacher-edit/{teacherId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TeacherEdit(int teacherId, Teacher updatedTeacher)
    {
        if (teacherId != updatedTeacher.TeacherId)
        {
            ModelState.AddModelError(string.Empty, "Teacher ID mismatch.");
            return View(updatedTeacher);
        }

        if (!ModelState.IsValid)
            return View(updatedTeacher);

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.TeacherId == teacherId && !t.IsDeleted);

        if (teacher == null)
        {
            TempData["ErrorMessage"] = "Teacher not found.";
            return RedirectToAction("Teachers");
        }

        teacher.Name = updatedTeacher.Name;
        teacher.Email = updatedTeacher.Email;
        teacher.NationalId = updatedTeacher.NationalId;
        teacher.Position = updatedTeacher.Position;
        teacher.Address = updatedTeacher.Address;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Teacher '{teacher.Name}' updated successfully.";
        return RedirectToAction("Teachers");
    }

    // POST: /admin/delete-student/{studentId} - Soft delete
    [HttpPost]
    public async Task<IActionResult> DeleteStudent(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);

        if (student == null)
        {
            TempData["ErrorMessage"] = "Student not found.";
            return RedirectToAction("Students");
        }

        // Soft delete
        student.IsDeleted = true;
        student.DeletedAt = DateTime.UtcNow;
        _context.Update(student);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Student '{student.Name}' has been deleted.";
        return RedirectToAction("Students");
    }

    // POST: /admin/delete-teacher/{teacherId} - Soft delete
    [HttpPost]
    public async Task<IActionResult> DeleteTeacher(int teacherId)
    {
        var teacher = await _context.Teachers.FindAsync(teacherId);

        if (teacher == null)
        {
            TempData["ErrorMessage"] = "Teacher not found.";
            return RedirectToAction("Teachers");
        }

        // Soft delete
        teacher.IsDeleted = true;
        teacher.DeletedAt = DateTime.UtcNow;
        _context.Update(teacher);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Teacher '{teacher.Name}' has been deleted.";
        return RedirectToAction("Teachers");
    }
}
