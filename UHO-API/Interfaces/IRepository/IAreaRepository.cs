using UHO_API.Models;

namespace UHO_API.Interfaces.IRepository;

public interface IAreaRepository:IRepository<AreaModel>
{
    Task<ApplicationUser?> GetResponsable(string jefeAreaId);
    
}