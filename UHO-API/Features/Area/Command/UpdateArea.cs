using Microsoft.AspNetCore.Identity;
using UHO_API.Features.Area.Dto;
using UHO_API.Features.Area.Mappings;
using UHO_API.Infraestructure.SD;
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

        public UpdateAreaHandler(IUnitOfWorks uow, UserManager<ApplicationUser> userManager,
            IRoleChangesService roleChangesService)
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
            
            var duplicateArea = await _uow.Area.Get(a => a.Nombre == request.Nombre && a.Id != request.Id);
            if (duplicateArea is not null)
                return Error.Validation("Otra área ya tiene ese nombre.");

            ApplicationUser? newJefe = null;
        
            if (!string.IsNullOrEmpty(request.JefeAreaId))
            {
                newJefe = await _userManager.FindByIdAsync(request.JefeAreaId);
                if (newJefe is null)
                    return Error.NotFound("El nuevo usuario asignado como Jefe de Área no fue encontrado.");

               
                if (!await _userManager.IsInRoleAsync(newJefe, Roles.UsuarioNormal))
                {
                    return Error.Validation(
                        "Solo un usuario con el rol 'UsuarioNormal' puede ser asignado como Jefe de Área.");
                }
            }
            

            using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                string? oldJefeId = areaToUpdate.JefeAreaId;

              
                if (oldJefeId is not null && oldJefeId != request.JefeAreaId)
                {
                    await _roleChangesService.DemoteToUsuarioNormalAsync(oldJefeId);
                }

                
                if (newJefe is not null)
                {
                    var promotionSuccess = await _roleChangesService.PromoteToJefeAreaAsync(newJefe.Id);
                    if (!promotionSuccess)
                    {
                        
                        return Error.Failure("No se pudo promover al usuario a Jefe de Área.");
                    }
                }

               
                areaToUpdate.Nombre = request.Nombre;
                areaToUpdate.JefeAreaId = request.JefeAreaId;

                _uow.Area.Update(areaToUpdate);
                await _uow.SaveChangesAsync();

                
                await transaction.CommitAsync();

               
                return new AreaResponse(
                    areaToUpdate.Id,
                    areaToUpdate.Nombre,
                    areaToUpdate.JefeAreaId,
                 
                    (await _userManager.FindByIdAsync(areaToUpdate.JefeAreaId))?.FullName ?? "No Asignado"
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // _logger.LogError(ex, "Error al actualizar el área {AreaId}", request.Id);
                return Error.Failure("Ocurrió un error inesperado al actualizar el área.");
            }
        }
    }
