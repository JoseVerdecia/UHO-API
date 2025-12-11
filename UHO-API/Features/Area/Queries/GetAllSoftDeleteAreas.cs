using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Area.Dtos;
using UHO_API.Features.Area.Mappings;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Area.Queries;

public class GetAllSoftDeleteAreasQuery:IRequest<IEnumerable<AreaResponse>>;


public class GetAllSoftDeleteAreasHandler : IRequestHandler<GetAllSoftDeleteAreasQuery, IEnumerable<AreaResponse>>
{
    private readonly IUnitOfWorks _uow;

    public GetAllSoftDeleteAreasHandler(IUnitOfWorks uow)
    {
        _uow = uow;
    }

    public async Task<Result<IEnumerable<AreaResponse>>> Handle(GetAllSoftDeleteAreasQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<AreaModel> allDeleteAreas = await _uow.Area.GetDeletedAreasAsync();
        
        return Result.Success(allDeleteAreas.MapToAreaResponses());
    }
}


