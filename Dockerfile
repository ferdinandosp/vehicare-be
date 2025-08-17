# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy solution file
COPY vehicare-be.sln ./

# Copy project files
COPY Vehicare.API/Vehicare.API.csproj Vehicare.API/
COPY Vehicare.API.Tests/Vehicare.API.Tests.csproj Vehicare.API.Tests/

# Restore NuGet packages
RUN dotnet restore vehicare-be.sln

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR /src/Vehicare.API
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 80
EXPOSE 443

# Set the entry point
ENTRYPOINT ["dotnet", "Vehicare.API.dll"]
