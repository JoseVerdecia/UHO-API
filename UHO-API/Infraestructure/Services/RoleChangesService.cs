using Microsoft.AspNetCore.Identity;
using UHO_API.Infraestructure.SD;
using UHO_API.Interfaces;
using UHO_API.Models;

namespace UHO_API.Infraestructure.Services;

public class RoleChangesService : IRoleChangesService
{
    private readonly UserManager<ApplicationUser> _userManager;
    

    public RoleChangesService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> PromoteToJefeAreaAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false; // Usuario no encontrado
        }

        // Si ya es JefeArea, no hacemos nada.
        if (await _userManager.IsInRoleAsync(user, Roles.JefeArea))
        {
            return true; // Ya está en el estado deseado.
        }

        // Quitamos el rol de JefeProceso si lo tuviera, para evitar conflictos.
        if (await _userManager.IsInRoleAsync(user, Roles.JefeProceso))
        {
            var removeResult = await _userManager.RemoveFromRoleAsync(user, Roles.JefeProceso);
            if (!removeResult.Succeeded) return false;
        }

        // Nos aseguramos que tenga el rol base.
        if (!await _userManager.IsInRoleAsync(user, Roles.UsuarioNormal))
        {
            var addBaseResult = await _userManager.AddToRoleAsync(user, Roles.UsuarioNormal);
            if (!addBaseResult.Succeeded) return false;
        }

        // Añadimos el nuevo rol de JefeArea.
        var result = await _userManager.AddToRoleAsync(user, Roles.JefeArea);
        return result.Succeeded;
    }

    public async Task<bool> PromoteToJefeProcesoAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        if (await _userManager.IsInRoleAsync(user, Roles.JefeProceso))
        {
            return true;
        }

        // Quitamos el rol de JefeArea si lo tuviera.
        if (await _userManager.IsInRoleAsync(user, Roles.JefeArea))
        {
            var removeResult = await _userManager.RemoveFromRoleAsync(user, Roles.JefeArea);
            if (!removeResult.Succeeded) return false;
        }

        // Nos aseguramos que tenga el rol base.
        if (!await _userManager.IsInRoleAsync(user, Roles.UsuarioNormal))
        {
            var addBaseResult = await _userManager.AddToRoleAsync(user, Roles.UsuarioNormal);
            if (!addBaseResult.Succeeded) return false;
        }

        // Añadimos el nuevo rol de JefeProceso.
        var result = await _userManager.AddToRoleAsync(user, Roles.JefeProceso);
        return result.Succeeded;
    }

    public async Task<bool> DemoteToUsuarioNormalAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        // Si ya es solo UsuarioNormal, no hacemos nada.
        if (await _userManager.IsInRoleAsync(user, Roles.UsuarioNormal) && 
            !await _userManager.IsInRoleAsync(user, Roles.JefeArea) && 
            !await _userManager.IsInRoleAsync(user, Roles.JefeProceso))
        {
            return true;
        }

        // Quitamos los roles de jefe si los tiene.
        var removeJefeArea = await _userManager.IsInRoleAsync(user, Roles.JefeArea) 
            ? _userManager.RemoveFromRoleAsync(user, Roles.JefeArea) 
            : Task.CompletedTask;

        var removeJefeProceso = await _userManager.IsInRoleAsync(user, Roles.JefeProceso) 
            ? _userManager.RemoveFromRoleAsync(user, Roles.JefeProceso) 
            : Task.CompletedTask;
            
        await Task.WhenAll(removeJefeArea, removeJefeProceso);

        // Nos aseguramos que tenga el rol base.
        if (!await _userManager.IsInRoleAsync(user, Roles.UsuarioNormal))
        {
            var result = await _userManager.AddToRoleAsync(user, Roles.UsuarioNormal);
            return result.Succeeded;
        }

        return true; 
    }
}