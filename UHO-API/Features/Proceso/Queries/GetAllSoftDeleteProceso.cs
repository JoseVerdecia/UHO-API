using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Proceso.Dtos;
using UHO_API.Features.Proceso.Mappings;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Proceso.Queries;

public class GetAllSoftDeleteProcesosQuery:IRequest<IEnumerable<ProcesoDto>>;



public class GetAllSoftDeleteProcesosQueryHandler : IRequestHandler<GetAllSoftDeleteProcesosQuery, IEnumerable<ProcesoDto>>
{
    private readonly IUnitOfWorks _uow;

    public GetAllSoftDeleteProcesosQueryHandler(IUnitOfWorks uow)
    {
        _uow = uow;
    }

    public async Task<Result<IEnumerable<ProcesoDto>>> Handle(GetAllSoftDeleteProcesosQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ProcesoModel> allDeleteProcesos = await _uow.Proceso.GetDeleted();
        
        return Result.Success(allDeleteProcesos.MapToProcesosDto());
    }
}


