# Employee Management System (EMS)

A modern enterprise-style **Employee Management System (EMS)** built using **ASP.NET Core MVC**, **C#**, **Entity Framework Core**, and **MySQL** with secure authentication, role-based authorization, audit logging, and interactive analytics dashboard.

---

# Project Overview

The Employee Management System (EMS) is designed to streamline employee and administrative operations inside an organization.

This system enables secure management of:

- Employees
- Departments
- Users
- Roles
- Security Access
- Audit Logs
- Analytics Dashboard

The project follows **ASP.NET Core MVC architecture** and provides a modern **SaaS-style responsive UI** using Bootstrap 5.

---

# Tech Stack

## Backend
- ASP.NET Core MVC
- C#
- Entity Framework Core
- MySQL

## Frontend
- HTML5
- CSS3
- Bootstrap 5
- Bootstrap Icons
- JavaScript
- jQuery
- AJAX
- Chart.js

## Tools
- Visual Studio Code
- Git
- GitHub
- MySQL Workbench

---

# Core Features

---

# Authentication & Security

Implemented enterprise-level authentication and security features.

Features:

- Login / Logout
- Session-based authentication
- Remember Me functionality
- Password hashing (SHA-based helper)
- Role-based authorization
- Change Password
- Forgot Password
- Admin Reset Password
- Force password change after admin reset
- Secure route protection using custom filters
- Unauthorized access toast notification
- Logout confirmation modal

Security Enhancements:

- Passwords are never stored as plain text
- Aadhaar card numbers stored as hash
- PAN card numbers stored as hash
- Sensitive values masked in UI
- Remember Me token stored securely
- Token cleared on logout

---

# Dashboard

Interactive analytics dashboard showing:

- Total Employees
- Total Departments
- Male Employees
- Female Employees
- Highest Salary
- Lowest Salary
- Average Salary
- Employees Added This Month

Charts:

- Employees by Department (Bar Chart)
- Gender Distribution (Doughnut Chart)

---

# Employee Management

Complete employee lifecycle management.

Features:

- Create Employee
- View Employee Details
- Edit Employee
- Soft Delete Employee
- Search Employees
- Filter by Department
- Filter by Month
- Filter by Week
- Export to Excel
- Export to PDF
- Pagination
- Active / Inactive toggle
- Confirmation modal for status change

Employee Information:

- Employee Code
- Employee Name
- Department
- Gender
- Date of Birth
- Salary
- Created Date
- Status

Additional Improvements:

- Pagination preserved during status toggle
- Search/filter preserved during actions
- Duplicate create prevention
- Create/Edit redirect bug fixed

---

# Department Management

Manage organizational departments.

Features:

- Create Department
- View Department Details
- Edit Department
- Delete Department
- Department-wise Employee Count
- Pagination
- Toast notifications

Protection:

- Department cannot be deleted if employees are assigned
- Error toast shown on invalid delete

---

# User Management

Admin-only secure user management.

Features:

- Create User
- View User Details
- Edit User
- Activate / Deactivate User
- Reset Password
- Role Assignment
- Aadhaar & PAN storage
- Sensitive data masking

User Fields:

- Name
- Username
- Email
- Role
- Password (Hashed)
- Aadhaar Number (Hashed)
- PAN Number (Hashed)
- Status

Security:

### Aadhaar
Stored as:
- Hash
- Last 4 digits only for masking

Example:
```txt
XXXX-XXXX-1234
```

### PAN
Stored as:
- Hash
- Masked value

Example:
```txt
XXXXX1234X
```

---

# Password Management

Advanced password handling system.

Features:

### Admin Reset Password
- Confirmation modal before reset
- Reset to configurable default password
- Password stored as hash
- Audit logged

### Force Password Change
After admin reset:

```txt
MustChangePassword = true
```

When user logs in:

- Dashboard popup appears
- Prompt to change password
- User may change immediately or later

After successful password change:

```txt
MustChangePassword = false
```

---

# Role Management

Manage system roles.

Available Roles:

- Admin
- HR
- Manager
- User

Features:

- Create Role
- View Role
- Edit Role
- Delete Role
- Assign Permissions

---

# Role-Based Access Control (RBAC)

Access rules:

## Admin
Full access:

- Dashboard
- Employees
- Departments
- Users
- Roles
- Audit Trail
- Settings

---

## HR
Access to:

- Dashboard
- Employees
- Departments

Restrictions:
- Cannot access Users
- Cannot access Roles
- Cannot access Settings

---

## Manager
Access to:

- Dashboard
- Employees (View Only)

Restrictions:
- Create hidden
- Edit hidden
- Delete hidden
- Status toggle hidden

---

## User
Limited access.

---

# Audit Trail

Tracks system actions for security and monitoring.

Audit Fields:

- Module
- Action
- Record Info
- Performed By
- Action Date

Examples:

- Employee Created
- Employee Updated
- Employee Activated
- User Created
- Password Reset
- Department Deleted
- User Changed Password
- Role Updated

Features:

- Pagination
- Page size filter
- 10 / 20 / 50 / 100 records per page

---

# UI / UX Features

Modern SaaS dashboard design.

Features:

- Responsive layout
- Sidebar navigation
- Professional cards
- Data tables
- Bootstrap modals
- Toast notifications
- Confirmation dialogs
- Pagination
- Advanced footer
- Improved login page
- Password visibility toggle
- Role-aware UI rendering

---

# Custom Filters Used

## SessionAuthorizeAttribute
Checks login session.

Flow:

```txt
No session → Redirect to Login
```

---

## RoleAuthorizeAttribute
Checks role permission.

Flow:

```txt
Unauthorized role
→ Redirect to previous page
→ Show toast notification
```

---

# MVC Project Structure

```bash
EmployeeManagementSystem/
│
├── Controllers/
│   ├── AccountController.cs
│   ├── HomeController.cs
│   ├── EmployeeController.cs
│   ├── DepartmentController.cs
│   ├── UserController.cs
│   ├── RoleController.cs
│   └── AuditController.cs
│
├── Models/
│   ├── Employee.cs
│   ├── Department.cs
│   ├── User.cs
│   ├── Role.cs
│   ├── AuditLog.cs
│   └── ViewModels/
│
├── Data/
│   └── EmployeeDbContext.cs
│
├── Filters/
│   ├── SessionAuthorizeAttribute.cs
│   └── RoleAuthorizeAttribute.cs
│
├── Helpers/
│   ├── PasswordHelper.cs
│   └── RememberMeHelper.cs
│
├── Views/
│   ├── Account/
│   ├── Home/
│   ├── Employee/
│   ├── Department/
│   ├── User/
│   ├── Role/
│   ├── Audit/
│   └── Shared/
│
└── wwwroot/
    ├── css/
    ├── js/
    └── lib/
```

---

# Database Tables

Main Tables:

- Employees
- Departments
- Users
- Roles
- AuditLogs

---

# Installation Guide

## 1. Clone Repository

```bash
git clone https://github.com/Cshekhar004/EmployeeManagementSystem.git
```

---

## 2. Navigate to Project

```bash
cd EmployeeManagementSystem
```

---

## 3. Restore Packages

```bash
dotnet restore
```

---

## 4. Configure Database

Update connection string in:

```json
appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;database=EmployeeManagementDB;user=root;password=YOUR_PASSWORD;"
}
```

---

## 5. Run Migrations

```bash
dotnet ef database update
```

---

## 6. Run Project

```bash
dotnet build
dotnet run
```

---

# Screenshots

Add screenshots here.

Examples:

- Login Page
  <img width="1918" height="927" alt="image" src="https://github.com/user-attachments/assets/e4fff1a5-2c7e-4931-90bb-72f08b08a34b" />
  
- Dashboard
  <img width="1900" height="927" alt="image" src="https://github.com/user-attachments/assets/bd83860c-7efb-44c3-a58f-f5562c0cbf69" />

- Employee List
  <img width="1901" height="927" alt="image" src="https://github.com/user-attachments/assets/ec851aa8-2869-4988-8069-a84736520959" />

- Department List
  <img width="1901" height="927" alt="image" src="https://github.com/user-attachments/assets/e9ec30a4-357c-4fce-ab6a-26f8d0eb5351" />

- User Management
  <img width="1906" height="926" alt="image" src="https://github.com/user-attachments/assets/c6c55eba-2060-4ae8-b0ca-ef50e8e1dfac" />

- Audit Trail
  <img width="1896" height="927" alt="image" src="https://github.com/user-attachments/assets/70cf3875-3b28-48a1-bbf0-3d9a6bbfca68" />

---

# Learning Outcomes

This project helped me learn:

- ASP.NET Core MVC architecture
- Entity Framework Core
- MySQL integration
- Session Authentication
- Role-Based Authorization
- Password Hashing
- Secure Token Handling
- Audit Logging
- Bootstrap Modal Workflow
- AJAX CRUD Operations
- Pagination & Filtering
- Dashboard Analytics
- Git & GitHub Workflow
- Real-world Debugging

---

# Future Enhancements

Possible upgrades:

- JWT Authentication
- Email Notifications
- Attendance Module
- Leave Management
- Payroll Module
- Cloud Deployment
- Docker Support
- CI/CD Pipeline
- API Version
- Mobile App

---

# Author

**Chandrashekhar Sahu**  
B.Tech Computer Engineering  
ASP.NET Core / C# Developer

GitHub:  
https://github.com/Cshekhar004

---

# License

This project is developed for educational, portfolio, and learning purposes.
