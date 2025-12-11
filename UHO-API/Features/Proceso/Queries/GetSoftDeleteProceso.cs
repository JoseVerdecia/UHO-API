using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Proceso.Dtos;
using UHO_API.Features.Proceso.Mappings;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Proceso.Queries;

public record GetSoftDeleteProcesoQuery(int id):IRequest<ProcesoDto>;


public class GetSoftDeletePrcesoHandler : IRequestHandler<GetSoftDeleteProcesoQuery, ProcesoDto>
{
    private readonly IUnitOfWorks _uow;

    public GetSoftDeletePrcesoHandler(IUnitOfWorks uow)
    {
        _uow = uow;
    }

    public async Task<Result<ProcesoDto>> Handle(GetSoftDeleteProcesoQuery request, CancellationToken cancellationToken)
    {
        ProcesoModel? proceso = await _uow.Proceso.Get(p => p.Id == request.id && p.IsDeleted == true);

        if (proceso is null)
            return Result.Failure<ProcesoDto>(Error.NotFound("Proceso","El Proceso no existe"));

        var result =  _uow.Proceso.SoftDelete(proceso.Id);

        if (!result.IsCompleted)
            return Result.Failure<ProcesoDto>(Error.NotFound("Proceso","Proceso no existe"));
        
        return  Result.Success(proceso.MapToDto());
    }
}