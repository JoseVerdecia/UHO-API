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

    public DegradeUserToUsuarioNormalHandler(
        UserManager<ApplicationUser> userManager, 
        IRoleChangesService roleChangesService,
        IValidator<DegradeUserToUsuarioNormalCommand> validator,
        IUnitOfWorks uow)
    {
        _userManager = userManager;
        _roleChangesService = roleChangesService;
        _validator = validator;
        _uow = uow;
    }

    public async Task<Result<ApplicationUser>> Handle(DegradeUserToUsuarioNormalCommand request, CancellationToken cancellationToken)
    {
        
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.Validation(validationResult.Errors.First().ErrorMessage);
        }

       
        var user = await _userManager.FindByIdAsync(request.UserId);
        
        if (user is null)
        {
            return Error.NotFound("Usuario no encontrado.");
        }

       
        if (await _userManager.IsInRoleAsync(user, Roles.UsuarioNormal) && 
            !await _userManager.IsInRoleAsync(user, Roles.JefeArea) && 
            !await _userManager.IsInRoleAsync(user, Roles.JefeProceso))
        {
            return Error.Validation("El usuario ya tiene el rol 'UsuarioNormal'.");
        }

      
        var success = await _roleChangesService.DemoteToUsuarioNormalAsync(request.UserId);

        if (!success)
        {
            
            return Error.Failure("No se pudo modificar los roles del usuario. Inténtelo de nuevo.");
        }

        AreaModel? area = await _uow.Area.Get(a => a.JefeAreaId == user.Id);

        if (area is null)
            return Error.NotFound("El Area no existe");

        area.JefeAreaId = null;
        area.JefeArea = null;
        
        _uow.Area.Update(area);
        await _uow.SaveChangesAsync();

       
        return user;
    }
}