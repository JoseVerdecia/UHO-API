using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Indicador.Dtos;
using UHO_API.Features.Indicador.Mappings;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Indicador.Queries;

public record GetAllIndicadoresQuery() : IRequest<IEnumerable<IndicadorDto>>;

public class GetAllIndicadoresHandler : IRequestHandler<GetAllIndicadoresQuery, IEnumerable<IndicadorDto>>
{
    private readonly IUnitOfWorks _uow;

    public GetAllIndicadoresHandler(IUnitOfWorks uow) => _uow = uow;

    public async Task<Result<IEnumerable<IndicadorDto>>> Handle(GetAllIndicadoresQuery request, CancellationToken cancellationToken)
    {
        var indicadores = await _uow.Indicador.GetAllActive(includeProperties: "Proceso,Objetivos,IndicadoresAsignados");
        var dtos = indicadores.Select(i => i.ToDto());
        return Result.Success(dtos);
    }
}