# Employee Management System (EMS)

A modern **Employee Management System** built using **ASP.NET Core MVC**, **C#**, **Entity Framework Core**, and **SQL Server**.  
This system helps organizations manage employees, departments, users, roles, and audit logs through a secure role-based dashboard.

---

## Project Overview

The Employee Management System (EMS) is designed to simplify employee data management and administrative operations within an organization.

It provides:

- Secure login and session management
- Role-based access control
- Employee management
- Department management
- User and role management
- Audit trail tracking
- Analytics dashboard
- Export reports (Excel/PDF)

The application follows a clean MVC architecture and includes a modern SaaS-style UI.

---

## Tech Stack

### Backend
- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server

### Frontend
- HTML
- CSS
- Bootstrap 5
- JavaScript
- jQuery
- Chart.js

### Tools
- Visual Studio Code
- Git
- GitHub
- SQL Server Management Studio (SSMS)

---

## Features

# Authentication & Security
- Login / Logout
- Session-based authentication
- Role-based authorization
- Change password
- Reset password
- Active / Inactive users
- Protected routes using custom filters

---

# Dashboard
Interactive dashboard showing:

- Total Employees
- Total Departments
- Male Employees
- Female Employees
- Highest Salary
- Lowest Salary
- Average Salary
- Employees Added This Month

Charts included:
- Employees by Department (Bar Chart)
- Gender Distribution (Doughnut Chart)

---

# Employee Management
Manage employee records with full CRUD operations.

Features:
- Create employee
- View employee details
- Edit employee
- Search employee
- Filter by gender
- Export to Excel
- Export to PDF
- Active / Inactive employee
- Department assignment

Employee details include:
- Employee Code
- Employee Name
- Department
- Gender
- Date of Birth
- Salary
- Created Date

---

# Department Management
Manage organization departments.

Features:
- Create department
- View department details
- Edit department
- Delete department
- Department-wise employee count

---

# User Management
Admin can manage system users.

Features:
- Create user
- View user details
- Edit user
- Reset password
- Activate / Deactivate user
- Role assignment

User roles include:
- Admin
- HR
- Manager
- User

---

# Role Management
Manage access roles.

Features:
- Create role
- View role
- Edit role
- Delete role
- Assign role permissions

---

# Audit Trail
Tracks important system actions.

Logs include:
- Module name
- Action performed
- Record info
- Performed by
- Date and time

Examples:
- Employee Created
- Employee Updated
- User Activated
- Password Reset
- Role Updated

---

# UI/UX Features
- Modern SaaS dashboard design
- Responsive layout
- Sidebar navigation
- Bootstrap modals
- Toast notifications
- Professional cards & tables
- Enhanced pagination
- Interactive charts

---

## Role-Based Access Control

### Admin
Access to:
- Dashboard
- Employees
- Departments
- Users
- Roles
- Audit Trail
- Settings

### HR
Access to:
- Dashboard
- Employees
- Departments

### Manager
Access to:
- Dashboard
- Employees (View)

### User
Limited access

---

## Project Structure

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
│   └── DashboardViewModel.cs
│
├── Views/
│   ├── Account/
│   ├── Home/
│   ├── Employee/
│   ├── Department/
│   ├── User/
│   ├── Role/
│   └── Audit/
│
├── Data/
│   └── EmployeeDbContext.cs
│
├── Filters/
│   ├── SessionAuthorizeAttribute.cs
│   └── RoleAuthorizeAttribute.cs
│
└── wwwroot/
    ├── css/
    ├── js/
    └── lib/
```

---

## Database Tables

Main tables:

- Employees
- Departments
- Users
- Roles
- AuditLogs

---

## Installation Guide

### 1. Clone Repository
```bash
git clone https://github.com/Cshekhar004/EmployeeManagementSystem.git
```

### 2. Navigate to Project
```bash
cd EmployeeManagementSystem
```

### 3. Restore Packages
```bash
dotnet restore
```

### 4. Configure Database
Update connection string in:

```json
appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=EmployeeManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 5. Run Project
```bash
dotnet build
dotnet run
```

---

## Future Enhancements
Possible improvements:

- Password hashing
- Email notifications
- Reports module
- Dark mode
- Cloud deployment
- Profile management
- Advanced analytics

---

## Screenshots
Add screenshots here after uploading them to GitHub.

Example:

- Login Page
  <img width="1918" height="930" alt="Screenshot 2026-06-27 145424" src="https://github.com/user-attachments/assets/dbf1b6f7-80ed-4dc3-b48f-59ca80575a62" />

- Dashboard
  <img width="1901" height="927" alt="Screenshot 2026-06-27 145455" src="https://github.com/user-attachments/assets/2a91d879-5775-4cf9-ada3-003579752216" />

- Employee List
  <img width="1918" height="928" alt="Screenshot 2026-06-27 145518" src="https://github.com/user-attachments/assets/9419edd7-cc9e-4e60-ae1e-579dd1f279ba" />

- User Management
  <img width="1918" height="927" alt="Screenshot 2026-06-27 145645" src="https://github.com/user-attachments/assets/fcbc3a7b-ef51-4f33-9069-a5d7479bc976" />

- Audit Trail
  <img width="1918" height="927" alt="image" src="https://github.com/user-attachments/assets/bf5841eb-3921-4296-ab1e-5bce72e619dc" />


---

## Learning Outcomes
Through this project I learned:

- ASP.NET Core MVC architecture
- Entity Framework Core
- SQL Server integration
- Authentication & Authorization
- Role-based security
- CRUD operations
- Dashboard analytics
- Git & GitHub workflow
- UI/UX design improvements

---

## Author

**Chandrashekhar Sahu**  
B.Tech Computer Engineering  
ASP.NET Core / C# Developer

GitHub:  
https://github.com/Cshekhar004

---

## License
This project is for educational and portfolio purposes.
