using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using UHO_API.Core.Entities;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Area.Commands;
using UHO_API.Features.Area.Dtos;
using UHO_API.Features.Area.Endpoints;
using UHO_API.Features.Area.Queries;
using UHO_API.Features.Area.Validations;
using UHO_API.Features.Authentication;
using UHO_API.Features.Authentication.Dtos;
using UHO_API.Features.Authentication.Endpoints;
using UHO_API.Features.Authentication.Queries;
using UHO_API.Features.Authentication.Validations;
using UHO_API.Features.Users;
using UHO_API.Features.Users.Commands;
using UHO_API.Features.Users.Endpoints;
using UHO_API.Features.Users.Queries;
using UHO_API.Features.Users.Validations;
using UHO_API.Infraestructure.Data;
using UHO_API.Infraestructure.Repository;
using UHO_API.Infraestructure.Services;
using UHO_API.Infraestructure.Settings;
using UHO_API.Shared.Constants.SD;
using UHO_API.Shared.Mediator;
using IMediator = UHO_API.Core.Interfaces.IMediator;


var builder = WebApplication.CreateBuilder(args);


// --- 1. Configuración ---

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<DefaultAdminUserSettings>(builder.Configuration.GetSection("DefaultAdminUser"));
builder.Services.AddScoped<IJwtService, JwtService>();


//builder.Services.JwtConfiguration(builder.Configuration);
//builder.Services.AddAuthenticationConfiguration(builder.Configuration);

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AreaModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DegradeUserValidator>();


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// --- 3. Autenticación JWT ---

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings!.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.SecretKey))
    };
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("AuthDb");
    options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
});

// --- 4. Mediator y Handlers ---
//builder.Services.AddMediatorHandlersConfiguration();

builder.Services.AddSingleton<IMediator, Mediator>();

// Auth
builder.Services.AddScoped<IRequestHandler<RegisterRequest, AuthenticationResponse>, RegisterHandler>();
builder.Services.AddScoped<IRequestHandler<LoginRequest, AuthenticationResponse>, LoginHandler>();
builder.Services.AddScoped<IRequestHandler<RefreshTokenRequest, AuthenticationResponse>, RefreshTokenHandler>();

// Areas
builder.Services.AddScoped<IRequestHandler<GetAllAreaQuery, IEnumerable<AreaResponse>>, GetAllAreaHandler>();
builder.Services.AddScoped<IRequestHandler<GetAreaByIdQuery, AreaResponse>, GetAreaByIdHandler>();
builder.Services.AddScoped<IRequestHandler<CreateAreaCommand, AreaResponse>, CreateAreaHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateAreaCommand, AreaResponse>, UpdateAreaHandler>();
builder.Services.AddScoped<IRequestHandler<SoftDeleteAreaCommand, bool>, SoftDeleteAreaHandler>();
builder.Services.AddScoped<IRequestHandler<HardDeleteCommand, bool>, HardDeleteHandler>();
// User
builder.Services.AddScoped<IRequestHandler<GetUserQuery, ApplicationUser>, GetUserQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetAllUsersQuery, IEnumerable<ApplicationUser>>, GetAllUserQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetAllJefeAreasQuery, IEnumerable<ApplicationUser>>, GetAllJefeAreasQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetAllUsuariosNormalQuery, IEnumerable<ApplicationUser>>, GetAllUsuariosNormalQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetAllJefeProcesosQuery, IEnumerable<ApplicationUser>>, GetAllJefeProcesosQueryHandler>();
builder.Services.AddScoped<IRequestHandler<DegradeUserToUsuarioNormalCommand, ApplicationUser>, DegradeUserToUsuarioNormalHandler>();

builder.Services.AddScoped<IUnitOfWorks, UnitOfWorks>();
builder.Services.AddScoped<IRoleChangesService, RoleChangesService>();

// Add services to the container.
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


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // 1. Obtener los servicios necesarios
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var adminSettings = services.GetRequiredService<IOptions<DefaultAdminUserSettings>>().Value;

        // 2. Crear los roles (si no existen)
        var roles = new[] { 
            Roles.Administrador, 
            Roles.JefeArea, 
            Roles.JefeProceso, 
            Roles.UsuarioNormal 
        };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole(roleName));
                logger.LogInformation("Rol '{RoleName}' creado.", roleName);
            }
        }

        // 3. Crear el usuario Administrador (si no existe)
        var adminUser = await userManager.FindByEmailAsync(adminSettings.Email);

        if (adminUser == null)
        {
            // El usuario no existe, lo creamos
            adminUser = new ApplicationUser
            {
                UserName = adminSettings.Email,
                Email = adminSettings.Email,
                // Puedes establecer otras propiedades aquí, por ejemplo:
                 FullName = "Jose",
                // LastName = "User"
                EmailConfirmed = true // Marcar el email como confirmado para poder iniciar sesión
            };

            var createResult = await userManager.CreateAsync(adminUser, adminSettings.Password);

            if (createResult.Succeeded)
            {
                logger.LogInformation("Usuario administrador '{Email}' creado.", adminSettings.Email);
                
                // Asignarle el rol de Administrador
                await userManager.AddToRoleAsync(adminUser, Roles.Administrador);
                logger.LogInformation("Rol '{Role}' asignado al usuario '{Email}'.", Roles.Administrador, adminSettings.Email);
            }
            else
            {
                // Si falla la creación, logueamos los errores
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                logger.LogError("Error al crear el usuario administrador: {Errors}", errors);
            }
        }
        else
        {
            logger.LogInformation("El usuario administrador '{Email}' ya existe.", adminSettings.Email);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocurrió un error durante el seeding de datos iniciales.");
    }
}

app.Run();

