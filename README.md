# 🚗 Vehicle Maintenance Tracker API

A GraphQL API to track vehicle maintenance schedules and logs. Built with ASP.NET Core and PostgreSQL.

## Features

- Register and manage vehicles
- Define maintenance schedules (by mileage or time)
- Log completed tasks with cost and mileage
- View upcoming and overdue tasks
- Export history for analysis

## Tech Stack

- ASP.NET Core 8
- PostgreSQL + EF Core
- HotChocolate GraphQL
- Optional: Docker, Serilog, FluentValidation

## Setup

1. Clone repo
2. Update `appsettings.Development.json` with your DB connection string
3. Run migrations:
```bash
dotnet ef database update
```

# Folder structures
VehicleMaintenance.Api/
├── GraphQL/
│   ├── Queries/
│   │   └── VehicleQuery.cs
│   ├── Mutations/
│   │   └── VehicleMutation.cs
│   ├── Types/
│   │   └── VehicleType.cs
│   └── Inputs/
│       └── CreateVehicleInput.cs
│       └── LogMaintenanceInput.cs
│
├── Data/
│   ├── AppDbContext.cs
│   └── Migrations/
│       └── <EF Migrations>
│
├── Models/
│   ├── Vehicle.cs
│   ├── MaintenanceSchedule.cs
│   └── MaintenanceLog.cs
│
├── Services/
│   └── VehicleService.cs
│   └── MaintenanceService.cs
│
├── Validators/
│   └── CreateVehicleInputValidator.cs
│
├── Configuration/
│   └── DependencyInjection.cs
│
├── appsettings.json
├── Program.cs
└── VehicleMaintenance.Api.csproj
