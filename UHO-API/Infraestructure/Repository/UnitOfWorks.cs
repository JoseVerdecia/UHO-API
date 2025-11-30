using Microsoft.AspNetCore.Identity;
using UHO_API.Data;
using UHO_API.Interfaces.IRepository;
using UHO_API.Models;

namespace UHO_API.Infraestructure.Repository;

public class UnitOfWorks:IUnitOfWorks, IAsyncDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public UnitOfWorks(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
        Area = new AreaRepository(context, userManager);
    }


    public IAreaRepository Area { get; private set; }
    
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public void SaveChanges() => _context.SaveChanges();

    public void Dispose() => _context.Dispose();

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}   