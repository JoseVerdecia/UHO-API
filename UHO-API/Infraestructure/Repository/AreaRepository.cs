using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UHO_API.Data;
using UHO_API.Infraestructure.SD;
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

    public async Task<AreaModel?> GetActiveByIdAsync(int id)
    {
        return await _context.Areas
            .Include(a => a.JefeArea)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task<IEnumerable<AreaModel>> GetActiveAreasAsync()
    {
        return await _context.Areas
            .Include(a => a.JefeArea)
            .Where(a => !a.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<AreaModel>> GetDeletedAreasAsync()
    {
        return await _context.Areas
            .Include(a => a.JefeArea)
            .Where(a => a.IsDeleted)
            .ToListAsync();
    }
}