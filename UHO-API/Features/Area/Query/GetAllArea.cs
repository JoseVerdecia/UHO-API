using UHO_API.Features.Area.Dto;
using UHO_API.Features.Area.Mappings;
using UHO_API.Interfaces;
using UHO_API.Interfaces.IRepository;
using UHO_API.Models;
using UHO_API.Utilities;

namespace UHO_API.Features.Area.Query;

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