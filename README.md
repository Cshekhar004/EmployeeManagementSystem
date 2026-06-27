# Employee Management System

A simple ASP.NET Core MVC application for managing employees, departments, users and roles with audit logging. It provides CRUD for employees and departments, user authentication/management, role-based authorization, and EF Core migrations for database schema management.

## Stack
- Language(s): C# (server-side), Razor/HTML/CSS (views)
- Framework / runtime: ASP.NET Core MVC (Razor views)
- Notable libraries: Entity Framework Core (migrations & DbContext), ASP.NET Core MVC, simple custom authorization attributes (Filters)

## Features
- Employee CRUD (create, read, update, delete) with an "IsActive" flag
- Department CRUD
- User and Role management (basic users, roles, login/password flows)
- Audit logging for important operations
- Entity Framework Core migrations included to create and update the database schema
- Razor-based Views served from Views/

## Repository layout
Top-level entries:
- EmployeeManagement.csproj — project file
- Program.cs — application entrypoint / host configuration
- appsettings.json — configuration (connection strings, logging, etc.)
- Controllers/ — MVC controllers (AccountController, EmployeeController, DepartmentController, RoleController, UserController, AuditController, HomeController, ErrorController)
- Data/ — EF Core DbContext (EmployeeDbContext.cs)
- Models/ — domain models and view models (Employee, Department, User, Role, AuditLog, ViewModels)
  - Models/ViewModels/ — view-specific models (UserViewModel, ChangePasswordViewModel, etc.)
- Filters/ — custom authorization/session filters (RoleAuthorizeAttribute, SessionAuthorizeAttribute)
- Migrations/ — EF Core migration files and model snapshot
- Views/ — Razor views (organized by controller: Account, Employee, Department, Role, User, Audit, Home, Shared, Error)
- wwwroot/ — static assets (css/, js/, lib/, favicon.ico)
- Properties/ — assembly / launch settings (if any)
- .gitignore

How it fits together:
- Program.cs configures the web host, services and middleware.
- EmployeeDbContext (Data/) is registered and used by controllers and EF Core migrations to read/write data.
- Controllers handle HTTP requests, use models and DbContext, and return Razor views located in Views/.
- Filters enforce session and role-based access for controller actions.
- Migrations contain the database schema history and can be applied to initialize/update the database.

## Getting started (shortest path)
Prerequisites:
- .NET 6.0 or later SDK installed (use the version your project targets — check EmployeeManagement.csproj)
- A database server (SQL Server / LocalDB is commonly used with ASP.NET Core; update connection string if using another provider)
- (Optional) dotnet-ef tool for applying migrations: `dotnet tool install --global dotnet-ef`

Steps:
1. Clone the repository
   ```bash
   git clone https://github.com/Cshekhar004/EmployeeManagementSystem.git
   cd EmployeeManagementSystem
