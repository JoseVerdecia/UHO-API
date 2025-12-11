using Microsoft.EntityFrameworkCore.Storage;

namespace UHO_API.Core.Interfaces.IRepository;

public interface IUnitOfWorks:IDisposable
{
    IAreaRepository Area { get; }
    IProcesoRepository Proceso { get; }
    IObjetivoRepository Objetivo { get; }
    IIndicadorDeAreaRepository IndicadorDeArea { get; }
    IIndicadorRepository Indicador { get; }
    Task SaveChangesAsync();
    void SaveChanges();
    
    
    // 
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}