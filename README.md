# Employee Leave Management System

ASP.NET Core MVC (.NET 10) application for submitting and approving employee leave requests.

Built with **Clean Architecture**, **DDD**, and **CQRS (MediatR)**.

## Solution structure

| Project | Responsibility |
|---------|----------------|
| `EmployeeLeaveManagement.Domain` | Aggregates, value objects, domain rules, repository ports |
| `EmployeeLeaveManagement.Application` | Commands/queries, validators, DTOs |
| `EmployeeLeaveManagement.Infrastructure` | EF Core, SQL Server, Identity, repository implementations |
| `EmployeeLeaveManagement.Web` | MVC controllers, Razor views, Bootstrap UI |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, SQL Server Express, Docker, or Azure SQL)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

## Configuration

Connection strings live in:

- `EmployeeLeaveManagement.Web/appsettings.json`
- `EmployeeLeaveManagement.Web/appsettings.Development.json`

Default (Docker SQL Server on port 1433):

```json
"DefaultConnection": "Server=localhost,1433;Database=EmployeeLeaveManagement;User Id=sa;Password=Your_password123;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

**Windows / LocalDB alternative:**

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeLeaveManagement;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### Quick SQL Server via Docker

```bash
docker run -d --name elm-sql \
  -e 'ACCEPT_EULA=Y' \
  -e 'MSSQL_SA_PASSWORD=Your_password123' \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

## Setup

```bash
dotnet restore
dotnet build
dotnet ef database update \
  --project EmployeeLeaveManagement.Infrastructure \
  --startup-project EmployeeLeaveManagement.Web
dotnet run --project EmployeeLeaveManagement.Web --launch-profile http
```

On first run the app also migrates and seeds demo data automatically.

Browse to the HTTPS URL shown in the console (typically `https://localhost:7xxx`).

## Demo accounts

| Role | Email | Password |
|------|-------|----------|
| Manager | `manager@demo.local` | `Manager123!` |
| Employee | `employee@demo.local` | `Employee123!` |

Seeded leave types: Annual Leave (20), Sick Leave (10), Casual Leave (5).

## Features

- Employee CRUD (managers)
- Leave type CRUD (managers)
- Apply for leave / view requests (employees see own; managers see all)
- Approve / reject pending requests (managers)
- Dashboard counts: employees, total / pending / approved / rejected requests

## Business rules enforced

- Start date cannot be earlier than today
- End date must be on or after start date
- No overlapping leave requests (pending or approved)
- Requested days cannot exceed leave type maximum
- Only pending requests can be approved or rejected
- Approved / rejected requests cannot be edited

## EF Core migrations

```bash
dotnet ef migrations add <Name> \
  --project EmployeeLeaveManagement.Infrastructure \
  --startup-project EmployeeLeaveManagement.Web \
  --output-dir Persistence/Migrations

dotnet ef database update \
  --project EmployeeLeaveManagement.Infrastructure \
  --startup-project EmployeeLeaveManagement.Web
```
