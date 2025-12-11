using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Proceso.Dtos;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Proceso.Queries;

public record GetProcesoByIdQuery(int Id) : IRequest<ProcesoDto>;

public class GetProcesoByIdHandler : IRequestHandler<GetProcesoByIdQuery, ProcesoDto>
{
    private readonly IUnitOfWorks _uow;

    public GetProcesoByIdHandler(IUnitOfWorks uow)
    {
        _uow = uow;
    }

    public async Task<Result<ProcesoDto>> Handle(GetProcesoByIdQuery query, CancellationToken cancellationToken)
    {
        // TODO: En vez de utilizar Get , debo utilizar GetActiveById para las Procesos no SoftEliminadas
        var proceso = await _uow.Proceso.GetActiveById(query.Id,includeProperties:"JefeDeProceso");
        
        if (proceso is null)
        {
            return Result.Failure<ProcesoDto>(
                Error.NotFound("Proceso", query.Id.ToString())
            );
        }
        
        return Result.Success(new ProcesoDto{
            Id = proceso.Id,
            Nombre = proceso.Nombre,
           JefeProcesoId = proceso.JefeDeProcesoId ?? "No Asignado",
            JefeProcesoNombre = proceso.JefeDeProceso?.FullName ?? "No Asignado",
            Email = proceso.JefeDeProceso?.Email ?? "N/A"
        });
    }
}