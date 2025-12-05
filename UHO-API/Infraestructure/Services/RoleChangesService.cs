using Microsoft.AspNetCore.Identity;
using UHO_API.Core.Entities;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;
using UHO_API.Shared.Results;


namespace UHO_API.Infraestructure.Services;

public class RoleChangesService : IRoleChangesService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RoleChangesService> _logger;

    public RoleChangesService(
        UserManager<ApplicationUser> userManager,
        ILogger<RoleChangesService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> PromoteToJefeAreaAsync(string userId)
    {   
        
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                _logger.LogWarning("Usuario {UserId} no encontrado para promover a Jefe de Área", userId);
                return Result.Failure(Error.NotFound("Usuario", userId));
            }
            
            if (user.IsDeleted)
            {
                return Result.Failure(Error.Business("UserDeleted", 
                    "No se puede promover a un usuario eliminado"));
            }
            
          
            if (await _userManager.IsInRoleAsync(user, Roles.JefeArea))
            {
                _logger.LogInformation("Usuario {UserId} ya tiene rol Jefe de Área", userId);
                return Result.Success("El Usuario ya tiene el Rol: 'JefeArea'");
            }
            

            if (await _userManager.IsInRoleAsync(user, Roles.JefeProceso))
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, Roles.JefeProceso);
                if (!removeResult.Succeeded)
                {
                    _logger.LogError("Error al remover rol JefeProceso del usuario {UserId}", userId);
                    return Result.Failure(Error.Failure(ErrorCodes.RoleRemoveFailed,"No se puedo quitar el Rol: 'JefeProceso'"));
                }
                _logger.LogInformation("Rol JefeProceso removido del usuario {UserId}", userId);
            }
            
            
            if (await _userManager.IsInRoleAsync(user, Roles.UsuarioNormal))
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, Roles.UsuarioNormal);
                if (!removeResult.Succeeded)
                {
                    _logger.LogError("Error al remover rol UsuarioNormal del usuario {UserId}", userId);
                    return Result.Failure(Error.Failure(ErrorCodes.RoleRemoveFailed,"No se pudo quitar el Rol: 'UsuarioNormal'"));;
                }
            }
            
            
            var result = await _userManager.AddToRoleAsync(user, Roles.JefeArea);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario {UserId} promovido exitosamente a Jefe de Área", userId);
                return Result.Success("Usuario promovido exitosamente a Jefe de Area");
            }
            
            _logger.LogError("Error al agregar rol JefeArea al usuario {UserId}: {Errors}", 
                userId, string.Join(", ", result.Errors.Select(e => e.Description)));
            return Result.Failure(Error.Failure(ErrorCodes.RoleAddFailed,"No se puedo agregar el Rol: 'JefeArea'"));
       
    }

    public async Task<Result> PromoteToJefeProcesoAsync(string userId)
    {
        
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                _logger.LogWarning("Usuario {UserId} no encontrado para promover a Jefe de Proceso", userId);
                return Result.Failure(Error.NotFound("Usuario", userId));
            }
            
            if (user.IsDeleted)
            {
                return Result.Failure(Error.Business(ErrorCodes.UserDeleted, 
                    "No se puede promover a un usuario eliminado"));
            }

            if (await _userManager.IsInRoleAsync(user, Roles.JefeProceso))
            {
                _logger.LogInformation("Usuario {UserId} ya tiene rol Jefe de Proceso", userId);
                return Result.Success("El Usuario ya tiene el Rol: 'JefeProceso'");
            }
            
       
            if (await _userManager.IsInRoleAsync(user, Roles.JefeArea))
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, Roles.JefeArea);
                if (!removeResult.Succeeded)
                {
                    _logger.LogError("Error al remover rol JefeArea del usuario {UserId}", userId);
                    return Result.Failure(Error.Failure(ErrorCodes.RoleRemoveFailed,"No se pudo quitar el Rol: 'JefeArea'"));
                }
                _logger.LogInformation("Rol JefeArea removido del usuario {UserId}", userId);
            }
            
           
            if (await _userManager.IsInRoleAsync(user, Roles.UsuarioNormal))
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, Roles.UsuarioNormal);
                if (!removeResult.Succeeded)
                {
                    _logger.LogError("Error al remover rol UsuarioNormal del usuario {UserId}", userId);
                    return Result.Failure(Error.Failure(ErrorCodes.RoleRemoveFailed,"No se pudo quitar el Rol: 'UsuarioNormal'"));
                }
            }
            
            var result = await _userManager.AddToRoleAsync(user, Roles.JefeProceso);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario {UserId} promovido exitosamente a Jefe de Proceso", userId);
                return Result.Success("Usuario promovido exitosamente a Jefe de Proceso");
            }
            
            _logger.LogError("Error al agregar rol JefeProceso al usuario {UserId}: {Errors}", 
                userId, string.Join(", ", result.Errors.Select(e => e.Description)));
            return  Result.Failure(Error.Failure(ErrorCodes.RoleAddFailed,"No se pudo agregar el Rol: 'JefeArea'"));
       
    }

    public async Task<Result> DemoteToUsuarioNormalAsync(string userId)
    {
       
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                _logger.LogWarning("Usuario {UserId} no encontrado para demover a Usuario Normal", userId);
                return Result.Failure(Error.NotFound(ErrorCodes.UserNotFound, userId));;
            }

           
            if (await _userManager.IsInRoleAsync(user, Roles.UsuarioNormal) && 
                !await _userManager.IsInRoleAsync(user, Roles.JefeArea) && 
                !await _userManager.IsInRoleAsync(user, Roles.JefeProceso))
            {
                _logger.LogInformation("Usuario {UserId} ya tiene rol UsuarioNormal sin roles de jefe", userId);
                return Result.Success();
            }

            
            var tasks = new List<Task<IdentityResult>>();
            
            if (await _userManager.IsInRoleAsync(user, Roles.JefeArea))
            {
                tasks.Add(_userManager.RemoveFromRoleAsync(user, Roles.JefeArea));
            }
            
            if (await _userManager.IsInRoleAsync(user, Roles.JefeProceso))
            {
                tasks.Add(_userManager.RemoveFromRoleAsync(user, Roles.JefeProceso));
            }
            
            if (tasks.Any())
            {
                var results = await Task.WhenAll(tasks);
                if (results.Any(r => !r.Succeeded))
                {
                    _logger.LogError("Error al remover roles de jefe del usuario {UserId}", userId);
                    return Result.Failure(Error.Failure(ErrorCodes.RoleRemoveFailed,"No se pudo quitar los roles del usuario"));
                }
                _logger.LogInformation("Roles de jefe removidos del usuario {UserId}", userId);
            }
            
          
            if (!await _userManager.IsInRoleAsync(user, Roles.UsuarioNormal))
            {
                var result = await _userManager.AddToRoleAsync(user, Roles.UsuarioNormal);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario {UserId} demovido exitosamente a Usuario Normal", userId);
                    return Result.Success();
                }
                
                _logger.LogError("Error al agregar rol UsuarioNormal al usuario {UserId}: {Errors}", 
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return Result.Failure(Error.Failure(ErrorCodes.RoleAddFailed,"No se pudo agregar el Rol: 'UsuarioNormal'"));;
            }

            return Result.Success("Usuario demovido exitosamente a UsuarioNormal");
        
    }
}