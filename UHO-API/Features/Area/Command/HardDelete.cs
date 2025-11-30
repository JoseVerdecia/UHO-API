using UHO_API.Interfaces;
using UHO_API.Interfaces.IRepository;
using UHO_API.Utilities;

namespace UHO_API.Features.Area.Command;

public record HardDeleteCommand(int Id):IRequest<bool>;

public class HardDeleteHandler(IUnitOfWorks _uow) : IRequestHandler<HardDeleteCommand, bool>
{
    
    public async Task<Result<bool>> Handle(HardDeleteCommand request, CancellationToken cancellationToken)
    {
        var area = await _uow.Area.Get(p => p.Id == request.Id);

        if (area is null)
            return Error.NotFound("Area no encontrada");

        _uow.Area.Delete(area);
        await _uow.SaveChangesAsync();
        
        return Result<bool>.Success(true);
    }
}