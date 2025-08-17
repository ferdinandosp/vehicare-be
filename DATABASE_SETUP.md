# 🗄️ Database Setup Guide

This guide will help you set up PostgreSQL for the Vehicare API.

## 📋 Prerequisites

- .NET 8 SDK
- PostgreSQL (see installation options below)

## 🛠️ Option 1: Using Docker (Recommended)

### 1. Install Docker
- **macOS**: Download Docker Desktop from [docker.com](https://www.docker.com/products/docker-desktop)
- **Windows**: Download Docker Desktop from [docker.com](https://www.docker.com/products/docker-desktop)
- **Linux**: Follow instructions at [docs.docker.com](https://docs.docker.com/engine/install/)

### 2. Start PostgreSQL with Docker Compose
```bash
# From the project root directory
docker-compose up -d postgres

# This will:
# - Download PostgreSQL 15 image
# - Create a database named 'vehicare-db'
# - Set up user 'postgres' with password 'postgres'
# - Expose PostgreSQL on port 5432
```

### 3. Optional: Start PgAdmin (Database Management UI)
```bash
# Start PgAdmin alongside PostgreSQL
docker-compose up -d

# Access PgAdmin at: http://localhost:5050
# Email: admin@vehicare.com
# Password: admin
```

### 4. Run Database Migrations
```bash
cd Vehicare.API
./setup-database.sh
# or manually:
dotnet ef database update
```

## 🛠️ Option 2: Local PostgreSQL Installation

### 1. Install PostgreSQL

#### macOS (using Homebrew)
```bash
brew install postgresql
brew services start postgresql
```

#### Ubuntu/Debian
```bash
sudo apt-get update
sudo apt-get install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql
```

#### Windows
1. Download PostgreSQL from [postgresql.org](https://www.postgresql.org/download/windows/)
2. Run the installer
3. Remember the password you set for the 'postgres' user
4. Start the PostgreSQL service

### 2. Create Database
```bash
# Connect to PostgreSQL
sudo -u postgres psql

# Create database (in PostgreSQL prompt)
CREATE DATABASE "vehicare-db";

# Create user (optional, or use existing postgres user)
CREATE USER vehicare_user WITH PASSWORD 'your_password';
GRANT ALL PRIVILEGES ON DATABASE "vehicare-db" TO vehicare_user;

# Exit PostgreSQL
\q
```

### 3. Update Connection String
Update `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=vehicare-db;Username=postgres;Password=your_password"
  }
}
```

### 4. Run Database Setup
```bash
cd Vehicare.API
./setup-database.sh
```

## 🔧 Manual Migration Commands

If you prefer to run migrations manually:

```bash
# Navigate to the API project
cd Vehicare.API

# Apply migrations to database
dotnet ef database update

# Create a new migration (when you modify models)
dotnet ef migrations add MigrationName

# Remove last migration (if not applied to database)
dotnet ef migrations remove

# Generate SQL script for migrations
dotnet ef migrations script

# Drop database (careful!)
dotnet ef database drop
```

## 📊 Database Schema

The initial migration creates the following tables:

### Users Table
```sql
CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Email" VARCHAR(255) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(255) NOT NULL,
    "FirstName" VARCHAR(100) NOT NULL,
    "LastName" VARCHAR(100) NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "LastLoginAt" TIMESTAMP WITH TIME ZONE,
    "IsActive" BOOLEAN NOT NULL DEFAULT true
);
```

### Vehicles Table
```sql
CREATE TABLE "Vehicles" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "Make" VARCHAR(50) NOT NULL,
    "Model" VARCHAR(50) NOT NULL,
    "Year" INTEGER NOT NULL,
    "VIN" CHAR(17) NOT NULL UNIQUE,
    "LicensePlate" VARCHAR(20),
    "CurrentMileage" INTEGER NOT NULL,
    "Color" VARCHAR(30),
    "PurchaseDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL
);
```

### Indexes
- `IX_Users_Email` - Unique index on user email
- `IX_Vehicles_VIN` - Unique index on vehicle VIN
- `IX_Vehicles_UserId` - Index for user-vehicle relationship

## 🔍 Verifying the Setup

### Check Database Connection
```bash
# Test connection using psql
psql -h localhost -p 5432 -U postgres -d vehicare-db

# List tables
\dt

# Check table structure
\d "Users"
\d "Vehicles"
```

### Check Application Connection
```bash
# Start the application
cd Vehicare.API
dotnet run

# The application should start without database errors
# Check logs for successful database connection
```

## 🐛 Troubleshooting

### Common Issues

1. **PostgreSQL not running**
   ```bash
   # Check if PostgreSQL is running
   pg_isready -h localhost -p 5432 -U postgres

   # Start PostgreSQL (macOS)
   brew services start postgresql

   # Start PostgreSQL (Ubuntu)
   sudo systemctl start postgresql
   ```

2. **Connection refused**
   - Check if PostgreSQL is listening on port 5432
   - Verify username and password in connection string
   - Check firewall settings

3. **Database does not exist**
   ```bash
   # Create database manually
   createdb -h localhost -p 5432 -U postgres vehicare-db
   ```

4. **Migration errors**
   ```bash
   # Check if migrations folder exists
   ls Migrations/

   # Recreate migrations if needed
   dotnet ef migrations remove
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Permission denied**
   ```bash
   # Grant permissions to user
   sudo -u postgres psql
   GRANT ALL PRIVILEGES ON DATABASE "vehicare-db" TO postgres;
   ```

## 🔄 Reset Database

To completely reset the database:

```bash
# Using Docker
docker-compose down -v
docker-compose up -d postgres

# Using local PostgreSQL
dropdb -h localhost -p 5432 -U postgres vehicare-db
createdb -h localhost -p 5432 -U postgres vehicare-db

# Run migrations
cd Vehicare.API
dotnet ef database update
```

## 📚 Additional Resources

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Npgsql Entity Framework Core Provider](https://www.npgsql.org/efcore/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
