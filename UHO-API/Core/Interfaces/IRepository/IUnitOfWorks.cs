using Microsoft.EntityFrameworkCore.Storage;

namespace UHO_API.Core.Interfaces.IRepository;

public interface IUnitOfWorks:IDisposable
{
    IAreaRepository Area { get; }
    Task SaveChangesAsync();
    void SaveChanges();
    
    
    // 
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}