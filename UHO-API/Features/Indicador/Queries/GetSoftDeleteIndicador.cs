using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Indicador.Dtos;
using UHO_API.Features.Indicador.Mappings;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Indicador.Queries;

public record GetDeletedIndicadoresQuery() : IRequest<IEnumerable<IndicadorDto>>;

public class GetDeletedIndicadoresHandler : IRequestHandler<GetDeletedIndicadoresQuery, IEnumerable<IndicadorDto>>
{
    private readonly IUnitOfWorks _uow;

    public GetDeletedIndicadoresHandler(IUnitOfWorks uow) => _uow = uow;

    public async Task<Result<IEnumerable<IndicadorDto>>> Handle(GetDeletedIndicadoresQuery request, CancellationToken cancellationToken)
    {
        var deleted = await _uow.Indicador.GetDeleted(includeProperties: "Proceso,Objetivos,IndicadoresAsignados");
        return Result.Success(deleted.Select(d => d.ToDto()));
    }
}
