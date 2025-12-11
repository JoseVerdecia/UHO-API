using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Area.Dtos;
using UHO_API.Features.Area.Mappings;
using UHO_API.Features.Proceso.Dtos;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Area.Queries;

public record GetSoftDeleteAreaQuery(int id):IRequest<AreaResponse>;

public class GetSoftDeleteAreaHandler : IRequestHandler<GetSoftDeleteAreaQuery, AreaResponse>
{
    private readonly IUnitOfWorks _uow;

    public GetSoftDeleteAreaHandler(IUnitOfWorks uow)
    {
        _uow = uow;
    }

    public async Task<Result<AreaResponse>> Handle(GetSoftDeleteAreaQuery request, CancellationToken cancellationToken)
    {
        AreaModel? area = await _uow.Area.Get(p => p.Id == request.id && p.IsDeleted == true);

        if (area is null)
            return Result.Failure<AreaResponse>(Error.NotFound("Area","El Area no existe"));

        var result =  _uow.Proceso.SoftDelete(area.Id);

        if (!result.IsCompleted)
            return Result.Failure<AreaResponse>(Error.NotFound("Area","El Area no existe"));
        
        return  Result.Success(area.MapToAreaResponse());
    }
}