using Microsoft.AspNetCore.Identity;
using UHO_API.Features.Area.Dto;
using UHO_API.Features.Area.Mappings;
using UHO_API.Interfaces;
using UHO_API.Interfaces.IRepository;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Area.Command;

public record UpdateAreaCommand(int Id, string Nombre, string? JefeAreaId) : IRequest<AreaResponse>;

public class UpdateAreaHandler : IRequestHandler<UpdateAreaCommand, AreaResponse>
{
    private readonly IUnitOfWorks _uow;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleChangesService _roleChangesService;

    public UpdateAreaHandler(IUnitOfWorks uow, UserManager<ApplicationUser> userManager, IRoleChangesService roleChangesService)
    {
        _uow = uow;
        _userManager = userManager;
        _roleChangesService = roleChangesService;
    }

    public async Task<Result<AreaResponse>> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
    {
        AreaModel? areaToUpdate = await _uow.Area.GetById(request.Id);
        
        if (areaToUpdate is null)
            return Error.NotFound("Área no encontrada.");

        string? oldJefeId = areaToUpdate.JefeAreaId;

        //var user = await _userManager.FindByIdAsync(request.JefeAreaId);
        
        var duplicateArea = await _uow.Area.Get(a => a.Nombre == request.Nombre && a.Id != request.Id);
       
        if (duplicateArea is not null)
            return Error.Validation("Otra área ya tiene ese nombre.");
            
        // Gestion de Roles
        if (oldJefeId != request.JefeAreaId)
        {
            if (oldJefeId is not null)
            {
                var esJefeDeOtraArea = await _uow.Area.Get(a=>a.JefeAreaId == oldJefeId && a.Id != request.Id);

                if (esJefeDeOtraArea is null)
                {
                    await _roleChangesService.DemoteToUsuarioNormalAsync(oldJefeId);
                }
            }
            
            if (request.JefeAreaId is not null)
            {
                var newJefeExists = await _userManager.FindByIdAsync(request.JefeAreaId);
               
                if (newJefeExists is null)
                    return Error.NotFound("El nuevo usuario asignado como Jefe de Área no fue encontrado.");
                
                await _roleChangesService.PromoteToJefeAreaAsync(request.JefeAreaId);
            }
        
        }

        areaToUpdate.Nombre = request.Nombre;
        areaToUpdate.JefeAreaId = request.JefeAreaId;

        _uow.Area.Update(areaToUpdate);
        await _uow.SaveChangesAsync();
        
        
        return new AreaResponse(
            areaToUpdate.Id,
            areaToUpdate.Nombre,
            areaToUpdate.JefeAreaId,
            areaToUpdate.JefeArea?.FullName ?? "No Asignado"
            );
    }
}