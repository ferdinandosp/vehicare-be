using Vehicare.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services using extension methods
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddGraphQLServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map GraphQL endpoint
app.MapGraphQL("/graphql");

await app.RunAsync();
