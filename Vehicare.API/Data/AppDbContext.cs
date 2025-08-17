using Microsoft.EntityFrameworkCore;
using Vehicare.API.Models;

namespace Vehicare.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasAnnotation("Collation", "SQL_Latin1_General_CP1_CI_AS");

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.LastLoginAt);

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Create unique index on email
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");
        });

        // Configure Vehicle entity
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Make)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Model)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Year)
                .IsRequired();

            entity.Property(e => e.VIN)
                .IsRequired()
                .HasMaxLength(17)
                .IsFixedLength();

            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20);

            entity.Property(e => e.CurrentMileage)
                .IsRequired();

            entity.Property(e => e.Color)
                .HasMaxLength(30);

            entity.Property(e => e.PurchaseDate)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Create unique index on VIN
            entity.HasIndex(e => e.VIN)
                .IsUnique()
                .HasDatabaseName("IX_Vehicles_VIN");

            // Configure relationship with User
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Vehicles_Users");

            // Create index on UserId for performance
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_Vehicles_UserId");
        });
    }
}
