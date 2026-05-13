using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers;

public class AccountController : Controller
{
    private readonly StudentManagementDbContext _context;
    private const string UserTypeSessionKey = "UserType";
    private const string UserIdSessionKey = "UserId";
    private const string UserNameSessionKey = "UserName";

    public AccountController(StudentManagementDbContext context)
    {
        _context = context;
    }

    // GET: /account/login
    [HttpGet]
    public IActionResult Login()
    {
        if (IsUserLoggedIn())
            return RedirectToUserDashboard();

        return View();
    }

    // POST: /account/login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.NationalId))
        {
            ModelState.AddModelError(string.Empty, "Email and National ID are required.");
            return View(model);
        }

        // Check Student (exclude soft-deleted)
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Email == model.Email && 
                                      s.NationalId == model.NationalId && 
                                      !s.IsDeleted);

        if (student != null)
        {
            SetUserSession("Student", student.StudentId, student.Name);
            return RedirectToAction("Profile", "Student");
        }

        // Check Teacher (exclude soft-deleted)
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Email == model.Email && 
                                      t.NationalId == model.NationalId && 
                                      !t.IsDeleted);

        if (teacher != null)
        {
            SetUserSession("Teacher", teacher.TeacherId, teacher.Name);
            return RedirectToAction("Profile", "Teacher");
        }

        // Check Admin (never soft-deleted)
        var admin = await _context.Admins
            .FirstOrDefaultAsync(a => a.Email == model.Email && 
                                      a.NationalId == model.NationalId);

        if (admin != null)
        {
            SetUserSession("Admin", admin.AdminId, admin.Name);
            return RedirectToAction("Index", "Admin");
        }

        // Invalid credentials
        ModelState.AddModelError(string.Empty, "Invalid email or national ID.");
        return View(model);
    }

    // GET: /account/logout
    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }

    // Helper: Set user session data
    private void SetUserSession(string userType, int userId, string userName)
    {
        HttpContext.Session.SetString(UserTypeSessionKey, userType);
        HttpContext.Session.SetInt32(UserIdSessionKey, userId);
        HttpContext.Session.SetString(UserNameSessionKey, userName);
    }

    // Helper: Check if user is logged in
    private bool IsUserLoggedIn()
    {
        return HttpContext.Session.GetString(UserTypeSessionKey) != null;
    }

    // Helper: Redirect to appropriate dashboard based on user type
    private IActionResult RedirectToUserDashboard()
    {
        var userType = HttpContext.Session.GetString(UserTypeSessionKey);

        return userType switch
        {
            "Student" => RedirectToAction("Profile", "Student"),
            "Teacher" => RedirectToAction("Profile", "Teacher"),
            "Admin" => RedirectToAction("Index", "Admin"),
            _ => RedirectToAction("Login")
        };
    }
}
