using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Indicador.Dtos;
using UHO_API.Features.Indicador.Mappings;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Indicador.Queries;

public record GetIndicadorByIdQuery(int Id) : IRequest<IndicadorDto>;

public class GetIndicadorByIdHandler : IRequestHandler<GetIndicadorByIdQuery, IndicadorDto>
{
    private readonly IUnitOfWorks _uow;

    public GetIndicadorByIdHandler(IUnitOfWorks uow) => _uow = uow;

    public async Task<Result<IndicadorDto>> Handle(GetIndicadorByIdQuery request, CancellationToken cancellationToken)
    {
        var indicador = await _uow.Indicador.Get(i => i.Id == request.Id, includeProperties: "Proceso,Objetivos,IndicadoresAsignados");
        if (indicador is null) return Result.Failure<IndicadorDto>(Error.NotFound("Indicador", request.Id.ToString()));
        return Result.Success(indicador.ToDto());
    }
}