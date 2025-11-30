using Microsoft.AspNetCore.Identity;
using UHO_API.Data;
using UHO_API.Interfaces.IRepository;
using UHO_API.Models;

namespace UHO_API.Infraestructure.Repository;

public class AreaRepository:Repository<AreaModel>,IAreaRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    
    public AreaRepository(ApplicationDbContext context,UserManager<ApplicationUser> userManager) : base(context)
    {
        _userManager = userManager;
        _context = context;
    }
    public AreaRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> GetResponsable(string jefeAreaId)
    {
        var responsable = await _userManager.FindByIdAsync(jefeAreaId);

        if (responsable is null) return null;
        
        return responsable;

    }
}