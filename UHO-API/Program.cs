using Scalar.AspNetCore;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Extensions.ConfigurationExtensions;
using UHO_API.Extensions.ModelsExtensions;
using UHO_API.Features.Area.Endpoints;
using UHO_API.Features.Authentication.Endpoints;
using UHO_API.Features.Indicador.Endpoints;
using UHO_API.Features.Proceso.Endpoints;
using UHO_API.Features.Users.Endpoints;
using UHO_API.Infraestructure.Repository;
using UHO_API.Infraestructure.Sedeer;
using UHO_API.Infraestructure.Services;
using UHO_API.Infraestructure.Settings;
using UHO_API.Shared.Constants.SD;
using UHO_API.Shared.Mediator;
using IMediator = UHO_API.Core.Interfaces.IMediator;


var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<DefaultAdminUserSettings>(builder.Configuration.GetSection("DefaultAdminUser"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();


// Validation Extensions
builder.Services.AddValidationConfiguration();

builder.Services.AddIdentityConfiguration();


// --- Autenticación JWT ---
builder.Services.AddJwtAuthentication(jwtSettings);

// DataBase Configuration
builder.Services.AddDbConfiguration();

// MediatR Pattern Configuration
builder.Services.AddSingleton<IMediator, Mediator>();

// Auth Commands 
builder.Services.AddAuthCommandsConfiguration();


        // AREAS Commands and Queries

// Queries
builder.Services.AddAreaQueriesConfiguration();
// Commands
builder.Services.AddAreaCommandsConfiguration();


        // USER Commands and Queries

// Queries 
builder.Services.AddUserQueriesConfiguration();
// Command
builder.Services.AddUserCommandsConfiguration();

        // INDICADORES  Commands and Queries 

// Queries
builder.Services.AddAreaQueriesConfiguration();
// Commands
builder.Services.AddAreaCommandsConfiguration();


        // PROCESOS Commands and Queries
// Queries
builder.Services.AddProcesoQueriesConfiguration();
// Commands
builder.Services.AddProcesoCommandsConfiguration();


// Extra Services
// UnitOfWorks
builder.Services.AddScoped<IUnitOfWorks, UnitOfWorks>();

// Role Service
builder.Services.AddScoped<IRoleChangesService, RoleChangesService>();

// EvaluationService
builder.Services.AddScoped(typeof(IEvaluacionService<>), typeof(EvaluacionService<>));

builder.Services.AddScoped<IJwtService, JwtService>();



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();


app.MapAuthenticationEndpoints();
app.MapAreaEndpoints();
app.MapUserEndpoints();
app.MapIndicadorEndpoints();
app.MapProcesoEndpoints();


await app.AddDataSeeding();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var urls = app.Urls;
    foreach (var url in urls)
    {
        Console.WriteLine($"Scalar UI -> {url}/scalar");
    }
});

app.Run();

