# Manage Employee Application

## Overview

Manage Employee is a self-hosted employee management system built using .NET 8 and SQL Server.

The solution contains:

- ASP.NET Core MVC Web Application
- Console Application
- Shared Business Layer
- Shared Data Access Layer

The application supports:

- Employee listing
- Employee search by name or title
- Title summary with minimum and maximum salary
- Add employee
- Salary history management
- Active and exited employees

---

# Technologies Used

- .NET 8
- ASP.NET Core MVC
- Console Application
- ADO.NET
- SQL Server
- Razor Views
- Layered Architecture

---

# Solution Structure

ManageEmployeeSolution
│
├── ManageEmployee.Web
├── ManageEmployee.Console
├── ManageEmployee.Business
├── ManageEmployee.Data
├── ManageEmployee.Models
│
└── Database
    ├── 01_CreateTables.sql
    ├── 02_StoredProcedures.sql
    └── 03_SeedData.sql

# Setup Instructions
1. Clone Repository
  git clone https://github.com/sumithkmohan-psl/manage-employee.git
2. Create Database
3. Configure Database
  Update connection string in: appsettings.json
4. Run SQL Scripts
  Execute scripts in order:
    01_CreateTables.sql
    02_StoredProcedures.sql
    03_SeedData.sql
