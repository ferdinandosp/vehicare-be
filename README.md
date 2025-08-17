# 🚗 Vehicle Maintenance Tracker API

A GraphQL API to track vehicle maintenance schedules and logs. Built with ASP.NET Core, PostgreSQL, and HotChocolate GraphQL.

## Features

- **User Management**: Register, login, and JWT authentication
- **Vehicle Management**: Add, update, delete, and view vehicles
- **Database Integration**: PostgreSQL with Entity Framework Core
- **GraphQL API**: Complete GraphQL schema with queries and mutations
- **Input Validation**: FluentValidation for robust data validation
- **Security**: JWT-based authentication and user data isolation

## Tech Stack

- ASP.NET Core 8
- PostgreSQL + Entity Framework Core
- HotChocolate GraphQL
- FluentValidation
- BCrypt for password hashing
- JWT Authentication

## 🚀 Quick Start

### 1. Prerequisites
- .NET 8 SDK
- PostgreSQL (local installation or Docker)

### 2. Database Setup
Choose one of the following options:

#### Option A: Using Docker (Recommended)
```bash
# Start PostgreSQL with Docker
docker compose up -d postgres

# Run migrations
cd Vehicare.API
dotnet ef database update
```

#### Option B: Local PostgreSQL
1. Install PostgreSQL locally
2. Create database: `createdb vehicare-db`
3. Update connection string in `appsettings.Development.json`
4. Run migrations: `dotnet ef database update`

📖 **Detailed database setup instructions**: See [DATABASE_SETUP.md](DATABASE_SETUP.md)

### 3. Run the Application
```bash
cd Vehicare.API
dotnet run
```

The API will be available at:
- **GraphQL Playground**: `https://localhost:5006/graphql`
- **GraphQL Endpoint**: `https://localhost:5006/graphql`

## 📖 API Documentation

### Authentication

#### Register User
```graphql
mutation {
  register(input: {
    email: "user@example.com"
    password: "password123"
    firstName: "John"
    lastName: "Doe"
  }) {
    success
    user { id email firstName lastName }
    error
  }
}
```

#### Login
```graphql
mutation {
  login(input: {
    email: "user@example.com"
    password: "password123"
  }) {
    success
    token
    user { id email firstName lastName }
    error
  }
}
```

**Example Response:**
```json
{
  "data": {
    "login": {
      "success": true,
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "user": {
        "id": 1,
        "email": "user@example.com",
        "firstName": "John",
        "lastName": "Doe"
      },
      "error": null
    }
  }
}
```

**⚠️ Important**: Copy the `token` value and use it in the Authorization header for all future requests.

### Vehicle Management

All vehicle operations require authentication. You must include the JWT token in the Authorization header.

#### 🔐 Authentication Header Setup

After logging in, you'll receive a JWT token. Include it in all subsequent requests:

**HTTP Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

**GraphQL Playground Setup:**
1. Copy the token from the login response
2. In GraphQL Playground, click the "HTTP Headers" tab (bottom left)
3. Add the following JSON:
```json
{
  "Authorization": "Bearer YOUR_JWT_TOKEN_HERE"
}
```

**Example with curl:**
```bash
curl -X POST http://localhost:5006/graphql \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE" \
  -d '{"query": "query { myVehicles { id make model } }"}'
```

#### Create Vehicle
```graphql
mutation {
  createVehicle(input: {
    make: "Toyota"
    model: "Camry"
    year: 2022
    vin: "1HGBH41JXMN109186"
    licensePlate: "ABC123"
    currentMileage: 15000
    color: "Silver"
    purchaseDate: "2022-01-15T00:00:00.000Z"
  }) {
    success
    vehicle { id make model year vin currentMileage }
    error
  }
}
```

#### Get User's Vehicles
```graphql
query {
  myVehicles {
    id make model year vin licensePlate
    currentMileage color purchaseDate
    createdAt updatedAt
  }
}
```

#### Update Vehicle
```graphql
mutation {
  updateVehicle(input: {
    id: 1
    currentMileage: 16500
    color: "Blue"
  }) {
    success
    vehicle { id currentMileage color updatedAt }
    error
  }
}
```

## � Troubleshooting

### Authentication Issues

**Problem: "Access Denied" or "Unauthorized" when creating vehicles**
- **Solution**: Make sure you include the JWT token in the Authorization header
- **Format**: `Authorization: Bearer YOUR_JWT_TOKEN_HERE`
- **Check**: Token should start with `eyJ` and be quite long

**Problem: "Invalid token" errors**
- **Solution**: Make sure the token is not expired (check if you need to login again)
- **Solution**: Copy the entire token without extra spaces or characters

**Problem: GraphQL Playground not sending headers**
- **Solution**: Click "HTTP Headers" tab in GraphQL Playground
- **Solution**: Add the authorization header as JSON: `{"Authorization": "Bearer YOUR_TOKEN"}`

### Database Issues

**Problem: Connection errors**
- **Solution**: Verify PostgreSQL is running
- **Solution**: Check connection string in `appsettings.Development.json`
- **Solution**: Run `dotnet ef database update` to apply migrations

**Problem: "Database does not exist"**
- **Solution**: Create database: `createdb vehicare-db`
- **Solution**: Or use the setup script: `./setup-database.sh`

## �📁 Project Structure

```
Vehicare.API/
├── Configuration/
│   └── DependencyInjection.cs    # Service registration
├── Data/
│   └── AppDbContext.cs           # Entity Framework context
├── GraphQL/
│   ├── Queries/
│   │   ├── UserQuery.cs          # User-related queries
│   │   └── VehicleQuery.cs       # Vehicle-related queries
│   ├── Mutations/
│   │   ├── UserMutation.cs       # User-related mutations
│   │   └── VehicleMutation.cs    # Vehicle-related mutations
│   ├── Types/
│   │   ├── GraphQLTypes.cs       # User GraphQL types
│   │   └── VehicleTypes.cs       # Vehicle GraphQL types
│   └── Inputs/
│       ├── AuthInputs.cs         # Authentication inputs
│       └── VehicleInputs.cs      # Vehicle inputs
├── Migrations/                   # Entity Framework migrations
├── Models/
│   ├── User.cs                   # User entity
│   ├── Vehicle.cs                # Vehicle entity
│   ├── AuthModels.cs             # Authentication DTOs
│   └── VehicleModels.cs          # Vehicle DTOs
├── Services/
│   ├── DatabaseUserService.cs   # User service (database)
│   ├── DatabaseVehicleService.cs # Vehicle service (database)
│   └── JwtService.cs             # JWT token service
├── Validators/
│   ├── AuthInputValidators.cs    # Authentication validation
│   └── VehicleInputValidators.cs # Vehicle validation
├── Program.cs                    # Application entry point
└── setup-database.sh             # Database setup script
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
