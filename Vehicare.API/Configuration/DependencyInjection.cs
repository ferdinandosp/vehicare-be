using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using FluentValidation;
using Vehicare.API.Data;
using Vehicare.API.Services;
using Vehicare.API.GraphQL.Queries;
using Vehicare.API.GraphQL.Mutations;
using Vehicare.API.GraphQL.Types;
using Vehicare.API.Validators;

namespace Vehicare.API.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Register application services (using database implementations)
        services.AddScoped<IUserService, DatabaseUserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IVehicleService, DatabaseVehicleService>();

        // Register validators
        services.AddValidatorsFromAssemblyContaining<RegisterInputValidator>();

        return services;
    }

    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure JWT Authentication
        var jwtSettings = configuration.GetSection("JWT");
        var secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-key-that-should-be-at-least-32-characters-long";
        var key = Encoding.UTF8.GetBytes(secretKey);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddGraphQLServices(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddQueryType()
                .AddTypeExtension<UserQuery>()
                .AddTypeExtension<VehicleQuery>()
            .AddMutationType()
                .AddTypeExtension<UserMutation>()
                .AddTypeExtension<VehicleMutation>()
            .AddType<UserType>()
            .AddType<VehicleType>()
            .AddType<LoginInputType>()
            .AddType<RegisterInputType>()
            .AddType<CreateVehicleInputType>()
            .AddType<UpdateVehicleInputType>()
            .AddType<LoginPayloadType>()
            .AddType<RegisterPayloadType>()
            .AddType<CreateVehiclePayloadType>()
            .AddType<UpdateVehiclePayloadType>()
            .AddType<DeleteVehiclePayloadType>();

        return services;
    }
}
