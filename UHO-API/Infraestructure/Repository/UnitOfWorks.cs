using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using UHO_API.Data;
using UHO_API.Interfaces.IRepository;
using UHO_API.Models;

namespace UHO_API.Infraestructure.Repository;

public class UnitOfWorks:IUnitOfWorks
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private IDbContextTransaction _transaction;
    
    public UnitOfWorks(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
        Area = new AreaRepository(context, userManager);
    }


    public IAreaRepository Area { get; private set; }
    
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public void SaveChanges() => _context.SaveChanges();
    
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_transaction == null)
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }
        return _transaction;
    }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            
            await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackAsync();
            throw; 
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}   