using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UHO_API.Core.Entities;
using UHO_API.Core.Enums;
using UHO_API.Shared.Constants.SD;

namespace UHO_API.Infraestructure.Sedeer;

public static class DataSeeding
{
    public static async Task AddDataSeeding(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
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
    }
}
