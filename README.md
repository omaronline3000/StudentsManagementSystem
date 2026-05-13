# 🎓 Student Management System

Welcome to the **Student Management System**! This is a robust web application built using **ASP.NET Core (.NET 10)** designed to streamline the management of student records, teacher profiles, and course attendance.

## ✨ Key Features

- **👩‍🏫 Teacher Dashboard:** A dedicated profile page for teachers to manage their information and view assigned courses.
- **📊 Attendance Tracking:** Easily upload attendance records (supports file uploads like Excel/CSV) and view recent class sessions.
- **📚 Course & Session Management:** Track course titles, session dates, and uploaded attendance files.
- **📱 Responsive UI:** A clean, mobile-friendly interface built with **Bootstrap 5** and enhanced with **FontAwesome** icons.
- **✅ Client-Side Validation:** Fast and secure form validations powered by **jQuery Validation**.

## 🛠️ Technology Stack

**Backend:**
- C#
- ASP.NET Core MVC / Razor Pages (.NET 10.0)
- Entity Framework Core (ORM)

**Frontend:**
- HTML5, CSS3, Razor (`.cshtml`)
- Bootstrap 5
- jQuery & jQuery Validation
- FontAwesome (Icons)

## 🚀 Getting Started

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

Ensure you have the following installed:
- .NET 10.0 SDK
- Visual Studio 2022 (v17.10+) OR Visual Studio Code
- SQL Server (or your preferred configured database)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/StudentManagementSystem.git
   cd StudentManagementSystem
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Apply Database Migrations**
   Make sure your connection string in `appsettings.json` is correctly set, then run:
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```
   The application will start, and you can access it by navigating to `http://localhost:5000` (or the port specified in your console) in your web browser.

## 📄 License

This project is licensed under the Assiut University License.
