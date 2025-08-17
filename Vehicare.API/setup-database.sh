#!/bin/bash

# PostgreSQL Database Setup Script for Vehicare

echo "🚗 Setting up Vehicare PostgreSQL Database..."

# Variables
DB_NAME="vehicare-db"
DB_USER="postgres"
DB_HOST="localhost"
DB_PORT="5432"

# Function to check if PostgreSQL is running
check_postgres() {
    if ! pg_isready -h $DB_HOST -p $DB_PORT -U $DB_USER >/dev/null 2>&1; then
        echo "❌ PostgreSQL is not running or not accessible."
        echo "Please make sure PostgreSQL is installed and running on $DB_HOST:$DB_PORT"
        echo ""
        echo "To install PostgreSQL:"
        echo "  macOS: brew install postgresql"
        echo "  Ubuntu: sudo apt-get install postgresql postgresql-contrib"
        echo "  Windows: Download from https://www.postgresql.org/download/"
        echo ""
        echo "To start PostgreSQL:"
        echo "  macOS: brew services start postgresql"
        echo "  Ubuntu: sudo systemctl start postgresql"
        echo "  Windows: Start the PostgreSQL service"
        exit 1
    fi
    echo "✅ PostgreSQL is running"
}

# Function to create database
create_database() {
    echo "📁 Creating database '$DB_NAME'..."

    # Check if database already exists
    if psql -h $DB_HOST -p $DB_PORT -U $DB_USER -lqt | cut -d \| -f 1 | grep -qw $DB_NAME; then
        echo "⚠️  Database '$DB_NAME' already exists"
        read -p "Do you want to drop and recreate it? (y/N): " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Yy]$ ]]; then
            echo "🗑️  Dropping existing database..."
            dropdb -h $DB_HOST -p $DB_PORT -U $DB_USER $DB_NAME
            echo "📁 Creating new database..."
            createdb -h $DB_HOST -p $DB_PORT -U $DB_USER $DB_NAME
            echo "✅ Database '$DB_NAME' recreated successfully"
        else
            echo "📁 Using existing database '$DB_NAME'"
        fi
    else
        createdb -h $DB_HOST -p $DB_PORT -U $DB_USER $DB_NAME
        echo "✅ Database '$DB_NAME' created successfully"
    fi
}

# Function to run migrations
run_migrations() {
    echo "🔄 Running Entity Framework migrations..."
    dotnet ef database update
    if [ $? -eq 0 ]; then
        echo "✅ Migrations applied successfully"
    else
        echo "❌ Failed to apply migrations"
        exit 1
    fi
}

# Function to verify database setup
verify_setup() {
    echo "🔍 Verifying database setup..."

    # Check if tables were created
    TABLES=$(psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';")

    if [ $TABLES -gt 0 ]; then
        echo "✅ Database tables created successfully"
        echo "📊 Found $TABLES tables in the database"

        # List the tables
        echo "📋 Tables created:"
        psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME -c "\dt"
    else
        echo "❌ No tables found in the database"
        exit 1
    fi
}

# Main execution
echo "🚀 Starting database setup..."
echo "Database: $DB_NAME"
echo "Host: $DB_HOST:$DB_PORT"
echo "User: $DB_USER"
echo ""

check_postgres
create_database
run_migrations
verify_setup

echo ""
echo "🎉 Database setup completed successfully!"
echo ""
echo "📝 Connection details:"
echo "  Host: $DB_HOST"
echo "  Port: $DB_PORT"
echo "  Database: $DB_NAME"
echo "  Username: $DB_USER"
echo ""
echo "🔧 You can now start the application with: dotnet run"
