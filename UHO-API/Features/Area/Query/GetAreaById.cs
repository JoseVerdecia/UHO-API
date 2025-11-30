using UHO_API.Features.Area.Dto;
using UHO_API.Features.Area.Mappings;
using UHO_API.Interfaces;
using UHO_API.Interfaces.IRepository;
using UHO_API.Utilities;

namespace UHO_API.Features.Area.Query;

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
        // Obtenemos el área incluyendo al jefe de área
        var area = await _uow.Area.Get(a => a.Id == query.Id, includeProperties: "JefeArea");
        
        if (area is null)
        {
            return Error.NotFound("Área no encontrada.");
        }
        return new AreaResponse(
            area.Id,
            area.Nombre,
            area.JefeAreaId ?? "No Asignado",
            area.JefeArea?.FullName ?? "No Asignado"
            );
    }
}