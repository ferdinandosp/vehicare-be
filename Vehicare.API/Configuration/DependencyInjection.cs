using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
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
        // Register application services
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IJwtService, JwtService>();

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
            .AddQueryType<UserQuery>()
            .AddMutationType<UserMutation>()
            .AddType<UserType>()
            .AddType<LoginInputType>()
            .AddType<RegisterInputType>()
            .AddType<LoginPayloadType>()
            .AddType<RegisterPayloadType>();

        return services;
    }
}
