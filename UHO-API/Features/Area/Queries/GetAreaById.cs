using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Area.Dtos;
using UHO_API.Shared.Results;


namespace UHO_API.Features.Area.Queries;

public record GetAreaByIdQuery(int Id) : IRequest<AreaResponse>;

public class GetAreaByIdHandler : IRequestHandler<GetAreaByIdQuery, AreaResponse>
{
    private readonly IUnitOfWorks _uow;

    public GetAreaByIdHandler(IUnitOfWorks uow)
    {
        _uow = uow;
    }

    public async Task<Result<AreaResponse>> Handle(GetAreaByIdQuery query, CancellationToken cancellationToken)
    {
        // TODO: En vez de utilizar Get , debo utilizar GetActiveById para las Areas no SoftEliminadas
        var area = await _uow.Area.Get(q=>q.Id == query.Id,includeProperties:"JefeArea");
        
        if (area is null)
        {
            return Result.Failure<AreaResponse>(
                Error.NotFound("Area", query.Id.ToString())
            );
        }
        
        return Result.Success(new AreaResponse(
                area.Id,
                area.Nombre,
                area.JefeAreaId ?? "No Asignado",
                area.JefeArea?.FullName ?? "No Asignado",
                area.JefeArea?.Email ?? "N/A")
        );
    }
}