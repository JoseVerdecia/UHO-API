using Microsoft.AspNetCore.Identity;
using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Area.Dtos;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Area.Commands;

public record CreateAreaCommand(string Nombre, string? JefeAreaId) : IRequest<AreaResponse>;

public class CreateAreaHandler : IRequestHandler<CreateAreaCommand, AreaResponse>
{
    private readonly IUnitOfWorks _uow;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleChangesService _roleChangesService;

    public CreateAreaHandler(IUnitOfWorks uow, UserManager<ApplicationUser> userManager, IRoleChangesService roleChangesService)
    {
        _uow = uow;
        _userManager = userManager;
        _roleChangesService = roleChangesService;
    }

    public async Task<Result<AreaResponse>> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
    {
        
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return Result.Failure<AreaResponse>(
                Error.Validation("Nombre", "El nombre del área es requerido")
            );
        }
        
        var existingArea = await _uow.Area.Get(a => a.Nombre == request.Nombre);
        
        if (existingArea is not null)
        {
            return Result.Failure<AreaResponse>(
                Error.Conflict("Area", "Nombre", request.Nombre)
            );
        }

        ApplicationUser? jefeArea = null;
        
        if (!string.IsNullOrWhiteSpace(request.JefeAreaId))
        {
            jefeArea = await _userManager.FindByIdAsync(request.JefeAreaId);
            
            if (jefeArea is null)
            {
                return Result.Failure<AreaResponse>(
                    Error.NotFound("Usuario", request.JefeAreaId)
                );
            }
        }
        
        AreaModel newArea = new()
        {
            Nombre = request.Nombre,
            JefeAreaId = jefeArea?.Id,
            CreatedAt = DateTime.UtcNow
        };
        
        if (jefeArea is not null)
        {
            newArea.JefeArea = jefeArea;
            
            try
            {
                await _roleChangesService.PromoteToJefeAreaAsync(jefeArea.Id);
            }
            catch (Exception ex)
            {
                return Result.Failure<AreaResponse>(
                    Error.Business("PromotionFailed", $"No se pudo promover al usuario: {ex.Message}")
                );
            }
        }
        
        try
        {
            _uow.Area.Add(newArea);
            await _uow.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Result.Failure<AreaResponse>(
                Error.Failure("DatabaseError", $"Error al guardar el área: {ex.Message}")
            );
        }
        
        var response = new AreaResponse(
            newArea.Id,
            newArea.Nombre,
            newArea.JefeAreaId ?? "No Asignado",
            newArea.JefeArea?.FullName ?? "No Asignado",
            newArea.JefeArea?.Email ?? "N/A"
        );
      
        return Result.Success(response);
    }
}