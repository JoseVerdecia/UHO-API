using Microsoft.AspNetCore.Identity;
using UHO_API.Features.Area.Dto;
using UHO_API.Features.Area.Mappings;
using UHO_API.Interfaces;
using UHO_API.Interfaces.IRepository;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Area.Command;

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
        var existingArea = await _uow.Area.Get(a => a.Nombre == request.Nombre);
        
        if (existingArea is not null)
        {
            return Error.Validation("Ya existe un área con ese nombre.");
        }

        ApplicationUser? jefeArea =await _userManager.FindByIdAsync(request.JefeAreaId);

        AreaModel newArea = new AreaModel();
        
        newArea.Nombre = request.Nombre;
        
        if (jefeArea is not null)
        {
            newArea.JefeArea = jefeArea;
            newArea.JefeAreaId = jefeArea.Id;
            await _roleChangesService.PromoteToJefeAreaAsync(jefeArea.Id);
        }
        
        _uow.Area.Add(newArea);
        await _uow.SaveChangesAsync();
        
        
        return new AreaResponse(
            newArea.Id,
            newArea.Nombre,
            newArea.JefeAreaId ?? "No Asignado",
            newArea.JefeArea?.FullName?? "No Asignado"
        );
    }
}