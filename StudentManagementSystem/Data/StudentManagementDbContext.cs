using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data;

public class StudentManagementDbContext : DbContext
{
    public StudentManagementDbContext(DbContextOptions<StudentManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<AttendanceSession> AttendanceSessions { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Student entity
        modelBuilder.Entity<Student>()
            .HasKey(s => s.StudentId);

        modelBuilder.Entity<Student>()
            .Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Student>()
            .Property(s => s.Address)
            .HasMaxLength(500);

        modelBuilder.Entity<Student>()
            .Property(s => s.Email)
            .HasMaxLength(320);

        modelBuilder.Entity<Student>()
            .Property(s => s.NationalId)
            .HasMaxLength(50);

        modelBuilder.Entity<Student>()
            .Property(s => s.TotalGrade)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Student>()
            .Property(s => s.IsDeleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Student>()
            .HasMany(s => s.AttendanceRecords)
            .WithOne(ar => ar.Student)
            .HasForeignKey(ar => ar.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Teacher entity
        modelBuilder.Entity<Teacher>()
            .HasKey(t => t.TeacherId);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.Address)
            .HasMaxLength(500);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.Email)
            .HasMaxLength(320);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.NationalId)
            .HasMaxLength(50);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.Position)
            .HasMaxLength(100);

        modelBuilder.Entity<Teacher>()
            .Property(t => t.IsDeleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Teacher>()
            .HasMany(t => t.AttendanceSessions)
            .WithOne(s => s.Teacher)
            .HasForeignKey(s => s.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Admin entity
        modelBuilder.Entity<Admin>()
            .HasKey(a => a.AdminId);

        modelBuilder.Entity<Admin>()
            .Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Admin>()
            .Property(a => a.Address)
            .HasMaxLength(500);

        modelBuilder.Entity<Admin>()
            .Property(a => a.Email)
            .HasMaxLength(320);

        modelBuilder.Entity<Admin>()
            .Property(a => a.NationalId)
            .HasMaxLength(50);

        // Configure AttendanceSession entity
        modelBuilder.Entity<AttendanceSession>()
            .HasKey(s => s.SessionId);

        modelBuilder.Entity<AttendanceSession>()
            .Property(s => s.CourseTitle)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<AttendanceSession>()
            .Property(s => s.UploadedFileName)
            .HasMaxLength(260);

        modelBuilder.Entity<AttendanceSession>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<AttendanceSession>()
            .HasOne(s => s.Teacher)
            .WithMany(t => t.AttendanceSessions)
            .HasForeignKey(s => s.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttendanceSession>()
            .HasMany(s => s.AttendanceRecords)
            .WithOne(ar => ar.AttendanceSession)
            .HasForeignKey(ar => ar.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure AttendanceRecord entity
        modelBuilder.Entity<AttendanceRecord>()
            .HasKey(ar => ar.RecordId);

        modelBuilder.Entity<AttendanceRecord>()
            .Property(ar => ar.IsPresent)
            .HasDefaultValue(false);

        modelBuilder.Entity<AttendanceRecord>()
            .HasOne(ar => ar.AttendanceSession)
            .WithMany(s => s.AttendanceRecords)
            .HasForeignKey(ar => ar.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AttendanceRecord>()
            .HasOne(ar => ar.Student)
            .WithMany(s => s.AttendanceRecords)
            .HasForeignKey(ar => ar.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
