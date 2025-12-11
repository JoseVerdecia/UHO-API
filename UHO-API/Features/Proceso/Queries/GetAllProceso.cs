using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Proceso.Dtos;
using UHO_API.Features.Proceso.Mappings;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Proceso.Queries;

public record GetAllProcesoQuery():IRequest<IEnumerable<ProcesoDto>>;


public class GetAllProcesoHandler : IRequestHandler<GetAllProcesoQuery, IEnumerable<ProcesoDto>>
{
    private readonly IUnitOfWorks _uow;

    public GetAllProcesoHandler(IUnitOfWorks uow)
    {
        _uow = uow;
    }
    
    public async Task<Result<IEnumerable<ProcesoDto>>> Handle(GetAllProcesoQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<ProcesoModel> activeProcesos = await _uow.Proceso.GetActive(includeProperties:"JefeDeProceso");

        var procesoDto = activeProcesos.MapToProcesosDto();
        
        return Result.Success(procesoDto);
    }
}