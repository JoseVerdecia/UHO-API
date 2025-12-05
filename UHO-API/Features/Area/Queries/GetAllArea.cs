using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Area.Dtos;
using UHO_API.Features.Area.Mappings;
using UHO_API.Shared.Results;


namespace UHO_API.Features.Area.Queries;

public record GetAllAreaQuery():IRequest<IEnumerable<AreaResponse>>;


public class GetAllAreaHandler : IRequestHandler<GetAllAreaQuery, IEnumerable<AreaResponse>>
{
    private readonly IUnitOfWorks _uow;

    public GetAllAreaHandler(IUnitOfWorks uow)
    {
        _uow = uow;
    }
    
    public async Task<Result<IEnumerable<AreaResponse>>> Handle(GetAllAreaQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<AreaModel> activeAreas = await _uow.Area.GetActive(includeProperties:"JefeArea");

        var areasResponse = activeAreas.MapToAreaResponses();
        
        return Result.Success(areasResponse);
    }
}