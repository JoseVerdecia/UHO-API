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
            return false;
        
        if (await _userManager.IsInRoleAsync(user, Roles.JefeArea))
            return true;
        
        await _userManager.RemoveFromRoleAsync(user, Roles.UsuarioNormal);
        
        var result = await _userManager.AddToRoleAsync(user, Roles.JefeArea);
        
        return result.Succeeded;
    }

    public async Task<bool> PromoteToJefeProcesoAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
            return false;

        if (await _userManager.IsInRoleAsync(user, Roles.JefeProceso))
            return true;
        
        await _userManager.RemoveFromRoleAsync(user, Roles.UsuarioNormal);
        
        var result = await _userManager.AddToRoleAsync(user, Roles.JefeProceso);
        
        return result.Succeeded;
    }

    public async Task<bool> DemoteToUsuarioNormalAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
            return false;

   
        if (await _userManager.IsInRoleAsync(user, Roles.UsuarioNormal) && 
            !await _userManager.IsInRoleAsync(user, Roles.JefeArea) && 
            !await _userManager.IsInRoleAsync(user, Roles.JefeProceso))
            return true;


        var removeJefeArea = await _userManager.IsInRoleAsync(user, Roles.JefeArea) 
            ? _userManager.RemoveFromRoleAsync(user, Roles.JefeArea) 
            : Task.CompletedTask;

        var removeJefeProceso = await _userManager.IsInRoleAsync(user, Roles.JefeProceso) 
            ? _userManager.RemoveFromRoleAsync(user, Roles.JefeProceso) 
            : Task.CompletedTask;
            
        await Task.WhenAll(removeJefeArea, removeJefeProceso);
        
        if (!await _userManager.IsInRoleAsync(user, Roles.UsuarioNormal))
        {
            var result = await _userManager.AddToRoleAsync(user, Roles.UsuarioNormal);
            return result.Succeeded;
        }

        return true; 
    }
}