using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Infraestructure.Data;

namespace UHO_API.Infraestructure.Repository;

public class IndicadorDeAreaRepository:Repository<IndicadorDeAreaModel>,IIndicadorDeAreaRepository
{
    public IndicadorDeAreaRepository(ApplicationDbContext context) : base(context)
    {
    }
}