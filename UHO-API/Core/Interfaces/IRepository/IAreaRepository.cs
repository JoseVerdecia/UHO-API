using UHO_API.Core.Entities;

namespace UHO_API.Core.Interfaces.IRepository;

public interface IAreaRepository:IRepository<AreaModel>
{
    Task<ApplicationUser?> GetResponsable(string jefeAreaId);
    Task<AreaModel?> GetActiveByIdAsync(int id);
    Task<IEnumerable<AreaModel>> GetActiveAreasAsync();
    Task<IEnumerable<AreaModel>> GetDeletedAreasAsync();
    
}