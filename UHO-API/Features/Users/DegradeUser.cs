using FluentValidation;
using Microsoft.AspNetCore.Identity;
using UHO_API.Infraestructure.SD;
using UHO_API.Interfaces;
using UHO_API.Interfaces.IRepository;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Users;
public record DegradeUserToUsuarioNormalCommand(string UserId) : IRequest<ApplicationUser>;

public class DegradeUserToUsuarioNormalHandler : IRequestHandler<DegradeUserToUsuarioNormalCommand, ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleChangesService _roleChangesService;
    private readonly IValidator<DegradeUserToUsuarioNormalCommand> _validator;
    private readonly IUnitOfWorks _uow;
    private readonly ILogger<DegradeUserToUsuarioNormalHandler> _logger;

    public DegradeUserToUsuarioNormalHandler(
        UserManager<ApplicationUser> userManager, 
        IRoleChangesService roleChangesService,
        IValidator<DegradeUserToUsuarioNormalCommand> validator,
        IUnitOfWorks uow,
        ILogger<DegradeUserToUsuarioNormalHandler> logger)
    {
        _userManager = userManager;
        _roleChangesService = roleChangesService;
        _validator = validator;
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<ApplicationUser>> Handle(
        DegradeUserToUsuarioNormalCommand request, 
        CancellationToken cancellationToken)
    {
    
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result.Failure<ApplicationUser>(
                Error.Validation("CommandValidation", string.Join("; ", errors))
            );
        }

   
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            return Result.Failure<ApplicationUser>(
                Error.NotFound(ErrorCodes.UserNotFound, request.UserId)
            );
        }
        
        if (user.IsDeleted)
        {
            return Result.Failure<ApplicationUser>(
                Error.Business("UserDeleted", "No se puede degradar a un usuario eliminado")
            );
        }

    
        var userRoles = await _userManager.GetRolesAsync(user);
        
     
        if (userRoles.Contains(Roles.UsuarioNormal) && 
            !userRoles.Contains(Roles.JefeArea) && 
            !userRoles.Contains(Roles.JefeProceso))
        {
            _logger.LogInformation("Usuario {UserId} ya tiene rol UsuarioNormal sin roles de jefe", request.UserId);
            return Result.Success(user);
        }

        try
        {
            using var transaction = await _uow.BeginTransactionAsync();
            
            try
            {
                // Si es JefeArea, buscar y desasignar el área
                if (userRoles.Contains(Roles.JefeArea))
                {
                    var area = await _uow.Area.Get(a => a.JefeAreaId == user.Id && !a.IsDeleted);
                    
                    if (area is not null)
                    {
                        area.JefeAreaId = null;
                        area.JefeArea = null;
                        area.UpdatedAt = DateTime.UtcNow;
                        _uow.Area.Update(area);
                        
                        _logger.LogInformation("Desasignado usuario {UserId} del área {AreaId}", 
                            user.Id, area.Id);
                    }
                    else
                    {
                        _logger.LogWarning("Usuario {UserId} tiene rol JefeArea pero no está asignado a un área activa", 
                            user.Id);
                    }
                }
                
                // Si es JefeProceso, buscar y desasignar procesos (Implementar mas Adelante cuando haga ProcesoModel)
                
                var demotionResult = await _roleChangesService.DemoteToUsuarioNormalAsync(request.UserId);
                if (demotionResult.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return Result.Failure<ApplicationUser>(
                        Error.Failure("RoleChangeFailed", 
                            "No se pudo modificar los roles del usuario")
                    );
                }
                
           
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                
                await _uow.SaveChangesAsync();
                
                await transaction.CommitAsync();
                
                _logger.LogInformation("Usuario {UserId} degradado exitosamente a UsuarioNormal",request.UserId);
                
                
                var updatedUser = await _userManager.FindByIdAsync(request.UserId);
                return Result.Success(updatedUser!);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error durante la degradación del usuario {UserId}", request.UserId);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al degradar usuario {UserId} a UsuarioNormal", request.UserId);
            return Result.Failure<ApplicationUser>(
                Error.Failure("DegradeFailed", 
                    $"Error al degradar usuario: {ex.Message}")
            );
        }
    }
}